using DemLock.Entities;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a field on a demo class, and is used in the
/// serializer to define how we are going to parse the entity
/// based on it's class type
/// </summary>
public class DField
{
    private DemoParserContext _context;
    public DField(DemoParserContext context)
    {
        _context = context;
    }
    
    public string? Name { get; set; }
    public DFieldType FieldType { get; set; }
    public string? SendNode { get; set; }
    public FieldEncodingInfo EncodingInfo { get; set; }
    public string SerializerName { get; set; }

    /// <summary>
    /// Get the activated field for this field template
    /// </summary>
    /// <returns></returns>
    public DObject Activate()
    {
        if(string.IsNullOrWhiteSpace(SerializerName))
            return DObject.CreateObject(FieldType.Name, EncodingInfo);
        return _context.GetSerializerByClassName(SerializerName).Instantiate();
    }
    public override string ToString()
    {
        return $"{Name}::{SerializerName}";
    }
}