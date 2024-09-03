using System.Text.Json.Nodes;

namespace DemLock.Parser.Events;

public class OnEntityUpdatedEventArgs : EventArgs
{
    public uint Tick { get; set; }
    public string EntityType { get; set; }
    public string UpdateType { get; set; }
    public JsonNode OriginalEntity { get; set; }
    public JsonNode ResultEntity { get; set; }
}