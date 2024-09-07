namespace DemLock.Entities.ClassMappings;

public class CBodyComponent: Entity
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public QAngle EyeAngles { get; set; }

    public override void UpdateProperty(ReadOnlySpan<int> path, object value)
    {
        switch (path[0])
        {
            case 7:
                EyeAngles = value as QAngle ?? new QAngle();
                break;
        }
    }
}