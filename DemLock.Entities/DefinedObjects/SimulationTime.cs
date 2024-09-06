using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class SimulationTime: FieldDecoder
{
    public float Value { get; set; }
    private float _tickInterval;

    public SimulationTime(float tickInterval)
    {
        _tickInterval = tickInterval;
    }
    
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        var ticks = bs.ReadVarUInt32();
        return  ticks * _tickInterval;
    }
    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(Value);
    
    public override string ToString() => ToTimeSpan().ToString();
}