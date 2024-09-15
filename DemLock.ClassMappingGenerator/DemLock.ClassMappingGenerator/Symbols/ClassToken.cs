using System.Text;

namespace DemLock.ClassMappingGenerator.Symbols;

public class ClassToken: BaseToken
{
    public string? ClassName { get; set; }
    public int Version { get; set; }
    /// <summary>
    /// list of field tokens that will be generated when this class is created
    /// </summary>
    public List<FieldToken> FieldTokens { get; set; } = new List<FieldToken>();


    public string ToCSharpCode()
    {
        string classField = "";

        foreach (var v in FieldTokens)
        {
            classField += v.ToCSharpProperty;
        }
        
        return _template.Execute(new Dictionary<string, object>()
        {
            {"ClassVersion", Version},
            {"ClassName", ClassName ?? ""},
            {"ClassFields", classField ?? ""},
            {"PropertyUpdates", "PLACEHOLDER"}
        });
    }

    private static StringTemplate _template = new StringTemplate(CLASS_CODE_TEMPLATE);
    private const string CLASS_CODE_TEMPLATE = @"
public class @{ClassName}
{
    public const int VERSION = @{ClassVersion};
    public const string CLASS_NAME = ""@{ClassName}"";
    
    // FIELDS
    @{ClassFields}
    
    // UPDATE PROPERTY FUNCTION
    @{PropertyUpdates}
}
";

    private const string PUBLIC_CONST_TEMPLATE = @"public const {0} {1} = {2}";

}

public class GeneratedClass
{
    public const int VERSION = 0;
    public const string CLASS_NAME = "GeneratedClass";
    
    // FIELDS
    public int m_Field { get; set; }
    
    // UPDATE PROPERTY FUNCTION
    public void UpdateProperty(ReadOnlySpan<int> path, object value)
    {
        switch (path[0])
        {
            // This is the same I guess?
            // Array of primitives
            // List of primitives
            
            // Array/list of mapped class
            
            // Nested mapped class
             case 0:
                m_Field = (int)value;
                break;
        }
    }
}