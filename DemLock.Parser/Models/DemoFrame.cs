namespace DemLock.Parser.Models;

/// <summary>
/// Represents an extracted frame from the demo file
/// </summary>
public class DemoFrame
{
    public DemoFrameCommand Command { get; set; }
    public bool IsCompressed { get; set; }
    public uint Tick { get; set; }
    public uint Size { get; set; }
    public byte[] Data { get; set; }

}