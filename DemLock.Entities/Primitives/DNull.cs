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

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new Exception("Tried to update a null value field");
    }
    public override object ReadValue(ref BitBuffer bs)
    {
        throw new Exception("Honestly not sure how you got here");
    }



}