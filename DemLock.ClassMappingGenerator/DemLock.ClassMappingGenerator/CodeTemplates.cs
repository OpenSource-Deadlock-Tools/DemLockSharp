using System.Text;

namespace DemLock.ClassMappingGenerator;

public class CodeTemplates
{
    public static string WithNamespace(string body, string @namespace)
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine($@"using System;");
        sb.AppendLine($"namespace {@namespace}");
        sb.AppendLine("{");
        sb.AppendLine(body);
        sb.AppendLine("}");
        
        return sb.ToString();

        
    }
    
}