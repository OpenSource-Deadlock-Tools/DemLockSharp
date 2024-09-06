using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVectorEmbeddedNetworkVar : DGeneric
{
    public int Size { get; set; }
    public Dictionary<int, FieldDecoder?> Data { get; set; }
    private readonly Func<FieldDecoder> _typeFactory;

    public CUtlVectorEmbeddedNetworkVar(string genericTypeName) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
        _typeFactory = () => new DNull();
    }
    
    public CUtlVectorEmbeddedNetworkVar(string genericTypeName, Func<FieldDecoder> typeFactory) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
        _typeFactory = typeFactory;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length == 0)
        {
            return (int)bs.ReadVarUInt32();
        }

        if (path.Length >= 1)
        {
            var i = path[0];
            if(!Data.ContainsKey(i) ) Data.Add(i, _typeFactory());
            return Data[i].SetValue(path[1..], ref bs);
        }

        return null;
    }
}