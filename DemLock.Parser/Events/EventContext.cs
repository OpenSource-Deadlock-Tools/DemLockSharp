namespace DemLock.Parser.Events;

/// <summary>
/// Provides a standard context that will be attached to events so you can know when they
/// happened
/// </summary>
public class EventContext
{
    public uint Tick { get; set; }
}