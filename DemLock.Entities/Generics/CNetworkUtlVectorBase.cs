using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CNetworkUtlVectorBase : DGeneric
{
    private FieldDecoder _childDecoder;
    public CNetworkUtlVectorBase(string genericTypeName, FieldDecoder childDecoder) : base(genericTypeName)
    {
        _childDecoder = childDecoder;
    }
    [Obsolete]
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    [Obsolete]
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new NotImplementedException("CNetworkUtlVectorBase.SetValue was called but shouldn't be called");
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