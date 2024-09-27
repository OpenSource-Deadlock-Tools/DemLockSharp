using System.Text;
using DemLock.Parser.Models;
using DemLock.Utils;
using Snappier;

namespace DemLock.Parser
{
    /// <summary>
    /// Represents a singular demo stream, and encapsulates all of the processing and
    /// parsing that is being done on it.
    /// </summary>
    internal class DemoStream : IDisposable
    {
        public readonly string FileHeader;
        private readonly Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoStream"/> class.
        /// </summary>
        /// <param name="stream">A instance of IO.Stream.</param>
        public DemoStream(Stream stream)
        {
            _stream = stream;
            FileHeader = ReadFileHeader();
        }

        public static DemoStream FromFilePath(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            return new DemoStream(stream);
        }


        private string ReadFileHeader()
        {
            byte[] stampBytes = new byte[16];
            return _stream.Read(stampBytes, 0, 16) != 16
                ? throw new ArgumentException("Invalid demo file stream!")
                : Encoding.ASCII.GetString(stampBytes);
        }

        public FrameData ReadFrame()
        {
            var frame = new FrameData();
            uint rawCmd = _stream.ReadVarUInt32();
            frame.Command = (DemoFrameCommand)(rawCmd & ~64);
            frame.Tick = _stream.ReadVarUInt32();
            if (frame.Tick == 4294967295)
            {
                frame.Tick = 0;
            }

            // Read the size
            frame.Size = _stream.ReadVarUInt32();

            // Read the raw data in
            frame.Data = new byte[frame.Size];
            var bytesRead = _stream.Read(frame.Data, 0, frame.Data.Length);

            // If the frame is compressed we will need to decompress it
            if ((rawCmd & 64) == 64)
            {
                frame.Data = Snappy.DecompressToArray(frame.Data);
            }

            return frame;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
