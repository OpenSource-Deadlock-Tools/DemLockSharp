using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVectorEmbeddedNetworkVar : DGeneric
{
    public int Size { get; set; }
    public Dictionary<int, FieldDecoder?> Data { get; set; }
    private readonly FieldDecoder _childDecoder;

    public CUtlVectorEmbeddedNetworkVar(string genericTypeName) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
    }
    
    public CUtlVectorEmbeddedNetworkVar(string genericTypeName, FieldDecoder childDecoder) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
        _childDecoder = childDecoder;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new NotImplementedException("CUtlVectorEmbeddedNetworkVar should not be getting called here!");
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        throw new NotImplementedException("CUtlVectorEmbeddedNetworkVar should not be getting called here!");
    }

    public override FieldDecoder GetFieldDecoder(ReadOnlySpan<int> path)
    {
        if (path.Length == 0)
            return new DUInt32();
        return _childDecoder.GetFieldDecoder(path[1..]);
    }
}