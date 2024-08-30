using DemLock.Utils;

namespace DemLock.Entities.Primitives;

/// <summary>
/// Represents the null primitive
/// </summary>
public class DNull: DPrimitive
{
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new NotImplementedException();
    }

    public override object GetValue()
    {
        return null;
    }
}