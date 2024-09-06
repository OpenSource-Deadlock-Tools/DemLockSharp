using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt8: DPrimitive
{
    public byte Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        return (byte)bs.ReadVarUInt32();
    }

}