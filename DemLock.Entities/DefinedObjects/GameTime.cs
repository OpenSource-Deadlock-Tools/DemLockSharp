using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class GameTime: DObject
{
    public float Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        Value = bs.ReadFloat();
    }
    public override object GetValue() => Value;
    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(Value);
    public override string ToString() => ToTimeSpan().ToString();
}