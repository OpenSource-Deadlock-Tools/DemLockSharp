using DemLock.Utils;

namespace DemLock.Entities.FieldDecoders;

public static class FloatDecoder
{
    public static float ReadFloat(ref BitBuffer bits, FieldEncodingInfo encodingInfo)
    {
        if (encodingInfo != null)
        {
            switch (encodingInfo.VarEncoder)
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
                    throw new Exception($"Unknown float encoder: {encodingInfo.VarEncoder}");
            }
        }

        if (encodingInfo.BitCount <= 0 || encodingInfo.BitCount >= 32)
            return bits.ReadFloat();

        var encoding = QuantizedFloatEncoding.Create(encodingInfo);
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
    private static float DecodeSimulationTime(ref BitBuffer buffer)
    {
        // Assume a 64 tick server... this will need to be set somehow
        var ticks = buffer.ReadVarUInt32();
        return ticks / 64.0f;
    }
}