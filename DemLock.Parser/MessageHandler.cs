using System.Text.Json.Nodes;
using DemLock.Parser.Models;
using DemLock.Utils;
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
            case MessageTypes.net_Tick:
                break;
            case MessageTypes.net_SetConVar:
                break;
            case MessageTypes.net_SignonState:
                break;
            case MessageTypes.net_SpawnGroup_Load:
                break;
            case MessageTypes.net_SpawnGroup_SetCreationTick:
                break;
            case MessageTypes.svc_ServerInfo:
                ProcessServerInfo(data);
                break;
            case MessageTypes.svc_ClassInfo:
                break;
            case MessageTypes.svc_CreateStringTable:
                ProcessCreateStringTable(data);
                break;
            case MessageTypes.svc_UpdateStringTable:
                ProcessUpdateStringTable(data);
                break;
            case MessageTypes.svc_VoiceInit:
                break;
            case MessageTypes.svc_ClearAllStringTables:
                break;
            case MessageTypes.svc_PacketEntities:
                ProcessPacketEntities(data);
                break;
            case MessageTypes.svc_HLTVStatus:
                break;
            case MessageTypes.UM_ParticleManager:
                break;
            case MessageTypes.UM_PlayResponseConditional:
                break;
            case MessageTypes.GE_Source1LegacyGameEventList:
                break;
            case MessageTypes.GE_Source1LegacyGameEvent:
                break;
            case MessageTypes.GE_SosStartSoundEvent:
                break;
            case MessageTypes.GE_SosStopSoundEvent:
                break;
            case MessageTypes.GE_SosSetSoundEventParams:
                break;
            case MessageTypes.GE_SosStopSoundEventHash:
                break;
            case MessageTypes.GE_FireBullets:
                break;
            case MessageTypes.GE_PlayerAnimEvent:
                break;
            case MessageTypes.GE_ParticleSystemManager:
                break;
            case MessageTypes.GE_ScreenTextPretty:
                break;
            case MessageTypes.GE_ServerRequestedTracer:
                break;
            case MessageTypes.GE_BulletImpact:
                break;
            case MessageTypes.GE_EnableSatVolumesEvent:
                break;
            case MessageTypes.GE_PlaceSatVolumeEvent:
                break;
            case MessageTypes.GE_DisableSatVolumesEvent:
                break;
            case MessageTypes.GE_RemoveSatVolumeEvent:
                break;
            case MessageTypes.k_EUserMsg_Damage:
                break;
            case MessageTypes.k_EUserMsg_MapPing:
                break;
            case MessageTypes.k_EUserMsg_TeamRewards:
                break;
            case MessageTypes.k_EUserMsg_AbilityFailed:
                break;
            case MessageTypes.k_EUserMsg_TriggerDamageFlash:
                break;
            case MessageTypes.k_EUserMsg_AbilitiesChanged:
                break;
            case MessageTypes.k_EUserMsg_RecentDamageSummary:
                break;
            case MessageTypes.k_EUserMsg_SpectatorTeamChanged:
                break;
            case MessageTypes.k_EUserMsg_ChatWheel:
                break;
            case MessageTypes.k_EUserMsg_GoldHistory:
                break;
            case MessageTypes.k_EUserMsg_ChatMsg:
                break;
            case MessageTypes.k_EUserMsg_QuickResponse:
                break;
            case MessageTypes.k_EUserMsg_PostMatchDetails:
                ProcessPostMatchDetails(data);
                break;
            case MessageTypes.k_EUserMsg_ChatEvent:
                break;
            case MessageTypes.k_EUserMsg_AbilityInterrupted:
                break;
            case MessageTypes.k_EUserMsg_HeroKilled:
                break;
            case MessageTypes.k_EUserMsg_ReturnIdol:
                break;
            case MessageTypes.k_EUserMsg_SetClientCameraAngles:
                break;
            case MessageTypes.k_EUserMsg_MapLine:
                break;
            case MessageTypes.k_EUserMsg_BulletHit:
                break;
            case MessageTypes.k_EUserMsg_ObjectiveMask:
                break;
            case MessageTypes.k_EUserMsg_ModifierApplied:
                break;
            case MessageTypes.k_EUserMsg_CameraController:
                break;
            case MessageTypes.k_EUserMsg_AuraModifierApplied:
                break;
            case MessageTypes.k_EUserMsg_ObstructedShotFired:
                break;
            case MessageTypes.k_EUserMsg_AbilityLateFailure:
                break;
            case MessageTypes.k_EUserMsg_AbilityPing:
                break;
            case MessageTypes.k_EUserMsg_PostProcessingAnim:
                break;
            case MessageTypes.k_EUserMsg_DeathReplayData:
                break;
            case MessageTypes.k_EUserMsg_PlayerLifetimeStatInfo:
                break;
            case MessageTypes.k_EUserMsg_ForceShopClosed:
                break;
            case MessageTypes.k_EUserMsg_StaminaDrained:
                break;
            case MessageTypes.k_EUserMsg_AbilityNotify:
                break;
            case MessageTypes.k_EUserMsg_GetDamageStatsResponse:
                break;
            case MessageTypes.k_EUserMsg_ParticipantStartSoundEvent:
                break;
            case MessageTypes.k_EUserMsg_ParticipantStopSoundEvent:
                break;
            case MessageTypes.k_EUserMsg_ParticipantStopSoundEventHash:
                break;
            case MessageTypes.k_EUserMsg_ParticipantSetSoundEventParams:
                break;
            case MessageTypes.k_EUserMsg_ParticipantSetLibraryStackFields:
                break;
            case MessageTypes.k_EUserMsg_CurrencyChanged:
                break;
            case MessageTypes.k_EUserMsg_GameOver:
                break;
            case MessageTypes.k_EUserMsg_BossKilled:
                break;
            case MessageTypes.k_EEntityMsg_BreakablePropSpawnDebris:
                break;
            case MessageTypes.TE_EffectDispatchId:
                break;
            case MessageTypes.TE_ArmorRicochetId:
                break;
            case MessageTypes.TE_BeamEntPointId:
                break;
            case MessageTypes.TE_BeamEntsId:
                break;
            case MessageTypes.TE_BeamPointsId:
                break;
            case MessageTypes.TE_BeamRingId:
                break;
            case MessageTypes.TE_BSPDecalId:
                break;
            case MessageTypes.TE_BubblesId:
                break;
            case MessageTypes.TE_BubbleTrailId:
                break;
            case MessageTypes.TE_DecalId:
                break;
            case MessageTypes.TE_WorldDecalId:
                break;
            case MessageTypes.TE_EnergySplashId:
                break;
            case MessageTypes.TE_FizzId:
                break;
            case MessageTypes.TE_ShatterSurfaceId:
                break;
            case MessageTypes.TE_GlowSpriteId:
                break;
            case MessageTypes.TE_ImpactId:
                break;
            case MessageTypes.TE_MuzzleFlashId:
                break;
            case MessageTypes.TE_BloodStreamId:
                break;
            case MessageTypes.TE_ExplosionId:
                break;
            case MessageTypes.TE_DustId:
                break;
            case MessageTypes.TE_LargeFunnelId:
                break;
            case MessageTypes.TE_SparksId:
                break;
            case MessageTypes.TE_PhysicsPropId:
                break;
            case MessageTypes.TE_PlayerDecalId:
                break;
            case MessageTypes.TE_ProjectedDecalId:
                break;
            case MessageTypes.TE_SmokeId:
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

                _events.Raise_OnEntityUpdated(_context.CurrentTick, entity, "CREATED");
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
                _events.Raise_OnEntityUpdated(_context.CurrentTick, entity, "UPDATE");
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
        _events.RaisOnCCitadelUserMsg_PostMatchDetails(this, matchDetails);
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
