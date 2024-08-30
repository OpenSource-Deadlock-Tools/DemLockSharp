﻿using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class Vector: DObject
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    private FieldEncodingInfo _encodingInfo;
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public Vector(FieldEncodingInfo encodingInfo)
    {
        _encodingInfo = encodingInfo;
    }
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
            if (_encodingInfo.VarEncoder == "normal")
            {
                (X,Y,Z) = (bs.Read3BitNormal());
                return;
            }
                X = ReadFloat( ref bs);
                Y = ReadFloat( ref bs);
                Z = ReadFloat( ref bs);

    }

    private float ReadFloat(ref BitBuffer bits)
    {
        
        if(_encodingInfo != null)
        {
            switch (_encodingInfo.VarEncoder)
            {
                case "coord":
                    return bits.ReadCoord();
                case "simtime":
                    return DecodeSimulationTime(ref bits);
                case "runetime":
                    return DecodeRuneTime(ref bits);
                case null:
                    break;
                default:
                    throw new Exception($"Unknown float encoder: {_encodingInfo.VarEncoder}");
            }
        }

        if (_encodingInfo.BitCount <= 0 || _encodingInfo.BitCount >= 32)
            return bits.ReadFloat();
        
        var encoding = QuantizedFloatEncoding.Create(_encodingInfo);
        return encoding.Decode(ref bits);
    }

    private static float DecodeRuneTime(ref BitBuffer buffer)
    {
        var bits = buffer.ReadUInt(4);
        unsafe
        {
            return *(float*)&bits;
        }
    }
 internal static float DecodeSimulationTime(ref BitBuffer buffer)
    {
        // Assume a 64 tick server... this will need to be set somehow
        var ticks = buffer.ReadVarUInt32();
        return ticks / 64.0f;
    }
    public override object GetValue() => (X, Y, Z);
    

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine($"\"X\": \"{X}\",");
        sb.AppendLine($"\"Y\": \"{Y}\",");
        sb.AppendLine($"\"Z\": \"{Z}\"");
        sb.AppendLine("}");

        return sb.ToString();
    }

}