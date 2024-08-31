using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVector: DGeneric
{
    public CUtlVector(string genericTypeName) : base(genericTypeName)
    {
    }

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
        throw new NotImplementedException();
    }
}