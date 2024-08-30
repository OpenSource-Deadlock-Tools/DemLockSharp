using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CUtlStringToken: DObject
{
    public UInt32 Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = bs.ReadVarUInt32();
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[CUtlStringToken : {Value}]";
    }
}