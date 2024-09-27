using DemLock.Parser.Models;

namespace DemLock.Parser;

public class PacketMessages
{
    public Action<MessageTypes, byte[]>? OnGameEvent;
    public Action<MessageTypes, byte[]>? OnNetMessage;
    public Action<MessageTypes, byte[]>? OnServiceMessage;
    public Action<MessageTypes, byte[]>? OnUserMessage;
    
    /// <summary>
    /// Take in a byte array containing the data of a packet frame that has a series of messages in it
    /// that need to be processed
    /// </summary>
    /// <param name="bytes"></param>
    internal void ProcessMessage(MessageTypes type, byte[] data)
    {
        switch (type)
        {
            case MessageTypes.net_Tick:
            case MessageTypes.net_SetConVar:
            case MessageTypes.net_SignonState:
            case MessageTypes.net_SpawnGroup_Load:
            case MessageTypes.net_SpawnGroup_SetCreationTick:
                OnNetMessage?.Invoke(type, data);
                break;
            
            case MessageTypes.svc_ServerInfo:
            case MessageTypes.svc_ClassInfo:
            case MessageTypes.svc_CreateStringTable:
            case MessageTypes.svc_UpdateStringTable:
            case MessageTypes.svc_VoiceInit:
            case MessageTypes.svc_ClearAllStringTables:
            case MessageTypes.svc_PacketEntities:
            case MessageTypes.svc_HLTVStatus:
                OnServiceMessage?.Invoke(type, data);
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
                OnGameEvent?.Invoke(type, data);
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
                OnUserMessage?.Invoke(type, data);
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
    }}