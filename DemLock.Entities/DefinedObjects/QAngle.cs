using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class QAngle : FieldDecoder
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

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
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
            return (pitch: Pitch, yaw: Yaw, roll: Roll);
        }

        if (_encodingInfo.VarEncoder == "qangle_precise")
        {
            hasPitch = bs.ReadOneBit();
            hasYaw = bs.ReadOneBit();
            hasRoll = bs.ReadOneBit();
            Pitch = hasPitch ? bs.ReadCoordPrecise() : 0.0f;
            Yaw = hasYaw ? bs.ReadCoordPrecise() : 0.0f;
            Roll = hasRoll ? bs.ReadCoordPrecise() : 0.0f;
            return (pitch: Pitch, yaw: Yaw, roll: Roll);
        }

        if (_encodingInfo.BitCount != 0)
        {
            Pitch = bs.ReadAngle(_encodingInfo.BitCount);
            Yaw = bs.ReadAngle(_encodingInfo.BitCount);
            Roll = bs.ReadAngle(_encodingInfo.BitCount);
            return (pitch: Pitch, yaw: Yaw, roll: Roll);
        }

        hasPitch = bs.ReadOneBit();
        hasYaw = bs.ReadOneBit();
        hasRoll = bs.ReadOneBit();
        Pitch = hasPitch ? bs.ReadCoord() : 0.0f;
        Yaw = hasYaw ? bs.ReadCoord() : 0.0f;
        Roll = hasRoll ? bs.ReadCoord() : 0.0f;
        return (pitch: Pitch, yaw: Yaw, roll: Roll);
    }

    public override string ToString()
    {
        return $"[QAngle : {{Pitch: {Pitch}, Yaw: {Yaw}, Roll: {Roll}}}]";
    }
}