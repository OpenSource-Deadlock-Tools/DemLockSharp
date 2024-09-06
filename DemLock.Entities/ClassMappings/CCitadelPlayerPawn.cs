namespace DemLock.Entities.ClassMappings;

public class CCitadelPlayerPawn: Entity
{
    public float SimulationTime { get; set; }

    public override void UpdateProperty(ReadOnlySpan<int> path, object value)
    {
        switch (path[0])
        {
            case 0:
                SimulationTime = value as float? ?? 0;
                break;
        }
    }
}