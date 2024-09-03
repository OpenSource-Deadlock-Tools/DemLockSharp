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
    svc_ServerInfo = 40,
    svc_ClassInfo = 42,
    svc_CreateStringTable = 44,
    svc_UpdateStringTable = 45,
	svc_VoiceInit = 46,
    svc_ClearAllStringTables = 51,
    svc_PacketEntities = 55,
    UM_ParticleManager = 145,
	GE_Source1LegacyGameEventList = 205,
	GE_Source1LegacyGameEvent = 207,
	GE_SosStartSoundEvent = 208,
	k_EUserMsg_Damage = 300,
	k_EUserMsg_TriggerDamageFlash = 308,
	GE_FireBullets = 450,
	GE_BulletImpact = 461,

    // ReSharper enable InconsistentNaming
}