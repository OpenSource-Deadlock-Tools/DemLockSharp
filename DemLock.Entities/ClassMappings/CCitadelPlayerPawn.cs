namespace DemLock.Entities.ClassMappings;

public class CCitadelPlayerPawn: Entity
{
    public float SimulationTime { get; set; }
    
    public CBodyComponent CBodyComponent { get; set; }

    public override void UpdateProperty(ReadOnlySpan<int> path, object value)
    {
        switch (path[0])
        {
            case 0:
                SimulationTime = value as float? ?? 0;
                break;
            case 14:
                if (path.Length == 1)
                    CBodyComponent = new CBodyComponent();
                else
                    CBodyComponent.UpdateProperty(path[1..], value);
                break;
            
        }
    }
}