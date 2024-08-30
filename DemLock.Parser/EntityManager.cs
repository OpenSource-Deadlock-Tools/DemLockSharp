﻿using System.Diagnostics;
using DemLock.Entities;
using DemLock.Entities.FieldDecoders;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

/// <summary>
/// Manager for entities to consolidate all of the logic needed to instantiating, and updating
/// the objects
/// </summary>
public class EntityManager
{
    private DemoParserContext _context;

    public EntityManager(DemoParserContext context)
    {
        _context = context;
    }

    public DObject CreateEntity(ref BitBuffer entityData, string className)
    {
        Span<FieldPath> fieldPaths = stackalloc FieldPath[512];
        var fp = FieldPath.Default;
        // Keep reading field paths until we reach an op with a null reader.
        // The null reader signifies `FieldPathEncodeFinish`.
        var index = 0;
        while (FieldPathEncoding.ReadFieldPathOp(ref entityData) is { Reader: { } reader })
        {
            if (index == fieldPaths.Length)
            {
                var newArray = new FieldPath[fieldPaths.Length * 2];
                fieldPaths.CopyTo(newArray);
                fieldPaths = newArray;
            }

            reader.Invoke(ref entityData, ref fp);
            fieldPaths[index++] = fp;
        }

        var s = _context.GetSerializerByClassName(className);
        var dClass = _context.GetClassByName(className);
        var dObj = dClass.Activate();
        
        
        fieldPaths = fieldPaths[..index];
        for (var idx = 0; idx < fieldPaths.Length; idx++)
        {
            var fieldPath = fieldPaths[idx];
            var pathSpan = fieldPath.AsSpan();
            return Deserialize(s, pathSpan, ref entityData);
        }

        return null;
    }
    
    // Temporary code used for outputting comparison data properly
    List<int> pathStack = new List<int>();
    private DObject Deserialize(DSerializer serializer, ReadOnlySpan<int> path, ref BitBuffer bs, int depth = 0)
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

        FieldDecoder decoder = FieldDecoder.GetDecoder(field.FieldType.Name ?? "");

        return decoder.DecodeField(field.EncodingInfo, ref bs);
        
        if (field.FieldType.Name == "CHandle")
            value = $"{bs.ReadUVarInt64()}";
        else if (field.FieldType.Name == "CNetworkedQuantizedFloat")
        {
            //var encoding = QuantizedFloatEncoding.Create(field.EncodingInfo);
            //var v = encoding.Decode(ref bs);
            //value = $"{v}";
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
                //val = DecodeFixed64(ref bs);
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
                //var x = ReadFloat(field, ref bs);
                //var y = ReadFloat(field, ref bs);
                //var z = ReadFloat(field, ref bs);
                //vec = new DVector(x, y, z);
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
}