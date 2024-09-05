using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DBool: DPrimitive
{
    public bool Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        Value = bs.ReadBit();
    }

    public override object GetValue() => Value;

}