using DemLock.Entities;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a field on a demo class, and is used in the
/// serializer to define how we are going to parse the entity
/// based on it's class type
/// </summary>
public class DField
{
    
    public string? Name { get; set; }
    public DFieldType FieldType { get; set; }
    public string? SendNode { get; set; }
    public FieldEncodingInfo EncodingInfo { get; set; }
    public string SerializerName { get; set; }

    public override string ToString()
    {
        return $"{Name}::{SerializerName}";
    }
}