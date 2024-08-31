using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// Configuration schema for the demo parser that will be read in from the JSON settings
/// to control certain things in the parser
/// </summary>
public class DemoParserConfig
{
    public List<DemoFrameCommand> IgnoredFrames { get; set; } = new();
    public List<MessageTypes> IgnoredMessages { get; set; } = new();
    
    public bool LogReadFrames { get; set; }
    public bool LogMessageReads { get; set; }

}