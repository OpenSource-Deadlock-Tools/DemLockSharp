using DemLock.Utils;

namespace DemLock.Entities.Primitives;

public class DString: DPrimitive
{
    public string Value { get; set; }
    private int _maxLength;
    public DString()
    {
        _maxLength = 4096;

    }
    public DString(int maxLength)
    {
        _maxLength = maxLength;
    }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        // Not taking into account max length or anything yet, since it should be fine but will want to later for safety
        return bs.ReadStringUtf8();
    }
}