namespace DemLock.Parser.Models;

/// <summary>
/// Enum containing the mapping for ints to the message type so that
/// parsed messages can be properly mapped to the handler function
/// </summary>
public enum MessageTypes
{
    // ReSharper disable InconsistentNaming

    
    net_Tick = 4,
    net_SetConVar = 6,
    net_SignonState = 7,
	net_SpawnGroup_Load = 8,
	net_SpawnGroup_SetCreationTick = 11,
    svc_ServerInfo = 40,
    svc_ClassInfo = 42,
    svc_CreateStringTable = 44,
    svc_UpdateStringTable = 45,
	svc_VoiceInit = 46,
    svc_ClearAllStringTables = 51,
    svc_PacketEntities = 55,
	svc_HLTVStatus = 62,
    UM_ParticleManager = 145,
	UM_PlayResponseConditional = 166,
	GE_Source1LegacyGameEventList = 205,
	GE_Source1LegacyGameEvent = 207,
	GE_SosStartSoundEvent = 208,
	GE_SosStopSoundEvent = 209,
	GE_SosSetSoundEventParams = 210,
	GE_SosStopSoundEventHash = 212,
	k_EUserMsg_Damage = 300,
	k_EUserMsg_TriggerDamageFlash = 308,
	k_EUserMsg_ChatMsg = 314,
	k_EUserMsg_PostMatchDetails = 316,
	k_EUserMsg_ChatEvent = 317,
	k_EUserMsg_AbilityNotify = 338,
	k_EUserMsg_HeroKilled = 319,
	k_EUserMsg_PostProcessingAnim = 332,
	TE_EffectDispatchId = 400,
	GE_FireBullets = 450,
	GE_BulletImpact = 461,
	k_EEntityMsg_BreakablePropSpawnDebris = 500,

    // ReSharper enable InconsistentNaming
}