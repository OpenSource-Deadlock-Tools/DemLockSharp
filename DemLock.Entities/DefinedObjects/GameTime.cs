using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class GameTime: FieldDecoder
{
    public float Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        return bs.ReadFloat();
    }
    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(Value);
    public override string ToString() => ToTimeSpan().ToString();
}