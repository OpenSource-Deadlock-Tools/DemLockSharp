using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// Wrapper for all frame processing operations within the demo parsing process
/// </summary>
public class FrameHandler
{
    private readonly DemoEventSystem _events;
    private readonly MessageHandler _messageHandler;
    private readonly DemoParserContext _context;

    public FrameHandler(DemoEventSystem eventSystem, MessageHandler messageHandler, DemoParserContext context)
    {
        _context = context;
        _events = eventSystem;
        _messageHandler = messageHandler;
    }

    public void HandleFrame(DemoFrame frame)
    {
        switch (frame.Command)
        {
            case DemoFrameCommand.DEM_Stop: return;
            case DemoFrameCommand.DEM_FileHeader:
                HandleFileHeader(frame);
                break;
            case DemoFrameCommand.DEM_ClassInfo:
                HandleClassInfo(frame);
                break;
        }
    }

    private void HandleFileHeader(DemoFrame frame)
    {
        var fileHeader = CDemoFileHeader.Parser.ParseFrom(frame.Data);
        _events.RaiseOnFileHeader(frame.Tick, fileHeader);
    }

    /// <summary>
    /// Handle the class info message frame and update the corresponding tables
    /// </summary>
    /// <param name="frame"></param>
    private void HandleClassInfo(DemoFrame frame)
    {
        CDemoClassInfo demClassInfo = CDemoClassInfo.Parser.ParseFrom(frame.Data);

        foreach (var v in demClassInfo.Classes)
        {
            _context.AddClass(new DClass()
            {
                ClassId = v.ClassId,
                ClassName = v.NetworkName
            });
        }
    }
}