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
    public int SerializerVersion { get; set; }

    private static int tmp = 0;
    /// <summary>
    /// Get the activated field for this field template
    /// </summary>
    /// <returns></returns>
    public DObject Activate()
    {

        if (FieldType.Count > 0)
            return DObject.CreateFixedSizeArray(FieldType.Name, FieldType.Count, () => DObject.CreateObject(FieldType.Name, EncodingInfo));
            
        
        // If this has a generic, we will need to handle special cases
        if(FieldType.GenericType != null)
            return DObject.CreateGenericObject(FieldType.Name, FieldType.GenericType.Name);
        // Generics will come with the serializer set to the serializer for their generic type
        // This will need to be packed and sen to the generic at some point to get generics working fully
        // For now we will just check that it's a generic to get through initial processing and come back to it later
        
        // If the serializer is named this is a nested entity we need to activate
        if (!string.IsNullOrWhiteSpace(SerializerName))
            return _context.GetSerializerByClassName(SerializerName, SerializerVersion).Instantiate();
        
            
        return DObject.CreateObject(FieldType.Name, EncodingInfo);
    }
    public override string ToString()
    {
        return $"{Name}::{SerializerName}";
    }
}