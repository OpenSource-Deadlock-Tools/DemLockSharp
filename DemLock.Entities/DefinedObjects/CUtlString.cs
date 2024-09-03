using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CUtlString: DObject
{
    public string Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Value = bs.ReadStringUtf8();
    }

    public override object GetValue()
    {
        return Value;
    }
}