using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DUInt16: DPrimitive
{
    private UInt32 _value;
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
         _value = bs.ReadVarUInt32();
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[UInt32 : {_value}]";
    }
}