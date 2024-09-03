using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVectorEmbeddedNetworkVar : DGeneric
{
    public int Size { get; set; }
    public Dictionary<int, DObject?> Data { get; set; }
    private readonly Func<DObject> _typeFactory;

    public CUtlVectorEmbeddedNetworkVar(string genericTypeName) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
        _typeFactory = () => new DNull();
    }
    
    public CUtlVectorEmbeddedNetworkVar(string genericTypeName, Func<DObject> typeFactory) : base(genericTypeName)
    {
        Data = new ();
        GenericTypeName = genericTypeName;
        _typeFactory = typeFactory;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length == 0)
        {
            Size = (int)bs.ReadVarUInt32();
        }

        if (path.Length >= 1)
        {
            var i = path[0];
            if(!Data.ContainsKey(i) ) Data.Add(i, _typeFactory());
            Data[i].SetValue(path[1..], ref bs);
        }
    }

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine("\"@GenericType\": \"" + GenericTypeName + "\",");
        sb.AppendLine($"\"@Size\": \"{Size}\"");
        sb.AppendLine(",\"Values\": [");
        if (Data.Count > 0)
        {
            List<string> il = new List<string>();
            foreach (var kv in Data)
            {
                if (kv.Value != null)
                {
                    il.Add(kv.Value.ToJson());
                }
            }

            sb.AppendLine(string.Join(",", il));
        }

        sb.Append("]");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public override JsonNode ToJsonNode()
    {
        JsonArray arr = new JsonArray();
        if (Data.Count > 0)
        {
            foreach (var d in Data)
            {
                arr.Add(d.Value?.ToJsonNode());
            }
        }
        return arr;
    }

    public override object GetValue()
    {
        return Data;
    }
}