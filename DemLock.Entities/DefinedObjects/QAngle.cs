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
        if (_encodingInfo.VarEncoder == "qangle_pitch_yaw")
        {
            Pitch = bs.ReadAngle(_encodingInfo.BitCount);
            Yaw = bs.ReadAngle(_encodingInfo.BitCount);
            Roll = 0.0f;
            return;
        }

        if (_encodingInfo.VarEncoder == "qangle_precise")
        {
            Pitch = bs.ReadBit() ? bs.ReadCoordPrecise() : 0.0f;
            Yaw = bs.ReadBit() ? bs.ReadCoordPrecise() : 0.0f;
            Roll = bs.ReadBit() ? bs.ReadCoordPrecise() : 0.0f;
            return;
        }

        if (_encodingInfo.BitCount != 0)
        {
            Pitch = bs.ReadAngle(_encodingInfo.BitCount);
            Yaw = bs.ReadAngle(_encodingInfo.BitCount);
            Roll = bs.ReadAngle(_encodingInfo.BitCount);
            return;
        }

        Pitch = bs.ReadBit() ? bs.ReadCoord() : 0.0f;
        Yaw = bs.ReadBit() ? bs.ReadCoord() : 0.0f;
        Roll = bs.ReadBit() ? bs.ReadCoord() : 0.0f;
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[QAngle : {{Pitch: {Pitch}, Yaw: {Yaw}, Roll: {Roll}}}]";
    }
}