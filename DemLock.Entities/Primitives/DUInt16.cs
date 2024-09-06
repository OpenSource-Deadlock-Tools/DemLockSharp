using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt16: DPrimitive
{
    private UInt32 _value;
    public override void SetValue(object value)
    {
        throw new NotImplementedException($"DUInt16::SetValue(Object) is not implemented.");
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
         return bs.ReadVarUInt32();
    }

    public override string ToString()
    {
        return $"[UInt32 : {_value}]";
    }
}