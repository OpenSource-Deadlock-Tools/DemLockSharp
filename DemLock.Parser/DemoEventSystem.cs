using System.Text.Json.Nodes;
using DemLock.Entities;
using DemLock.Parser.Events;

namespace DemLock.Parser;

/// <summary>
/// Class that will wrap up all of the events that will be derived
/// and emitted from the demo system, as well as methods to simplify
/// invoking them safely from within a demo parser run
/// </summary>
public class DemoEventSystem
{
    public event EventHandler<OnChatMessageEventArgs> OnChatMessage;

    internal void RaiseOnChatMessage(object sender, OnChatMessageEventArgs e)
    {
        OnChatMessage?.Invoke(this, e);
    }

    public event EventHandler<OnFileHeaderEventArgs> OnFileHeader;

    internal void RaiseOnFileHeader(uint tick, CDemoFileHeader header) =>
        OnFileHeader?.Invoke(this, new OnFileHeaderEventArgs(header, tick));

    public event EventHandler<OnEntityUpdatedEventArgs> OnEntityUpdated;

    internal void Raise_OnEntityUpdated(uint tick, object updates, string entityClass, string updateType) => OnEntityUpdated?.Invoke(this, new OnEntityUpdatedEventArgs()
    {
        Tick = tick,
        Updates = updates,
        UpdateType = updateType,
        EntityType = entityClass
    });

    internal DemoEventSystem()
    {
    }
}