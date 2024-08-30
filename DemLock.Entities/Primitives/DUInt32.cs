using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt32: DPrimitive
{
    public UInt32 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = bs.ReadVarUInt32();
    }

    public override object GetValue() => Value;

}