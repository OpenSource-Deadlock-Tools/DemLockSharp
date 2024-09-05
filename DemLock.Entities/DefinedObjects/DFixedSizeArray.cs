using System.Text;
using System.Text.Json.Nodes;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class DFixedSizeArray:FieldDecoder
{
    public string TypeName { get; set; }
    public int Length { get; set; }
    private Func<FieldDecoder> _objectFactory;
    /// <summary>
    /// Right now this is a byte array but this is very very incorrect and I just need to get the baseline parsed and I can
    /// then properly address this issue
    /// </summary>
    public FieldDecoder[] Data { get; set; }
    public DFixedSizeArray(string typeName, int length, Func<FieldDecoder> objectFactory)
    {
        _objectFactory = objectFactory;
        TypeName = typeName;
        Length = length;
        Data = new FieldDecoder[length];
    }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length == 0) return bs.ReadBit();

        if (path.Length >= 1)
        {
            if(Data[path[0]] == null) Data[path[0]] = _objectFactory();
            return Data[path[0]].SetValue(path[1..], ref bs);
        }

        return null;
    }

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine($"\"@TypeName\": \"{TypeName}\",");
        sb.AppendLine($"\"Length\": \"{Length}\"");
        sb.AppendLine(",\"Values\": [");
        for (int i = 0; i < Data.Length; i++)
        {
            if (Data[i] != null)
            {
                sb.AppendLine(Data[i].ToJson());
                if(i < Data.Length - 1) sb.AppendLine(",");
                else sb.AppendLine();
            }
        }

        sb.Append("]");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public override JsonNode ToJsonNode()
    {
        JsonArray arr = new JsonArray();
        if (Data.Length > 0)
        {
            foreach (var d in Data)
            {
                arr.Add(d?.ToJsonNode());
            }
        }
        return arr;
    }

    public override object GetValue()
    {
        return Data;
    }
}