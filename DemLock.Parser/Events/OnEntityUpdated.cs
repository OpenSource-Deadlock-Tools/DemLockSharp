using System.Text.Json.Nodes;
using DemLock.Entities;
using DemLock.Entities.Generated;

namespace DemLock.Parser.Events;

public class OnEntityUpdatedEventArgs : EventArgs
{
    public uint Tick { get; set; }
    public string UpdateType { get; set; }
    public BaseEntity Entity { get; set; }
}