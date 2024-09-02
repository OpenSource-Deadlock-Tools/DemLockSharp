using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVector: DGeneric
{
    public int Size { get; set; }
    public CUtlVector(string genericTypeName) : base(genericTypeName)
    {
        GenericTypeName = genericTypeName;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException($"CUtlVector::SetValue(Object) is not implemented for {GenericTypeName}");
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length == 0)
        {
            Size = (int)bs.ReadVarUInt32();
        }
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }
}