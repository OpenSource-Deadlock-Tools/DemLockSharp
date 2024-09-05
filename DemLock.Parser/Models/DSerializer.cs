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
    private static Dictionary<(string, int), EntityDecoder> _serializerCache = new();

    public EntityDecoder Instantiate()
    {
        if (_serializerCache.ContainsKey((Name, Version)))
        {
            return _serializerCache[(Name, Version)];
        }
        EntityDecoder newEntityDecoder = new EntityDecoder();
        newEntityDecoder.ClassName = this.Name;
        foreach (var field in this.Fields)
        {
            newEntityDecoder.AddField(field.Activate(),field.Name);
        }
        _serializerCache.Add((Name, Version), newEntityDecoder);
        return newEntityDecoder;
    }
    
    public EntityDecoder Instantiate(uint serial)
    {
        if (_serializerCache.ContainsKey((Name, Version)))
        {
            return _serializerCache[(Name, Version)];
        }
            
        EntityDecoder newEntityDecoder = new EntityDecoder();
        newEntityDecoder.ClassName = Name;
        newEntityDecoder.Serial = serial;
        foreach (var field in this.Fields)
        {
            newEntityDecoder.AddField(field.Activate(),field.Name);
        }
        _serializerCache.Add((Name, Version), newEntityDecoder);
        return newEntityDecoder;
    }
    public override string ToString()
    {
        return $"{Name}:{Version}";
    }
}