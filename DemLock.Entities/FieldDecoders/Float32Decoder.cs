using System.Diagnostics;
using System.Text;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.FieldDecoders;

public class Float32Decoder : FieldDecoder
{
    public override DObject DecodeField(FieldEncodingInfo? encodingInfo, ref BitBuffer buffer)
    {
        float val = -1;

        if (encodingInfo != null)
        {
            switch (encodingInfo.VarEncoder)
            {
                case "coord":
                    val = buffer.ReadCoord();
                    break;
                case "simtime":
                    return new DFloat(DecodeSimulationTime(ref buffer));
                case "runetime":
                    return new DFloat(DecodeRuneTime(ref buffer));
                case null:
                    break;
                default:
                    throw new Exception($"Unknown float encoder: {encodingInfo.VarEncoder}");
            }
        }

        if (encodingInfo.BitCount <= 0 || encodingInfo.BitCount >= 32)
            return new DFloat(buffer.ReadFloat());
        
        var encoding = QuantizedFloatEncoding.Create(encodingInfo);
        return new DFloat(encoding.Decode(ref buffer));
    }

    internal static float DecodeSimulationTime(ref BitBuffer buffer)
    {
        var ticks = new GameTick(buffer.ReadVarUInt32());
        return ticks.ToGameTime().Value;
    }

    private static float DecodeRuneTime(ref BitBuffer buffer)
    {
        var bits = buffer.ReadUInt(4);
        unsafe
        {
            return *(float*)&bits;
        }
    }
}