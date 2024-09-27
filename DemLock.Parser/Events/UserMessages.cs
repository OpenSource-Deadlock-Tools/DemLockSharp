using DemLock.Parser.Models;
using Google.Protobuf;

namespace DemLock.Parser.Events;

public class UserMessages
{

    public event EventHandler<CCitadelUserMessage_Damage>? OnDamage;
    public event EventHandler<CCitadelUserMsg_MapPing>? OnMapPing;
    public event EventHandler<CCitadelUserMsg_TeamRewards>? OnTeamRewards;
    public event EventHandler<CCitadelUserMsg_TriggerDamageFlash>? OnTriggerDamageFlash;
    public event EventHandler<CCitadelUserMsg_AbilitiesChanged>? OnAbilitiesChanged;
    public event EventHandler<CCitadelUserMsg_RecentDamageSummary>? OnRecentDamageSummary;
    public event EventHandler<CCitadelUserMsg_SpectatorTeamChanged>? OnSpectatorTeamChanged;
    public event EventHandler<CCitadelUserMsg_ChatWheel>? OnChatWheel;
    public event EventHandler<CCitadelUserMsg_GoldHistory>? OnGoldHistory;
    public event EventHandler<CCitadelUserMsg_ChatMsg>? OnChatMsg;
    public event EventHandler<CCitadelUserMsg_QuickResponse>? OnQuickResponse;
    public event EventHandler<CCitadelUserMsg_PostMatchDetails>? OnPostMatchDetails;
    public event EventHandler<CCitadelUserMsg_ChatEvent>? OnChatEvent;
    public event EventHandler<CCitadelUserMsg_AbilityInterrupted>? OnAbilityInterrupted;
    public event EventHandler<CCitadelUserMsg_HeroKilled>? OnHeroKilled;
    public event EventHandler<CCitadelUserMsg_ReturnIdol>? OnReturnIdol;
    public event EventHandler<CCitadelUserMsg_SetClientCameraAngles>? OnSetClientCameraAngles;
    public event EventHandler<CCitadelUserMsg_MapLine>? OnMapLine;
    public event EventHandler<CCitadelUserMessage_BulletHit>? OnBulletHit;
    public event EventHandler<CCitadelUserMessage_ObjectiveMask>? OnObjectiveMask;
    public event EventHandler<CCitadelUserMessage_ModifierApplied>? OnModifierApplied;
    public event EventHandler<CCitadelUserMsg_CameraController>? OnCameraController;
    public event EventHandler<CCitadelUserMessage_AuraModifierApplied>? OnAuraModifierApplied;
    public event EventHandler<CCitadelUserMsg_ObstructedShotFired>? OnObstructedShotFired;
    public event EventHandler<CCitadelUserMsg_AbilityLateFailure>? OnAbilityLateFailure;
    public event EventHandler<CCitadelUserMsg_AbilityPing>? OnAbilityPing;
    public event EventHandler<CCitadelUserMsg_PostProcessingAnim>? OnPostProcessingAnim;
    public event EventHandler<CCitadelUserMsg_DeathReplayData>? OnDeathReplayData;
    public event EventHandler<CCitadelUserMsg_PlayerLifetimeStatInfo>? OnPlayerLifetimeStatInfo;
    public event EventHandler<CCitadelUserMsg_ForceShopClosed>? OnForceShopClosed;
    public event EventHandler<CCitadelUserMsg_StaminaDrained>? OnStaminaDrained;
    public event EventHandler<CCitadelUserMessage_AbilityNotify>? OnAbilityNotify;
    public event EventHandler<CCitadelUserMsg_GetDamageStatsResponse>? OnGetDamageStatsResponse;
    public event EventHandler<CCitadelUserMsg_ParticipantStartSoundEvent>? OnParticipantStartSoundEvent;
    public event EventHandler<CCitadelUserMsg_ParticipantStopSoundEvent>? OnParticipantStopSoundEvent;
    public event EventHandler<CCitadelUserMsg_ParticipantStopSoundEventHash>? OnParticipantStopSoundEventHash;
    public event EventHandler<CCitadelUserMsg_ParticipantSetSoundEventParams>? OnParticipantSetSoundEventParams;
    public event EventHandler<CCitadelUserMsg_ParticipantSetLibraryStackFields>? OnParticipantSetLibraryStackFields;
    public event EventHandler<CCitadelUserMessage_CurrencyChanged>? OnCurrencyChanged;
    public event EventHandler<CCitadelUserMessage_GameOver>? OnGameOver;
    public event EventHandler<CCitadelUserMsg_BossKilled>? OnBossKilled;
    internal UserMessages(){}
    
    internal void HandleUserMessage(MessageTypes type, byte[] data)
    {
        
        switch (type)
        {
            
            case MessageTypes.k_EUserMsg_Damage:
                OnDamage?.Invoke(this, CCitadelUserMessage_Damage.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_MapPing:
                OnMapPing?.Invoke(this, CCitadelUserMsg_MapPing.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_TeamRewards:
                OnTeamRewards?.Invoke(this, CCitadelUserMsg_TeamRewards.Parser.ParseFrom(data));
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
                OnGoldHistory?.Invoke(this, CCitadelUserMsg_GoldHistory.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_ChatMsg:
                OnChatMsg?.Invoke(this, CCitadelUserMsg_ChatMsg.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_QuickResponse:
                break;
            case MessageTypes.k_EUserMsg_PostMatchDetails:
                break;
            case MessageTypes.k_EUserMsg_ChatEvent:
                OnChatEvent?.Invoke(this, CCitadelUserMsg_ChatEvent.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_AbilityInterrupted:
                break;
            case MessageTypes.k_EUserMsg_HeroKilled:
                OnHeroKilled?.Invoke(this, CCitadelUserMsg_HeroKilled.Parser.ParseFrom(data));
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
                OnModifierApplied?.Invoke(this, CCitadelUserMessage_ModifierApplied.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_CameraController:
                OnCameraController?.Invoke(this, CCitadelUserMsg_CameraController.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_AuraModifierApplied:
                OnAuraModifierApplied?.Invoke(this, CCitadelUserMessage_AuraModifierApplied.Parser.ParseFrom(data));
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
                OnCurrencyChanged?.Invoke(this, CCitadelUserMessage_CurrencyChanged.Parser.ParseFrom(data));
                break;
            case MessageTypes.k_EUserMsg_GameOver:
                break;
            case MessageTypes.k_EUserMsg_BossKilled:
                break;
        }
    }
}