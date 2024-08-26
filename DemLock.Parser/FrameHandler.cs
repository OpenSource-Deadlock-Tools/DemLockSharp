using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// Wrapper for all frame processing operations within the demo parsing process
/// </summary>
public class FrameHandler
{

    private readonly DemoEventSystem _events;
    public FrameHandler(DemoEventSystem eventSystem)
    {
        _events = eventSystem;
    }
    
    public void HandleFrame(DemoFrame frame)
    {
        switch (frame.Command)
        {
            case DemoFrameCommand.DEM_Stop: return;
            case DemoFrameCommand.DEM_FileHeader:
                HandleFileHeader(frame);
                break;
        }
    }

    private void HandleFileHeader(DemoFrame frame)
    {
        var fileHeader = CDemoFileHeader.Parser.ParseFrom(frame.Data);
        _events.RaiseOnFileHeader(frame.Tick, fileHeader);
    }
    
}