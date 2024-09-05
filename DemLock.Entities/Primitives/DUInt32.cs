using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt32: DPrimitive
{
    public UInt32 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadVarUInt32();
    }

    public override object GetValue() => Value;

}