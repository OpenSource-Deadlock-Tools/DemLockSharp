namespace DemLock.Parser;

/// <summary>
/// Processor for handling messages that are extracted from demo packets
/// </summary>
public class MessageHandler
{
    private readonly DemoParserContext _context;
    private readonly DemoEventSystem _events;
    public MessageHandler(DemoEventSystem events, DemoParserContext context)
    {
        _events = events;
        _context = context;
    }
    
}