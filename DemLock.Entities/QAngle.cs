namespace DemLock.Entities;

public class QAngle
{
    public QAngle(float pitch,float yaw, float roll)
    {
        Roll = roll;
        Yaw = yaw;
        Pitch = pitch;
    }
    public QAngle()
    { }

    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Roll { get; set; }

    public override string ToString()
    {
        return $"( {Pitch}, {Yaw}, {Roll} )";
    }
}