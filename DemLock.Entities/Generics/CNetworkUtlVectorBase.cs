using System.Text;
using System.Text.Json.Nodes;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CNetworkUtlVectorBase : DGeneric
{
    public Dictionary<int, FieldDecoder?> Data { get; set; }
    private Func<FieldDecoder> _typeFactory;

    public CNetworkUtlVectorBase(string genericTypeName, Func<FieldDecoder> typeFactory) : base(genericTypeName)
    {
        _typeFactory = typeFactory;
        
        Data = new ();
    }

    public int Size { get; set; }

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
            if (!Data.ContainsKey(i)) Data.Add(i, _typeFactory());
            return Data[i].SetValue(path[1..], ref bs);
        }

        return null;
    }
    

}