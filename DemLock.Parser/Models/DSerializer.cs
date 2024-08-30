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
    public string? Name { get; set; }
    public int Version { get; set; }
    public DField[] Fields { get; set; }

    public DObject Instantiate()
    {
        DEntity newEntity = new DEntity();
        newEntity.ClassName = this.Name;
        foreach (var field in this.Fields)
        {
            newEntity.AddField(field.Activate(),field.Name);
        }
        return newEntity;
    }
    
}