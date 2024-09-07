using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace DemLock.Parser.Models;

/// <summary>
/// This is the field type containing information that will be used
/// when constructing serializers, it is sent as part of the 
/// </summary>
public class DFieldType
{
    private static readonly ConcurrentDictionary<string, DFieldType> _fieldTypeCache = new();
    
    public string? Name { get; set; }
    public DFieldType? GenericType { get; set; }
    public bool IsPointer { get; set; }
    public int Count { get; set; }


    
    public static DFieldType Parse(string typeName)
    {
        if (_fieldTypeCache.TryGetValue(typeName, out var fieldType))
            return fieldType;

        fieldType = ParseInternal(typeName);
        _fieldTypeCache[typeName] = fieldType;
        return fieldType;
    }
    
    private static DFieldType ParseInternal(string typeName) 
    {
        Regex regex = new Regex(@"^(?<name>[^\<\[\*]+)(\<\s(?<generic>.*)\s\>)?(?<ptr>\*)?(\[(?<count>.*)\])?$");
        
        var match = regex.Match(typeName);
        if (!match.Success)
            throw new Exception($"Invalid field type: {typeName}");

        var name = match.Groups["name"].Value;
        var genericParam = match.Groups["generic"] is { Success: true, Value: var genericName }
            ? Parse(genericName)
            : null;
        
        var isPointer = match.Groups["ptr"].Success;

        var count = match.Groups["count"] is { Success: true, Value: var countStr }
            ? int.Parse(countStr)
            : 0;
        
        return new DFieldType()
        {
            Name = name,
            GenericType = genericParam,
            IsPointer = isPointer,
            Count = count
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Name);
        if(GenericType != null)
            sb.Append($"< {GenericType} >");
        if (Count > 0)
            sb.Append($"[{Count}]");
        return sb.ToString();
    }
}