using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.Generics;

public class CNetworkUtlVectorBase: DGeneric
{
    public CNetworkUtlVectorBase(string genericTypeName) : base(genericTypeName)
    { }
    public int Size { get; set; }
    
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
        return Size;
    }
}