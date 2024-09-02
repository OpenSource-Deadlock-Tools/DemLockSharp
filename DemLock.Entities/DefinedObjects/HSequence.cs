using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class HSequence: DObject
{
    public ulong Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException("HSequence::SetValue(Object)");
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = bs.ReadUVarInt64() - 1;
    }

    public override object GetValue()
    {
        return Value;
    }
}