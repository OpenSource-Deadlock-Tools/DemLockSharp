using DemLock.Parser.Models;

namespace DemLock.Parser.Events;

public class NetMessages
{

    public Action<CNETMsg_Tick>? OnTick;
    public Action<CNETMsg_SetConVar>? OnSetConVar;
    public Action<CNETMsg_SignonState>? OnSignonState;
    public Action<CNETMsg_SpawnGroup_Load>? OnSpawnGroupLoad;
    public Action<CNETMsg_SpawnGroup_SetCreationTick>? OnSpawnGroupSetCreationTick;
    internal NetMessages(){}
    internal void HandleNetMessage(MessageTypes type, byte[] data)
    {
        switch (type)
        {
            case MessageTypes.net_Tick:
                   OnTick?.Invoke(CNETMsg_Tick.Parser.ParseFrom(data));
                break;
            case MessageTypes.net_SetConVar:
                   OnSetConVar?.Invoke(CNETMsg_SetConVar.Parser.ParseFrom(data));
                break;
            case MessageTypes.net_SignonState:
                   OnSignonState?.Invoke(CNETMsg_SignonState.Parser.ParseFrom(data));
                break;
            case MessageTypes.net_SpawnGroup_Load:
                   OnSpawnGroupLoad?.Invoke(CNETMsg_SpawnGroup_Load.Parser.ParseFrom(data));
                break;
            case MessageTypes.net_SpawnGroup_SetCreationTick:
                   OnSpawnGroupSetCreationTick?.Invoke(CNETMsg_SpawnGroup_SetCreationTick.Parser.ParseFrom(data));
                break;
        }
    }
}