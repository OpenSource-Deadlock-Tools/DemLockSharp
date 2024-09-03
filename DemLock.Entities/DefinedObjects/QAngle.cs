using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class QAngle : DObject
{
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Roll { get; set; }
    private FieldEncodingInfo _encodingInfo;

    public QAngle(FieldEncodingInfo info)
    {
        _encodingInfo = info;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        bool hasPitch ;
        bool hasYaw ;
        bool hasRoll ;
        
        if (_encodingInfo.VarEncoder == "qangle_pitch_yaw")
        {
            Pitch = bs.ReadAngle(_encodingInfo.BitCount);
            Yaw = bs.ReadAngle(_encodingInfo.BitCount);
            Roll = 0.0f;
            return;
        }

        if (_encodingInfo.VarEncoder == "qangle_precise")
        {
            hasPitch = bs.ReadOneBit();
            hasYaw = bs.ReadOneBit();
            hasRoll = bs.ReadOneBit();
            Pitch = hasPitch ? bs.ReadCoordPrecise() : 0.0f;
            Yaw = hasYaw ? bs.ReadCoordPrecise() : 0.0f;
            Roll = hasRoll ? bs.ReadCoordPrecise() : 0.0f;
            return;
        }

        if (_encodingInfo.BitCount != 0)
        {
            Pitch = bs.ReadAngle(_encodingInfo.BitCount);
            Yaw = bs.ReadAngle(_encodingInfo.BitCount);
            Roll = bs.ReadAngle(_encodingInfo.BitCount);
            return;
        }

        hasPitch = bs.ReadOneBit();
        hasYaw = bs.ReadOneBit();
        hasRoll = bs.ReadOneBit();
        Pitch = hasPitch ? bs.ReadCoord() : 0.0f;
        Yaw = hasYaw ? bs.ReadCoord() : 0.0f;
        Roll = hasRoll ? bs.ReadCoord() : 0.0f;
    }

    /// <summary>
    /// Bad implementation of get value here, tuple is not a great return type...
    /// not sure how I want to handle more complex access like this, because most interaction
    /// should be through the fields directly
    /// </summary>
    /// <returns></returns>
    public override object GetValue() => (Pitch, Yaw, Roll);

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine($"\"@FieldType\": \"QAngle\",");
        sb.AppendLine($"\"@VarEncoder\": \"{_encodingInfo.VarEncoder}\",");
        sb.AppendLine($"\"@BitCount\": \"{_encodingInfo.BitCount}\",");
        sb.AppendLine($"\"Pitch\": \"{Pitch}\",");
        sb.AppendLine($"\"Yaw\": \"{Yaw}\",");
        sb.AppendLine($"\"Roll\": \"{Roll}\"");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public override string ToString()
    {
        return $"[QAngle : {{Pitch: {Pitch}, Yaw: {Yaw}, Roll: {Roll}}}]";
    }
}