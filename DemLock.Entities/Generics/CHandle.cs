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
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadVarUInt32();
    }
    public override string ToString()
    {
        return $"[CBaseHandle {_value}]";
    }
}