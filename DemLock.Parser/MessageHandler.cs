using System.Diagnostics;
using DemLock.Parser.Models;
using DemLock.Utils;

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
        switch(type)
        {
            case MessageTypes.svc_PacketEntities:
                Console.WriteLine($"\tsvc_PacketEntities [{(int)type}::{data.Length}]");
                ProcessPacketEntities(data);
                break;
            case MessageTypes.svc_ServerInfo:
                Console.WriteLine($"\tsvc_ServerInfo [{(int)type}::{data.Length}]");
                ProcessServerInfo(data);
                break;
            default:
                //Console.WriteLine($"\tUnknown Message Type [{(int)type}::{data.Length}]");
                break;
        }
        
    }

    private void ProcessPacketEntities(byte[] data)
    {
        CSVCMsg_PacketEntities packetEntities = CSVCMsg_PacketEntities.Parser.ParseFrom(data);
        
        var eventData = new BitStream(packetEntities.EntityData.ToArray());
        var entityIndex = -1;

        for (int i = 0; i < packetEntities.UpdatedEntries; i++)
        {
            entityIndex += 1 + (int)eventData.ReadUBit();
            var updateType = eventData.ReadBitsToUint(2);
            if ((updateType & 0b01) != 0)
            {
                
            }else if (updateType == 0b10)
            {
                var classId = eventData.ReadBitsToUint(_context.ClassIDSize);
                var serialNum = eventData.ReadBitsToUint(DemoParserContext.NumEHandleSerialNumberBits);
                
                // Don't know what this is... every parser does it
                // Reference to demoinfo-net, they propose maybe it is spawngroup handles,
                // but it just doesn't really matter to us
                var _discard = eventData.ReadVarUInt32();
                var serverClass = _context.GetClassById((int)classId);
            }
        }
    }
    
    private void ProcessServerInfo(byte[] data)
    {
        CSVCMsg_ServerInfo serverInfo = CSVCMsg_ServerInfo.Parser.ParseFrom(data);
        
        _context.ClassIDSize = (int)Math.Log2(serverInfo.MaxClasses) + 1;
        _context.MaxPlayers = serverInfo.MaxClients;
    }
    
}