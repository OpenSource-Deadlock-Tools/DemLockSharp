using DemLock.Parser.Models;

namespace DemLock.Parser.Events;

public class ServiceMessages
{


    public Action<CSVCMsg_ServerInfo>? OnServerInfo;
    public Action<CSVCMsg_ClassInfo>? OnClassInfo;
    public Action<CSVCMsg_CreateStringTable>? OnCreateStringTable;
    public Action<CSVCMsg_UpdateStringTable>? OnUpdateStringTable;
    public Action<CSVCMsg_VoiceInit>? OnVoiceInit;
    public Action<CSVCMsg_ClearAllStringTables>? OnClearAllStringTables;
    public Action<CSVCMsg_PacketEntities>? OnPacketEntities;
    public Action<CSVCMsg_HLTVStatus>? OnHLTVStatus;
    
    internal ServiceMessages(){}

    internal void HandleServiceMessage(MessageTypes type, byte[] data)
    {
        switch (type)
        {
            case MessageTypes.svc_ServerInfo:
                break;
            case MessageTypes.svc_ClassInfo:
                break;
            case MessageTypes.svc_CreateStringTable:
                break;
            case MessageTypes.svc_UpdateStringTable:
                break;
            case MessageTypes.svc_VoiceInit:
                break;
            case MessageTypes.svc_ClearAllStringTables:
                break;
            case MessageTypes.svc_PacketEntities:
                break;
            case MessageTypes.svc_HLTVStatus:
                break;
        }
    }

}