using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DGenericEnum: DPrimitive
{
    public UInt64 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadUVarInt64();
    }

}