using DemLock.Parser.Models;

namespace DemLock.Parser.Events;

public class GameEvents
{
    public Action<CMsgFireBullets>? OnFireBullets;
    public Action<CMsgSource1LegacyGameEventList>? OnSource1LegacyGameEventList;
    public Action<CMsgSource1LegacyGameEvent>? OnSource1LegacyGameEvent;
    public Action<CMsgSosStartSoundEvent>? OnSosStartSoundEvent;
    public Action<CMsgSosStopSoundEvent>? OnSosStopSoundEvent;
    public Action<CMsgSosSetSoundEventParams>? OnSosSetSoundEventParams;
    public Action<CMsgSosStopSoundEventHash>? OnSosStopSoundEventHash;
    public Action<CMsgPlayerAnimEvent>? OnPlayerAnimEvent;
    public Action<CMsgParticleSystemManager>? OnParticleSystemManager;
    public Action<CMsgScreenTextPretty>? OnScreenTextPretty;
    public Action<CMsgServerRequestedTracer>? OnServerRequestedTracer;
    public Action<CMsgBulletImpact>? OnBulletImpact;
    public Action<CMsgEnableSatVolumesEvent>? OnEnableSatVolumesEvent;
    public Action<CMsgPlaceSatVolumeEvent>? OnPlaceSatVolumeEvent;
    public Action<CMsgDisableSatVolumesEvent>? OnDisableSatVolumesEvent;
    public Action<CMsgRemoveSatVolumeEvent>? OnRemoveSatVolumeEvent;
    
    internal GameEvents(){}
    internal void HandleGameEventMessage(MessageTypes type, byte[] data)
    {
        switch (type)
        {
            case MessageTypes.GE_FireBullets:
                if (OnFireBullets != null)
                {
                   var eventData = CMsgFireBullets.Parser.ParseFrom(data); 
                   OnFireBullets.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_Source1LegacyGameEventList:
                if (OnSource1LegacyGameEventList != null)
                {
                   var eventData = CMsgSource1LegacyGameEventList.Parser.ParseFrom(data); 
                   OnSource1LegacyGameEventList.Invoke( eventData);
                }
                break;
            case MessageTypes.GE_Source1LegacyGameEvent:
                if (OnSource1LegacyGameEvent != null)
                {
                   var eventData = CMsgSource1LegacyGameEvent.Parser.ParseFrom(data); 
                   OnSource1LegacyGameEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_SosStartSoundEvent:
                if (OnSosStartSoundEvent != null)
                {
                   var eventData = CMsgSosStartSoundEvent.Parser.ParseFrom(data); 
                   OnSosStartSoundEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_SosStopSoundEvent:
                if (OnSosStopSoundEvent != null)
                {
                   var eventData = CMsgSosStopSoundEvent.Parser.ParseFrom(data); 
                   OnSosStopSoundEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_SosSetSoundEventParams:
                if (OnSosSetSoundEventParams != null)
                {
                   var eventData = CMsgSosSetSoundEventParams.Parser.ParseFrom(data); 
                   OnSosSetSoundEventParams.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_SosStopSoundEventHash:
                if (OnSosStopSoundEventHash != null)
                {
                   var eventData = CMsgSosStopSoundEventHash.Parser.ParseFrom(data); 
                   OnSosStopSoundEventHash.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_PlayerAnimEvent:
                if (OnPlayerAnimEvent != null)
                {
                   var eventData = CMsgPlayerAnimEvent.Parser.ParseFrom(data); 
                   OnPlayerAnimEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_ParticleSystemManager:
                if (OnParticleSystemManager != null)
                {
                   var eventData = CMsgParticleSystemManager.Parser.ParseFrom(data); 
                   OnParticleSystemManager.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_ScreenTextPretty:
                if (OnScreenTextPretty != null)
                {
                   var eventData = CMsgScreenTextPretty.Parser.ParseFrom(data); 
                   OnScreenTextPretty.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_ServerRequestedTracer:
                if (OnServerRequestedTracer != null)
                {
                   var eventData = CMsgServerRequestedTracer.Parser.ParseFrom(data); 
                   OnServerRequestedTracer.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_BulletImpact:
                if (OnBulletImpact != null)
                {
                   var eventData = CMsgBulletImpact.Parser.ParseFrom(data); 
                   OnBulletImpact.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_EnableSatVolumesEvent:
                if (OnEnableSatVolumesEvent != null)
                {
                   var eventData = CMsgEnableSatVolumesEvent.Parser.ParseFrom(data); 
                   OnEnableSatVolumesEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_PlaceSatVolumeEvent:
                if (OnPlaceSatVolumeEvent != null)
                {
                   var eventData = CMsgPlaceSatVolumeEvent.Parser.ParseFrom(data); 
                   OnPlaceSatVolumeEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_DisableSatVolumesEvent:
                if (OnDisableSatVolumesEvent != null)
                {
                   var eventData = CMsgDisableSatVolumesEvent.Parser.ParseFrom(data); 
                   OnDisableSatVolumesEvent.Invoke(eventData);
                }
                break;
            case MessageTypes.GE_RemoveSatVolumeEvent:
                if (OnRemoveSatVolumeEvent != null)
                {
                   var eventData = CMsgRemoveSatVolumeEvent.Parser.ParseFrom(data); 
                   OnRemoveSatVolumeEvent.Invoke(eventData);
                }
                break;
        }
    }



}