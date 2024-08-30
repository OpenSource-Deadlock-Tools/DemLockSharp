using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DInt8: DPrimitive
{
    public sbyte Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        Value = (sbyte)bs.ReadVarInt32();
    }

    public override object GetValue() => Value;


    public override string ToString()
    {
        return $"[DInt8 : {Value}]";
    }
}