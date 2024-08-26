namespace DemLock.Parser.Events;

public class BaseDemoEventArgs: EventArgs
{
    public EventContext Context { get; set; }

    internal BaseDemoEventArgs(EventContext context)
    {
        Context = context;
    }
    internal BaseDemoEventArgs(uint tick)
    {
        Context = new EventContext()
        {
            Tick = tick
        };
    }
    
}