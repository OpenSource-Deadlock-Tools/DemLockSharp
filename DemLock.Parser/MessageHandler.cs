using System.Diagnostics;
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
        if (Enum.IsDefined(typeof(MessageTypes), type))
        {
                Console.WriteLine($"\t{type}({(int)type}) [{data.Length}]");
        }
        switch (type)
        {
            case MessageTypes.svc_PacketEntities:
                ProcessPacketEntities(data);
                break;
            case MessageTypes.svc_ServerInfo:
                ProcessServerInfo(data);
                break;
            case MessageTypes.svc_CreateStringTable:
                ProcessCreateStringTable(data);
                break;
            case MessageTypes.svc_UpdateStringTable:
                ProcessUpdateStringTable(data);
                break;
            default:
                //Console.WriteLine($"\tUnknown Message Type [{(int)type}::{data.Length}]");
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
        newTable.UserDataSize = createStringTable.UserDataSize;
        newTable.Flags = createStringTable.Flags;
        newTable.UsingVarintBitCounts = createStringTable.UsingVarintBitcounts;
        
        // I genuinely have no idea why this is the case, but when processing the decalprecache table
        // the message reports that it is a fixed size of 1, which causes bit alignment and overflow
        // brute forced it with many fixed size amount, and with fixed size 2, produced the results I 
        // expected to see... so manually overriding for now... this will require more research
        if (newTable.Name == "decalprecache") newTable.UserDataSize = 2;

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
        _context.UpdateStringTableAtIndex(updateStringTable.TableId, updateStringTable.StringData.ToByteArray(), updateStringTable.NumChangedEntries);
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
            }
            else if (updateType == 0b10)
            {
                var classId = eventData.ReadBitsToUint(_context.ClassIdSize);
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

        _context.ClassIdSize = (int)Math.Log2(serverInfo.MaxClasses) + 1;
        _context.MaxPlayers = serverInfo.MaxClients;
    }
}