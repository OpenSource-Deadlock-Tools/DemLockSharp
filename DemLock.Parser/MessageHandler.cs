using System.Text.Json.Nodes;
using DemLock.Parser.Events;
using DemLock.Parser.Models;
using DemLock.Utils;
using Newtonsoft.Json;
using Snappier;

namespace DemLock.Parser;

/// <summary>
/// Processor for handling messages that are extracted from demo packets
/// </summary>
public class MessageHandler
{
    private readonly DemoParserContext _context;
    private readonly GameEvents _gameEvents;
    private readonly NetMessages _netMessages;
    private readonly ServiceMessages _serviceMessages;
    private readonly UserMessages _userMessages;

    public MessageHandler( DemoParserContext context, GameEvents gameEvents,
        NetMessages netMessages, UserMessages userMessages, ServiceMessages serviceMessages)
    {
        _serviceMessages = serviceMessages;
        _userMessages = userMessages;
        _context = context;
        _gameEvents = gameEvents;
        _netMessages = netMessages;
    }

    /// <summary>
    /// Take in a byte array containing the data of a packet frame that has a series of messages in it
    /// that need to be processed
    /// </summary>
    /// <param name="bytes"></param>
    internal void ProcessMessage(MessageTypes type, byte[] data)
    {
        if (!Enum.IsDefined(typeof(MessageTypes), type))
            Console.WriteLine($"\t({(int)type}) [{data.Length}]");

        switch (type)
        {
            case MessageTypes.net_Tick:
            case MessageTypes.net_SetConVar:
            case MessageTypes.net_SignonState:
            case MessageTypes.net_SpawnGroup_Load:
            case MessageTypes.net_SpawnGroup_SetCreationTick:
                _netMessages.HandleNetMessage(type, data);
                break;
            
            case MessageTypes.svc_ServerInfo:
            case MessageTypes.svc_ClassInfo:
            case MessageTypes.svc_CreateStringTable:
            case MessageTypes.svc_UpdateStringTable:
            case MessageTypes.svc_VoiceInit:
            case MessageTypes.svc_ClearAllStringTables:
            case MessageTypes.svc_PacketEntities:
            case MessageTypes.svc_HLTVStatus:
                _serviceMessages.HandleServiceMessage(type, data);
                break;
            
            case MessageTypes.UM_ParticleManager :
            case MessageTypes.UM_PlayResponseConditional:
                break;
            
            case MessageTypes.GE_Source1LegacyGameEventList:
            case MessageTypes.GE_Source1LegacyGameEvent:
            case MessageTypes.GE_SosStartSoundEvent:
            case MessageTypes.GE_SosStopSoundEvent:
            case MessageTypes.GE_SosSetSoundEventParams:
            case MessageTypes.GE_SosStopSoundEventHash:
            case MessageTypes.GE_FireBullets:
            case MessageTypes.GE_PlayerAnimEvent:
            case MessageTypes.GE_ParticleSystemManager:
            case MessageTypes.GE_ScreenTextPretty:
            case MessageTypes.GE_ServerRequestedTracer:
            case MessageTypes.GE_BulletImpact:
            case MessageTypes.GE_EnableSatVolumesEvent:
            case MessageTypes.GE_PlaceSatVolumeEvent:
            case MessageTypes.GE_DisableSatVolumesEvent:
            case MessageTypes.GE_RemoveSatVolumeEvent:
                _gameEvents.HandleGameEventMessage(type, data);
                break;
            
            case MessageTypes.k_EUserMsg_Damage:
            case MessageTypes.k_EUserMsg_MapPing:
            case MessageTypes.k_EUserMsg_TeamRewards:
            case MessageTypes.k_EUserMsg_AbilityFailed:
            case MessageTypes.k_EUserMsg_TriggerDamageFlash:
            case MessageTypes.k_EUserMsg_AbilitiesChanged:
            case MessageTypes.k_EUserMsg_RecentDamageSummary:
            case MessageTypes.k_EUserMsg_SpectatorTeamChanged:
            case MessageTypes.k_EUserMsg_ChatWheel:
            case MessageTypes.k_EUserMsg_GoldHistory:
            case MessageTypes.k_EUserMsg_ChatMsg:
            case MessageTypes.k_EUserMsg_QuickResponse:
            case MessageTypes.k_EUserMsg_PostMatchDetails:
            case MessageTypes.k_EUserMsg_ChatEvent:
            case MessageTypes.k_EUserMsg_AbilityInterrupted:
            case MessageTypes.k_EUserMsg_HeroKilled:
            case MessageTypes.k_EUserMsg_ReturnIdol:
            case MessageTypes.k_EUserMsg_SetClientCameraAngles:
            case MessageTypes.k_EUserMsg_MapLine:
            case MessageTypes.k_EUserMsg_BulletHit:
            case MessageTypes.k_EUserMsg_ObjectiveMask:
            case MessageTypes.k_EUserMsg_ModifierApplied:
            case MessageTypes.k_EUserMsg_CameraController:
            case MessageTypes.k_EUserMsg_AuraModifierApplied:
            case MessageTypes.k_EUserMsg_ObstructedShotFired:
            case MessageTypes.k_EUserMsg_AbilityLateFailure:
            case MessageTypes.k_EUserMsg_AbilityPing:
            case MessageTypes.k_EUserMsg_PostProcessingAnim:
            case MessageTypes.k_EUserMsg_DeathReplayData:
            case MessageTypes.k_EUserMsg_PlayerLifetimeStatInfo:
            case MessageTypes.k_EUserMsg_ForceShopClosed:
            case MessageTypes.k_EUserMsg_StaminaDrained:
            case MessageTypes.k_EUserMsg_AbilityNotify:
            case MessageTypes.k_EUserMsg_GetDamageStatsResponse:
            case MessageTypes.k_EUserMsg_ParticipantStartSoundEvent:
            case MessageTypes.k_EUserMsg_ParticipantStopSoundEvent:
            case MessageTypes.k_EUserMsg_ParticipantStopSoundEventHash:
            case MessageTypes.k_EUserMsg_ParticipantSetSoundEventParams:
            case MessageTypes.k_EUserMsg_ParticipantSetLibraryStackFields:
            case MessageTypes.k_EUserMsg_CurrencyChanged:
            case MessageTypes.k_EUserMsg_GameOver:
            case MessageTypes.k_EUserMsg_BossKilled:
                _userMessages.HandleUserMessage(type, data);
                break;
            
            case MessageTypes.k_EEntityMsg_BreakablePropSpawnDebris:
                break;
            
            case MessageTypes.TE_EffectDispatchId:
            case MessageTypes.TE_ArmorRicochetId:
            case MessageTypes.TE_BeamEntPointId:
            case MessageTypes.TE_BeamEntsId:
            case MessageTypes.TE_BeamPointsId:
            case MessageTypes.TE_BeamRingId:
            case MessageTypes.TE_BSPDecalId:
            case MessageTypes.TE_BubblesId:
            case MessageTypes.TE_BubbleTrailId:
            case MessageTypes.TE_DecalId:
            case MessageTypes.TE_WorldDecalId:
            case MessageTypes.TE_EnergySplashId:
            case MessageTypes.TE_FizzId:
            case MessageTypes.TE_ShatterSurfaceId:
            case MessageTypes.TE_GlowSpriteId:
            case MessageTypes.TE_ImpactId:
            case MessageTypes.TE_MuzzleFlashId:
            case MessageTypes.TE_BloodStreamId:
            case MessageTypes.TE_ExplosionId:
            case MessageTypes.TE_DustId:
            case MessageTypes.TE_LargeFunnelId:
            case MessageTypes.TE_SparksId:
            case MessageTypes.TE_PhysicsPropId:
            case MessageTypes.TE_PlayerDecalId:
            case MessageTypes.TE_ProjectedDecalId:
            case MessageTypes.TE_SmokeId:
                break;
            default:
                Console.WriteLine($"\tUnhandled Message Type [{type}::{data.Length}]");
                break;
        }
    }

    private void ProcessFireBullets(byte[] data)
    {
        var fireBulletsEvent = CMsgFireBullets.Parser.ParseFrom(data);

        Console.WriteLine(JsonConvert.SerializeObject(fireBulletsEvent, Formatting.Indented));
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
            if (i == 2)
            {
            }

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
                {
                    _context.EntityManager.UpdateAtIndex(entityIndex, baseline);
                }

                var entity = _context.EntityManager.UpdateAtIndex(entityIndex, ref eventData);
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
                var entity = _context.EntityManager.UpdateAtIndex(entityIndex, ref eventData);
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


    private void ProcessPostMatchDetails(byte[] data)
    {
        CCitadelUserMsg_PostMatchDetails matchDetails = CCitadelUserMsg_PostMatchDetails.Parser.ParseFrom(data);
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