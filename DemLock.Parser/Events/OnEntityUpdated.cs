using System.Text.Json.Nodes;
using DemLock.Entities;

namespace DemLock.Parser.Events;

public class OnEntityUpdatedEventArgs : EventArgs
{
    public uint Tick { get; set; }
    public string EntityType { get; set; }
    public string UpdateType { get; set; }
    public List<UpdateDelta> Updates { get; set; }
}