using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CStrongHandle: DObject
{
    public UInt64 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        Value = bs.ReadUVarInt64();
    }

    public override object GetValue() => Value;


    public override string ToString()
    {
        return $"[CStrongHandle : {Value}]";
    }
}