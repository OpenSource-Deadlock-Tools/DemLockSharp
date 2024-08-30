﻿using System.Buffers.Binary;
using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt64 : DPrimitive
{
    public UInt64 Value { get; set; }
    private FieldEncodingInfo _encodingInfo;

    public DUInt64(FieldEncodingInfo encodingInfo)
    {
        _encodingInfo = encodingInfo;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        if (_encodingInfo.VarEncoder == "fixed64")
        {
            Value = DecodeFixed64(ref bs);
        }
        else if (_encodingInfo.VarEncoder != null)
        {
            throw new Exception($"Unknown uint64 encoder: {_encodingInfo.VarEncoder}");
        }
        else
        {
            Value = bs.ReadUVarInt64();
        }
    }

    private static ulong DecodeFixed64(ref BitBuffer buffer)
    {
        Span<byte> bytes = stackalloc byte[8];
        buffer.ReadBytes(bytes);
        return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    public override object GetValue() => Value;

}