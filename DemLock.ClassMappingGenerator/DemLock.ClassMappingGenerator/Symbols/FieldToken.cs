namespace DemLock.ClassMappingGenerator.Symbols;

public class FieldToken: BaseToken
{
    public string FieldName { get; set; } = "";
    public int FieldIndex { get; set; }
    
    public string ToCSharpProperty => $"public 'FIELD_TYPE' {FieldName} {{ get; set; }}";

    public string ToCSharpMappingCase => 
        $@"
case {FieldIndex}:
    this.{FieldName} = ('FIELD_TYPE')value;
    break;
";



}