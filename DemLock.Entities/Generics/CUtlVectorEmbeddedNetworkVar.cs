using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CUtlVectorEmbeddedNetworkVar: DGeneric
{
    public int Size { get; set; }
    public CUtlVectorEmbeddedNetworkVar(string genericTypeName) : base(genericTypeName)
    {
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
    }

    public override string ToJson()
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("{");
        sb.AppendLine("\"@GenericType\": \"" + GenericTypeName + "\",");
        sb.AppendLine("\"Size\": \"" + Size + "\"");
        sb.AppendLine("}");
        
        
        return sb.ToString();
    }
    public override object GetValue()
    {
        throw new NotImplementedException();
    }
}