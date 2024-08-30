using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DemLock.Entities;
using DemLock.Parser.Models;
using DemLock.Utils;
using Google.Protobuf.Reflection;
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
        newTable.UserDataSize = createStringTable.UserDataSizeBits;
        newTable.Flags = createStringTable.Flags;
        newTable.UsingVarintBitCounts = createStringTable.UsingVarintBitcounts;

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
        _context.UpdateStringTableAtIndex(updateStringTable.TableId, updateStringTable.StringData.ToByteArray(),
            updateStringTable.NumChangedEntries);
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

                var baseline = _context.GetInstanceBaseline((int)classId);
                
                var entityBuffer = new BitBuffer(baseline);
                
                
                var v = _context.EntityManager.CreateEntity(ref entityBuffer, serverClass.ClassName);
                

                Environment.Exit(0);
            }
        }
    }

    private SendNodeDecoder<object> CreateDecoder(DSerializer serializer)
    {
        return (object instance, ReadOnlySpan<int> path, ref BitBuffer buffer) =>
        {
            //Deserialize(serializer, path, ref buffer);
            //pathStack.Clear();
        };
    }



    private float ReadFloat(DField field, ref BitBuffer bs)
    {
        float val = -1;

        if (field.EncodingInfo.VarEncoder != null)
        {
            switch (field.EncodingInfo.VarEncoder)
            {
                case "coord":
                    return bs.ReadCoord();
                case "simtime":
                    //return DecodeSimulationTime(ref bs);
                case "runetime":
                    //return DecodeRuneTime(ref bs);
                case null:
                    break;
                default:
                    throw new Exception($"Unknown float encoder: {field.EncodingInfo.VarEncoder}");
            }
        }
        else
        {
            if (field.EncodingInfo.BitCount <= 0 || field.EncodingInfo.BitCount >= 32)
            {
                Debug.Assert(field.EncodingInfo.BitCount <= 32);
                //return DecodeFloatNoscale(ref bs);
            }
            else
            {
                //var encoding = QuantizedFloatEncoding.Create(field.EncodingInfo);
                //return encoding.Decode(ref bs);
            }
        }

        return -1;
    }





    private static void ReadNewEntity(ref BitBuffer entityBitBuffer, CEntityInstance entity)
    {
        Span<FieldPath> fieldPaths = stackalloc FieldPath[512];
        var fp = FieldPath.Default;
        // Keep reading field paths until we reach an op with a null reader.
        // The null reader signifies `FieldPathEncodeFinish`.
        var index = 0;
        while (FieldPathEncoding.ReadFieldPathOp(ref entityBitBuffer) is { Reader: { } reader })
        {
            if (index == fieldPaths.Length)
            {
                var newArray = new FieldPath[fieldPaths.Length * 2];
                fieldPaths.CopyTo(newArray);
                fieldPaths = newArray;
            }

            reader.Invoke(ref entityBitBuffer, ref fp);
            fieldPaths[index++] = fp;
        }

        fieldPaths = fieldPaths[..index];

        Console.WriteLine($"Bits Remaining: {entityBitBuffer.BitsRemaining}");
        for (var idx = 0; idx < fieldPaths.Length; idx++)
        {
            var fieldPath = fieldPaths[idx];
            var pathSpan = fieldPath.AsSpan();
            entity.ReadField(pathSpan, ref entityBitBuffer);
        }
    }


    private void ProcessServerInfo(byte[] data)
    {
        CSVCMsg_ServerInfo serverInfo = CSVCMsg_ServerInfo.Parser.ParseFrom(data);

        _context.ClassIdSize = (int)Math.Log2(serverInfo.MaxClasses) + 1;
        _context.MaxPlayers = serverInfo.MaxClients;
    }
}

internal delegate void SendNodeDecoder<in T>(
    T instance,
    ReadOnlySpan<int> fieldPath,
    ref BitBuffer buffer);

public class CEntityInstance
{
    private readonly SendNodeDecoder<object> _decoder;
    protected readonly DemoParser Demo;

    internal CEntityInstance(SendNodeDecoder<object> decoder, DClass serverClass, uint serialNumber)
    {
        _decoder = decoder;
        //Demo = context.Demo;
        ServerClass = serverClass;
        SerialNumber = serialNumber;
    }

    public CEntityIndex EntityIndex { get; }

    public CHandle<CEntityInstance> EntityHandle =>
        CHandle<CEntityInstance>.FromIndexSerialNum(EntityIndex, SerialNumber);

    /// <summary>
    /// Is this entity within the recording player's PVS?
    /// For GOTV demos, this is always <c>true</c>
    /// </summary>
    public bool IsActive { get; internal set; }

    public DClass ServerClass { get; }
    public uint SerialNumber { get; }

    internal void ReadField(ReadOnlySpan<int> fieldPath, ref BitBuffer buffer)
    {
        _decoder(this, fieldPath, ref buffer);
    }
}

public readonly record struct CEntityIndex(uint Value)
{
    public static readonly CEntityIndex Invalid = new(unchecked((uint)-1));
    public bool IsValid => this != Invalid;

    public override string ToString() => IsValid ? $"Entity #{Value}" : "<invalid>";
}

public readonly record struct CGameSceneNodeHandle(uint Value);

public readonly record struct CHandle<T>(ulong Value)
    where T : CEntityInstance
{
    internal const int MaxEdictBits = 14;
    public override string ToString() => IsValid ? $"Index = {Index.Value}, Serial = {SerialNum}" : "<invalid>";

    public bool IsValid => this != default && Index.Value != (MaxEdictBits - 1);

    public CEntityIndex Index => new((uint)(Value & (MaxEdictBits - 1)));
    public uint SerialNum => (uint)(Value >> MaxEdictBits);

    public static CHandle<T> FromIndexSerialNum(CEntityIndex index, uint serialNum) =>
        new(((ulong)index.Value) | (serialNum << MaxEdictBits));

    public static CHandle<T> FromEventStrictEHandle(uint value)
    {
        // EHandles in events are serialised differently than networked handles.
        //
        // Empirically the bit structure appears to be:
        //   1100100 0011110000 0 00001011101011
        //   ^^^^^^^ ^^^^^^^^^^ ^ ^^^^^^^^^^^^^^
        //   |       |          | \__ ent index
        //   |       |          \__ always zero?
        //   |       \__ serial number
        //   \__ unknown, varies

        Debug.Assert(value == uint.MaxValue || (value & (1 << 14)) == 0);

        var index = value & (MaxEdictBits - 1);
        var serialNum = (value >> 15) & ((1 << 10) - 1);

        return FromIndexSerialNum(new CEntityIndex(index), serialNum);
    }

    // public T? Get(DemoParser demo) => demo.GetEntityByHandle(this);

    // public TEntity? Get<TEntity>(DemoParser demo) where TEntity : T => demo.GetEntityByHandle(this) as TEntity;
}

public readonly record struct CUtlStringToken(uint Value);

public readonly record struct CStrongHandle(ulong Value);

public readonly struct DVector
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public DVector(float _x, float _y, float _z)
    {
        X = _x;
        Y = _y;
        Z = _z;
    }

    public DVector((float x, float y, float z) tuple)
    {
        X = tuple.x;
        Y = tuple.y;
        Z = tuple.z;
    }

    public override string ToString()
    {
        return $"{{ X = {X}, Y = {Y}, Z = {Z} }}";
    }
}