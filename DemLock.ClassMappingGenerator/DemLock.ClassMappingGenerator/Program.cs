using DemLock.ClassMappingGenerator.Symbols;

namespace DemLock.ClassMappingGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        ClassToken ct = new ClassToken();

        ct.ClassName = "Class1";
        
        ct.FieldTokens.Add(new FieldToken(){FieldName = "TestField", FieldIndex = 1});
        
        Console.WriteLine(ct.ToCSharpCode());
    }
    
}