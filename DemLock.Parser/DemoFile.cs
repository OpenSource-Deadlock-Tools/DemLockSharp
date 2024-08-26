using System.Text;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

/// <summary>
/// Represents a singular demo file, and encapsulates all of the processing and
/// parsing that is being done on it
/// </summary>
internal class DemoFile: IDisposable
{
    public string FileStamp => CheckedReadFileStamp();
    
    private readonly string _filePath;
    private readonly Stream _stream;
    private string _fileStamp;

    public DemoFile(string filePath)
    {
        _filePath = filePath;
        _stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
    }

    

    private string CheckedReadFileStamp()
    {
            if (string.IsNullOrEmpty(_fileStamp))
            {
                byte[] stampBytes = new byte[16];
                var bytesRead = _stream.Read(stampBytes, 0, 16);
                if (bytesRead != 16) throw new Exception("Invalid demo file stream!");
                _fileStamp = Encoding.ASCII.GetString(stampBytes);
            }
            return _fileStamp;
    } 


    public DemoFrame ReadFrame()
    {
        CheckedReadFileStamp();
        
        var frame = new DemoFrame();
        uint rawCmd = _stream.ReadVarUInt32();
        frame.Command = (DemoFrameCommand)(rawCmd & ~64);
        frame.IsCompressed = (rawCmd & 64) == 64;
        frame.Tick = _stream.ReadVarUInt32();
        if (frame.Tick == 4294967295) frame.Tick = 0;
        frame.Size = _stream.ReadVarUInt32();
        frame.Data = new byte[frame.Size];
        var bytesRead = _stream.Read(frame.Data, 0, frame.Data.Length);
        
        return frame;
    }
    
    public void Dispose()
    {
        _stream.Dispose();
    }
}