using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVector: DGeneric
{
    private readonly FieldDecoder _childDecoder;
    public Dictionary<int, FieldDecoder?> Data { get; set; }
    public CUtlVector(string genericTypeName,FieldDecoder childDecoder) : base(genericTypeName)
    {
        _childDecoder = childDecoder;
    }
    public override void SetValue(object value)
    {
        throw new NotImplementedException($"CUtlVector::SetValue(Object) is not implemented for {GenericTypeName}");
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new NotImplementedException($"CUtlVector::SetValue(ReadOnlySpan<int>) is not implemented for {GenericTypeName}");

    }
    public override object ReadValue(ref BitBuffer bs)
    {
        throw new NotImplementedException("CUtlVector should not be getting called here!");
    }

    public override FieldDecoder GetFieldDecoder(ReadOnlySpan<int> path)
    {
        
        if (path.Length == 0)
            return new DUInt32();

        return _childDecoder.GetFieldDecoder(path[1..]);
    }
}