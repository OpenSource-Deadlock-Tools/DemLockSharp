using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DInt8: DPrimitive
{
    public sbyte Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return (sbyte)bs.ReadVarInt32();
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        return (sbyte)bs.ReadVarInt32();
    }

    public override string ToString()
    {
        return $"[DInt8 : {Value}]";
    }
}