using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CHandle: DObject
{
    private UInt64 _value;
    
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        _value = bs.ReadUVarInt64();
        IsSet = true;
    }
    public override object GetValue() => _value;
    public override string ToString()
    {
        return $"[CBaseHandle {_value}]";
    }
}