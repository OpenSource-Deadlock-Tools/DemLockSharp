using DemLock.Utils;

namespace DemLock.Entities.Primitives;

/// <summary>
/// Represents a primitive piece of data
/// </summary>
public abstract class DPrimitive: DObject
{
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs, ref UpdateDelta returnDelta)
    {
        SetValue(path, ref bs);
        returnDelta.Value = GetValue();
    }
}