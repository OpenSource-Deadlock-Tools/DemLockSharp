using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DInt32: DPrimitive
{
    public Int32 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadVarInt32();
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        return bs.ReadVarInt32();
    }

}