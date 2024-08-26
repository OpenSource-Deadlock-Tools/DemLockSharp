namespace DemLock.Utils;

/// <summary>
/// Extension functions for the stream interface that will allow us to easily
/// read and process wire format types for standard byte streams
/// </summary>
public static class StreamExtensions
{
    public static uint ReadVarUInt32(this Stream stream)
    {
        uint result = 0;
        int count = 0;
        int b = 0;
        while (count < 5)
        {
            b = stream.ReadByte();
            result |= (uint)(b & 127) << (7*count);
            count++;
            if ((b & 0x80) == 0) break;
        }
        return result;
    }
    
    
}