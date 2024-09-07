using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CStrongHandle: DGeneric
{

    public UInt64 Value { get; set; }
    
    public CStrongHandle(string genericTypeName) : base(genericTypeName)
    { }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        return bs.ReadUVarInt64();
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        return bs.ReadUVarInt64();
    }

    public override string ToString()
    {
        return $"[CStrongHandle : {Value}]";
    }
}