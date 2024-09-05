using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CHandle: DGeneric
{
    private UInt64 _value;

    public CHandle(string genericTypeName) : base(genericTypeName)
    {
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        _value = bs.ReadVarUInt32();
        IsSet = true;
    }
    public override object GetValue() => _value;
    public override string ToString()
    {
        return $"[CBaseHandle {_value}]";
    }
}