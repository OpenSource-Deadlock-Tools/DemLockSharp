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
    
}