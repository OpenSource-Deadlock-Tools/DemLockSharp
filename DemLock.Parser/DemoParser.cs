using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// The main demo parser that will be used to set up the event listeners, and then send in a file to be
/// processed
/// </summary>
public class DemoParser
{
    public DemoEventSystem Events { get; }
    private FrameHandler _frameHandler; 

    public DemoParser()
    {
        Events = new DemoEventSystem();
        _frameHandler = new FrameHandler(Events);
    }
    public void ProcessDemo(string fileName)
    {
        using DemoFile demo = new DemoFile(fileName);
        DemoFrame frame;
        do
        {
            frame = demo.ReadFrame();
            _frameHandler.HandleFrame(frame);
        } while (frame.Command != DemoFrameCommand.DEM_Stop);
    }

}