using System.Text;
using DemLock.Entities;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a serializer that is sent in a packet on the demo file.
///
/// This means that it is NOT meant to be a serializer for JSON of proto data.
/// It defines how we should consume data that was sent for entity and class updates.
/// </summary>
public class DSerializer
{
    public string Name { get; set; }
    public int Version { get; set; }
    public DField[] Fields { get; set; }

    public DEntity Instantiate()
    {
        DEntity newEntity = new DEntity();
        newEntity.ClassName = this.Name;
        foreach (var field in this.Fields)
        {
            newEntity.AddField(field.Activate(),field.Name);
        }
        return newEntity;
    }
    
    public DEntity Instantiate(uint serial)
    {
        DEntity newEntity = new DEntity();
        newEntity.ClassName = Name;
        newEntity.Serial = serial;
        foreach (var field in this.Fields)
        {
            newEntity.AddField(field.Activate(),field.Name);
        }
        return newEntity;
    }

    public string DumpFields()
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        foreach (var f in this.Fields)
        {
            sb.AppendLine($"[{i++,-3}] {f.Name}::{f.FieldType}");
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"{Name}:{Version}";
    }
}