using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt8: DPrimitive
{
    public byte Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        Value = (byte)bs.ReadVarUInt32();
    }

    public override object GetValue() => Value;

}