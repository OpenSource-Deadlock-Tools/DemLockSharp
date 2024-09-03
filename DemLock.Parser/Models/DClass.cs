using DemLock.Entities;
using DemLock.Utils;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a class in the context of a demo file, it is given
/// the D prefix simply to indicate this is vastly different from
/// the built in class types
/// </summary>
public class DClass
{
    public string? ClassName { get; set; }
    public int ClassId { get; set; }
    public DField[] Fields { get; set; } = Array.Empty<DField>();

    public DObject Activate()
    {
        DEntity obj = new DEntity();
        foreach (var field in Fields)
            obj.AddField(field.Activate(),field.Name);
        return obj;
    }

    public override string ToString()
    {
        return $"{ClassName}::{ClassId}[{Fields.Length}]";
    }
}