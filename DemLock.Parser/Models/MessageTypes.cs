namespace DemLock.Parser.Models;

/// <summary>
/// Enum containing the mapping for ints to the message type so that
/// parsed messages can be properly mapped to the handler function
/// </summary>
public enum MessageTypes
{
    // ReSharper disable InconsistentNaming
    
    svc_ServerInfo = 40,
    svc_PacketEntities = 55

    // ReSharper enable InconsistentNaming
}