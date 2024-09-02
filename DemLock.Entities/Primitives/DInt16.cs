using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DInt16: DPrimitive
{
    public short Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException($"DInt16::SetValue(Object) is not implemented");
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = (short)bs.ReadVarInt32();
    }

    public override object GetValue()
    {
        throw new NotImplementedException($"DInt16::GetValue() is not implemented");
    }
}