using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CUtlSymbolLarge: FieldDecoder
{
    public string Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadStringUtf8();
    }


}