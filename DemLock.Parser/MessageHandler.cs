using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DemLock.Parser.Fields;
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
            //Console.WriteLine($"\t{type}({(int)type}) [{data.Length}]");
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

                bool isDone = false;

                var baseline = _context.GetInstanceBaseline((int)classId);
                Console.WriteLine(string.Join("_",baseline[..64].Select(x => x.ToString())));
                var v = new BitBuffer(baseline);
                var v2 = new BitBuffer(baseline);
                Console.WriteLine("Baseline Buffer dump");
                Console.WriteLine("Discarded Bits");
                Console.Write($"{v2.ReadUInt(32):B32}_");
                Console.Write($"{v2.ReadUInt(32):B32}_");
                Console.Write($"{v2.ReadUInt(32):B32}_");
                Console.Write($"{v2.ReadUInt(32):B32}_");
                Console.Write($"{v2.ReadUInt(12):B12}");
                Console.WriteLine("\n=======================\n==================\n");
                Console.WriteLine($"Bits Remaining: " + v2.BitsRemaining);
                for (int j = 0; j < 32; ++j)
                {
                    Console.WriteLine($"{v2.ReadByte():B8}");
                }Console.WriteLine($"===========================");
                
                var serializer = _context.GetSerializerByClassName(serverClass.ClassName);

                ReadNewEntity(ref v,
                    new CEntityInstance(CreateDecoder(serializer),
                        serverClass, serialNum));

                Environment.Exit(0);
            }
        }
    }

    private SendNodeDecoder<object> CreateDecoder(DSerializer serializer)
    {
        return (object instance, ReadOnlySpan<int> path, ref BitBuffer buffer) =>
        {
            Deserialize(serializer, path, ref buffer);
            pathStack.Clear();
        };
    }

    // Temporary code used for outputting comparison data properly
    List<int> pathStack = new List<int>();
    private object Deserialize(DSerializer serializer, ReadOnlySpan<int> path, ref BitBuffer bs, int depth = 0)
    {
        pathStack.Add(path[0]);
        string value = "";
        if (path.Length == 0) return null;
        // for (int i = 0; i < depth; i++) Console.Write("\t");
        var field = serializer.Fields[path[0]];
        if (!string.IsNullOrWhiteSpace(field.SerializerName))
            if (path.Length <= 1)
            {
                Console.WriteLine($"{field.Name} (IsSet: {bs.ReadBit()})");
                return null;
            }
            else
                return Deserialize(_context.GetSerializerByClassName(field.SerializerName), path[1..], ref bs, depth + 1);

        if (field.FieldType.Name == "CHandle")
            value = $"{bs.ReadUVarInt64()}";
        else if (field.FieldType.Name == "CNetworkedQuantizedFloat")
        {
            var encoding = QuantizedFloatEncoding.Create(field.EncodingInfo);
            var v = encoding.Decode(ref bs);
            value = $"{v}";
        }
        else if (field.FieldType.Name == "uint16")
        {
            value =$"{(ushort)bs.ReadVarUInt32()}";
        }
        else if (field.FieldType.Name == "CGameSceneNodeHandle")
        {
            value = $"{(ushort)bs.ReadVarUInt32()}";
        }
        else if (field.FieldType.Name == "QAngle")
        {
            QAngle ang;
            if (field.EncodingInfo.VarEncoder == "qangle_pitch_yaw")
            {
                ang = new QAngle(
                    bs.ReadAngle(field.EncodingInfo.BitCount),
                    bs.ReadAngle(field.EncodingInfo.BitCount),
                    0.0f);
            }
            else if (field.EncodingInfo.VarEncoder == "qangle_precise")
            {
                var hasPitch = bs.ReadBit();
                var hasYaw = bs.ReadBit();
                var hasRoll = bs.ReadBit();
                ang = new QAngle(
                    hasPitch ? bs.ReadCoordPrecise() : 0.0f,
                    hasYaw ? bs.ReadCoordPrecise() : 0.0f,
                    hasRoll ? bs.ReadCoordPrecise() : 0.0f);
            }
            else
            {
                if (field.EncodingInfo.BitCount != 0)
                {
                    ang = new QAngle(
                        bs.ReadAngle(field.EncodingInfo.BitCount),
                        bs.ReadAngle(field.EncodingInfo.BitCount),
                        bs.ReadAngle(field.EncodingInfo.BitCount));
                }

                var hasPitch = bs.ReadBit();
                var hasYaw = bs.ReadBit();
                var hasRoll = bs.ReadBit();
                ang = new QAngle(
                    hasPitch ? bs.ReadCoord() : 0.0f,
                    hasYaw ? bs.ReadCoord() : 0.0f,
                    hasRoll ? bs.ReadCoord() : 0.0f);
            }

            value = $"{ang.Pitch}::{ang.Roll}::{ang.Yaw}";
        }
        else if (field.FieldType.Name == "float32")
        {
            float val = -1;

            if (field.EncodingInfo.VarEncoder != null)
            {
                switch (field.EncodingInfo.VarEncoder)
                {
                    case "coord":
                        val = bs.ReadCoord();
                        break;
                    case "simtime":
                        val = DecodeSimulationTime(ref bs);
                        break;
                    case "runetime":
                        val = DecodeRuneTime(ref bs);
                        break;
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
                    val = DecodeFloatNoscale(ref bs);
                }
                else
                {
                    var encoding = QuantizedFloatEncoding.Create(field.EncodingInfo);
                    val = encoding.Decode(ref bs);
                }
            }

            value = $"{val}";
        }
        else if (field.FieldType.Name == "CUtlStringToken")
        {
            var v = new CUtlStringToken(bs.ReadVarUInt32());
            value = $"{v.Value}";
        }
        else if (field.FieldType.Name == "CStrongHandle")
        {
            var v = new CStrongHandle(bs.ReadUVarInt64());
            value = $"{v.Value}";
        }
        else if (field.FieldType.Name == "bool")
        {
            value = $"{bs.ReadBit()}";
        }
        else if (field.FieldType.Name == "uint64")
        {
            UInt64 val = 0;
            if (field.EncodingInfo.VarEncoder == "fixed64")
            {
                val = DecodeFixed64(ref bs);
            }
            else if (field.EncodingInfo.VarEncoder != null)
            {
                throw new Exception($"Unknown uint64 encoder: {field.EncodingInfo.VarEncoder}");
            }
            else
            {
                val = bs.ReadUVarInt64();
            }

            value = $"{val}";
        }
        else if (field.FieldType.Name == "int8")
        {
            value = $"{(byte)bs.ReadVarInt32()}";
        }
        else if (field.FieldType.Name == "uint8")
        {
            value = $"{(sbyte)bs.ReadVarUInt32()}";
        }
        else if (field.FieldType.Name == "uint32")
        {
            value = $"{bs.ReadVarUInt32()}";
        }
        else if (field.FieldType.Name == "uint16")
        {
            value = $"{bs.ReadVarUInt32()}";
        }
        else if (field.FieldType.Name == "int32")
        {
            value = $"{bs.ReadVarInt32()}";
        }
        else if (field.FieldType.Name == "Vector")
        {
            DVector vec = new DVector(-1, 1, -1);
            if (field.EncodingInfo.VarEncoder == "normal")
            {
                vec = new DVector(bs.Read3BitNormal());
            }
            else
            {
                var x = ReadFloat(field, ref bs);
                var y = ReadFloat(field, ref bs);
                var z = ReadFloat(field, ref bs);
                vec = new DVector(x, y, z);
            }

            value = $"{vec}";
        }
        // How the hell will I handle enums
        else if (new string[] { "EntityPlatformTypes_t", "MoveCollide_t", "MoveType_t" }.Contains(field.FieldType.Name))
        {
            value = $"{bs.ReadUVarInt64()}";
        }
        else
        {
            value = $"{field.FieldType}";
        }

        Console.WriteLine($"[{string.Join("_", pathStack)}]{field.Name}::{field.FieldType}={value}");
        return null;
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
                    return DecodeSimulationTime(ref bs);
                case "runetime":
                    return DecodeRuneTime(ref bs);
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
                return DecodeFloatNoscale(ref bs);
            }
            else
            {
                var encoding = QuantizedFloatEncoding.Create(field.EncodingInfo);
                return encoding.Decode(ref bs);
            }
        }

        return -1;
    }

    private static ulong DecodeFixed64(ref BitBuffer buffer)
    {
        Span<byte> bytes = stackalloc byte[8];
        buffer.ReadBytes(bytes);
        return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    private static float DecodeRuneTime(ref BitBuffer buffer)
    {
        var bits = buffer.ReadUInt(4);
        unsafe
        {
            return *(float*)&bits;
        }
    }

    internal static float DecodeFloatNoscale(ref BitBuffer buffer) => buffer.ReadFloat();

    internal static float DecodeSimulationTime(ref BitBuffer buffer)
    {
        var ticks = new GameTick(buffer.ReadVarUInt32());
        return ticks.ToGameTime().Value;
    }

    private int ExtractInt(DField field)
    {
        return 0;
    }

    private static void ReadNewEntity(ref BitBuffer entityBitBuffer, CEntityInstance entity)
    {
        Console.WriteLine($"Bits Remaining: {entityBitBuffer.BitsRemaining}");
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

public readonly record struct QAngle(float Pitch, float Yaw, float Roll);

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