using DemLock.Parser.Models;

namespace DemLock.Parser;

public class DemoFrames
{
    public Action? OnStop;
    public Action<CDemoFileHeader>? OnFileHeader;
    public Action<CDemoClassInfo>? OnClassInfo;
    public Action<CDemoSendTables>? OnSendTables;
    public Action<CDemoPacket>? OnPacket;
    public Action<CDemoPacket>? OnSignonPacket;
    public Action<CDemoFullPacket>? OnFullPacket;

    internal DemoFrames()
    {
    }

    internal void ProcessFrame(FrameData frame)
    {
        switch (frame.Command)
        {
            case DemoFrameCommand.DEM_Stop:
                OnStop?.Invoke();
                break;
            case DemoFrameCommand.DEM_FileHeader:
                OnFileHeader?.Invoke(CDemoFileHeader.Parser.ParseFrom(frame.Data));
                break;
            case DemoFrameCommand.DEM_ClassInfo:
                OnClassInfo?.Invoke(CDemoClassInfo.Parser.ParseFrom(frame.Data));
                break;
            case DemoFrameCommand.DEM_SendTables:
                OnSendTables?.Invoke(CDemoSendTables.Parser.ParseFrom(frame.Data));
                break;
            case DemoFrameCommand.DEM_Packet:
                OnPacket?.Invoke(CDemoPacket.Parser.ParseFrom(frame.Data));
                break;
            case DemoFrameCommand.DEM_SignonPacket:
                OnSignonPacket?.Invoke(CDemoPacket.Parser.ParseFrom(frame.Data));
                break;
            case DemoFrameCommand.DEM_FullPacket:
                OnFullPacket?.Invoke(CDemoFullPacket.Parser.ParseFrom(frame.Data));
                break;
        }
    }
}