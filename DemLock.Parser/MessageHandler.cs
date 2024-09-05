using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities;
using DemLock.Parser.Models;
using DemLock.Utils;
using Google.Protobuf.Reflection;
using Snappier;

namespace DemLock.Parser;

/// <summary>
/// Processor for handling messages that are extracted from demo packets
/// </summary>
public class MessageHandler
{
    private readonly DemoParserContext _context;
    private readonly DemoEventSystem _events;

    public MessageHandler(DemoEventSystem events, DemoParserContext context)
    {
        _events = events;
        _context = context;
    }

    /// <summary>
    /// Take in a byte array containing the data of a packet frame that has a series of messages in it
    /// that need to be processed
    /// </summary>
    /// <param name="bytes"></param>
    public void ProcessMessage(MessageTypes type, byte[] data)
    {
        if (_context.Config.IgnoredMessages.Contains(type)) return;
        if (_context.Config.LogMessageReads)
            if (Enum.IsDefined(typeof(MessageTypes), type))
                Console.WriteLine($"\t{type}({(int)type}) [{data.Length}]");

        switch (type)
        {
            case MessageTypes.svc_PacketEntities:
                ProcessPacketEntities(data);
                break;
            case MessageTypes.svc_ServerInfo:
                ProcessServerInfo(data);
                break;
            case MessageTypes.svc_CreateStringTable:
                ProcessCreateStringTable(data);
                break;
            case MessageTypes.svc_UpdateStringTable:
                ProcessUpdateStringTable(data);
                break;
            case MessageTypes.net_Tick:
                break;
            case MessageTypes.net_SignonState:
                break;
            case MessageTypes.net_SetConVar:
                break;
            case MessageTypes.net_SpawnGroup_Load:
                break;
            case MessageTypes.net_SpawnGroup_SetCreationTick:
                break;
            case MessageTypes.k_EUserMsg_AbilityNotify:
                break;
            case MessageTypes.k_EUserMsg_ChatEvent:
                break;
            case MessageTypes.svc_ClearAllStringTables:
                break;
            case MessageTypes.UM_ParticleManager:
                break;
            case MessageTypes.GE_SosStartSoundEvent:
                break;
            case MessageTypes.GE_SosStopSoundEvent:
                break;
            case MessageTypes.GE_SosSetSoundEventParams:
                break;
            case MessageTypes.GE_Source1LegacyGameEvent:
                break;
            case MessageTypes.GE_Source1LegacyGameEventList:
                break;
            case MessageTypes.svc_VoiceInit:
                break;
            case MessageTypes.svc_ClassInfo:
                break;
            case MessageTypes.k_EUserMsg_Damage:
                break;
            case MessageTypes.k_EUserMsg_TriggerDamageFlash:
                break;
            case MessageTypes.GE_FireBullets:
                break;
            case MessageTypes.GE_BulletImpact:
                break;
            case MessageTypes.GE_SosStopSoundEventHash:
                break;
            case MessageTypes.TE_EffectDispatchId:
                break;
            case MessageTypes.svc_HLTVStatus:
                break;
            case MessageTypes.UM_PlayResponseConditional:
                break;
            case MessageTypes.k_EUserMsg_PostMatchDetails:
                break;
            case MessageTypes.k_EEntityMsg_BreakablePropSpawnDebris:
                break;
            case MessageTypes.k_EUserMsg_HeroKilled:
                break;
            case MessageTypes.k_EUserMsg_PostProcessingAnim:
                break;
            case MessageTypes.k_EUserMsg_ChatMsg:
                break;
            default:
                Console.WriteLine($"\tUnhandled Message Type [{type}::{data.Length}]");
                break;
        }
    }

    private void ProcessCreateStringTable(byte[] data)
    {
        var createStringTable = CSVCMsg_CreateStringTable.Parser.ParseFrom(data);

        // Setup all the important fields for the string table
        var newTable = new StringTable();
        newTable.Name = createStringTable.Name;
        newTable.UserDataFixedSize = createStringTable.UserDataFixedSize;
        newTable.UserDataSize = createStringTable.UserDataSizeBits;
        newTable.Flags = createStringTable.Flags;
        newTable.UsingVarintBitCounts = createStringTable.UsingVarintBitcounts;

        // Get the string data, and decompress it if needed
        byte[] buffer;
        if (createStringTable.DataCompressed)
            buffer = Snappy.DecompressToArray(createStringTable.StringData.ToByteArray());
        else
            buffer = createStringTable.StringData.ToByteArray();
        // Update the string table with the decompressed string data
        newTable.Update(buffer, createStringTable.NumEntries);
        // Add the string table to our context
        _context.AddStringTable(newTable);
    }


    private void ProcessUpdateStringTable(byte[] data)
    {
        var updateStringTable = CSVCMsg_UpdateStringTable.Parser.ParseFrom(data);
        _context.UpdateStringTableAtIndex(updateStringTable.TableId, updateStringTable.StringData.ToByteArray(),
            updateStringTable.NumChangedEntries);
    }

    private void ProcessPacketEntities(byte[] data)
    {
        CSVCMsg_PacketEntities packetEntities = CSVCMsg_PacketEntities.Parser.ParseFrom(data);

        var eventData = new BitBuffer(packetEntities.EntityData.ToArray());
        var entityIndex = -1;

        for (int i = 0; i < packetEntities.UpdatedEntries; i++)
        {
            entityIndex += 1 + (int)eventData.ReadUBitVar();

            var flags = DeltaHeaderFlags.FHDR_ZERO;
            var updateType = PacketUpdateTypes.EnterPvs;

            if (eventData.ReadBit())
            {
                flags |= DeltaHeaderFlags.FHDR_LEAVE_PVS;
                if (eventData.ReadBit())
                    flags |= DeltaHeaderFlags.FHDR_DELETE;
            }
            else if (eventData.ReadBit())
                flags |= DeltaHeaderFlags.FHDR_ENTER_PVS;

            if ((flags & DeltaHeaderFlags.FHDR_ENTER_PVS) != 0)
                updateType = PacketUpdateTypes.EnterPvs;
            else if ((flags & DeltaHeaderFlags.FHDR_LEAVE_PVS) != 0)
                updateType = PacketUpdateTypes.LeavePvs;
            else
                updateType = PacketUpdateTypes.DeltaEnt;

            if (updateType == PacketUpdateTypes.EnterPvs)
            {
                var classId = eventData.ReadBitsToUint(_context.ClassIdSize);
                var serialNum = eventData.ReadBitsToUint(DemoParserContext.NumEHandleSerialNumberBits);

                // Don't know what this is... every parser does it
                // Reference to demoinfo-net, they propose maybe it is spawngroup handles,
                // but it just doesn't really matter to us
                var unusedUnknownValue = eventData.ReadVarUInt32();

                var serverClass = _context.GetClassById((int)classId);
                _context.EntityManager.AddNewEntity(entityIndex, serverClass, serialNum);

                var baseline = _context.GetInstanceBaseline((int)classId);
                if (baseline != null)
                    _context.EntityManager.UpdateAtIndex(entityIndex, baseline);

                _context.EntityManager.UpdateAtIndex(entityIndex, ref eventData);
            }

            if (updateType == PacketUpdateTypes.LeavePvs)
            {
                if ((flags & DeltaHeaderFlags.FHDR_DELETE) != 0)
                {
                    _context.EntityManager.DeleteEntity(entityIndex);
                }
            }

            if (updateType == PacketUpdateTypes.DeltaEnt)
            {
                var updates = _context.EntityManager.UpdateAtIndex(entityIndex, ref eventData);
                _events.Raise_OnEntityUpdated(_context.CurrentTick, updates, _context.EntityManager.GetEntityAtIndex(entityIndex).ClassName, "UPDATE");
            }
        }
    }

    private void ProcessServerInfo(byte[] data)
    {
        CSVCMsg_ServerInfo serverInfo = CSVCMsg_ServerInfo.Parser.ParseFrom(data);

        _context.ClassIdSize = (int)Math.Log2(serverInfo.MaxClasses) + 1;
        _context.MaxPlayers = serverInfo.MaxClients;
        _context.TickInterval = serverInfo.TickInterval;
    }
}

public enum PacketUpdateTypes
{
    EnterPvs,
    LeavePvs,
    DeltaEnt
}

[Flags]
public enum DeltaHeaderFlags
{
    // ReSharper disable InconsistentNaming
    FHDR_ZERO = 0x0000,
    FHDR_LEAVE_PVS = 0x0001,
    FHDR_DELETE = 0x0002,

    FHDR_ENTER_PVS = 0x0004
    // ReSharper restore InconsistentNaming
}