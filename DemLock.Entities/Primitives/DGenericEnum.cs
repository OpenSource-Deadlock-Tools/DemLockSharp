using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DGenericEnum: DPrimitive
{
    public UInt64 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = bs.ReadUVarInt64();
    }

    public override object GetValue() => Value;

}