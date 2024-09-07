using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DInt16: DPrimitive
{
    public short Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException($"DInt16::SetValue(Object) is not implemented");
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return (short)bs.ReadVarInt32();
    }
    public override object ReadValue(ref BitBuffer bs)
    {
        return (short)bs.ReadVarInt32();
    }

  
}