using System.Runtime.CompilerServices;

namespace DemLock.Utils;

public class BitStream
{
    private readonly byte[] _bytes;
    private int _bitPosition;
    private int _bytePosition;

    public static bool IsReadingFields = false;
    private const int MAX_STRING_LENGTH = 4082;

    public BitStream(byte[] bytes)
    {
        _bytes = bytes;
        _bitPosition = 0;
        _bytePosition = 0;
    }

    public uint ReadBitsToUint(int bitCount)
    {
        uint result = 0;
        for (int i = 0; i < bitCount; i++)
        {
            if (ReadBit())
            {
                result |= (uint)(1 << i);
            }
        }

        if (IsReadingFields)
        {
            Console.WriteLine($"ReadUBits: {result:B32}");
        }
        return result;
    }
    
    public uint ReadUInt(int bitCount)
    {
        uint result = 0;
        for (int i = 0; i < bitCount; i++)
        {
            if (ReadBit())
            {
                result |= (uint)(1 << i);
            }
        }

        if (IsReadingFields)
        {
            Console.Write($"ReadUint({bitCount}): ");
            uint mask = 1;
            for (int i = 0; i < bitCount; i++)
            {
                Console.Write((mask&result) != 0 ? "1" : "0");
                mask <<= 1;
            }
            Console.WriteLine();
        }
        return result;
    }

    public uint ReadUnt() => ReadUInt(32);
    
    public float ReadAngle(int bits)
    {
        var max = (float)((1UL << bits) - 1);
        return 360.0f * (ReadBitsToUint(bits) / max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        var bits = ReadBitsToUint(32);
        unsafe
        {
            return *(float*)&bits;
        }
    }

    public void ReadBytes(scoped Span<byte> output)
    {
        for (var i = 0; i < output.Length; ++i)
        {
            output[i] = ReadByte(8);
        }
    }

    public float ReadCoordPrecise()
    {
        return ReadBitsToUint(20) * (360.0f / (1 << 20)) - 180.0f;
    }

    public float ReadCoord()
    {
        const int FRACT_BITS = 5;

        var hasInt = ReadBit();
        var hasFract = ReadBit();

        if (hasInt || hasFract)
        {
            var signBit = ReadBit();

            var intval = hasInt ? ReadBitsToUint(14) + 1.0f : 0.0f;
            var fractval = hasFract ? ReadBitsToUint(FRACT_BITS) : 0.0f;

            var value = intval + fractval * (1.0f / (1 << FRACT_BITS));
            return signBit ? -value : value;
        }

        return 0.0f;
    }

    public ulong ReadUVarInt64()
    {

        // TODO: Figure out what the hell is happening here...
        // I am engineering this based on manta, as the C# parser
        // has some really weird logic in it that I cannot figure out
        // how I would adapt without reworking my whole parser
        byte b;
        ulong x = 0;
        int s = 0;
        for (int i = 0;; i++)
        {
            b = ReadByte();
            if (b < 0x80)
            {
                if (i > 9 || i == 9 && b > 1)
                    throw new Exception("It went and broke dawg");
                return x | (ulong)b << s;
            }
            x |= ((ulong)b&0x7f) << s;
            s += 7;
        }
        return x;
    }

    public int ReadUBitVarFieldPath()
    {
        if (ReadBit())
            return (int)ReadBitsToUint(2);
        if (ReadBit())
            return (int)ReadBitsToUint(4);
        if (ReadBit())
            return (int)ReadBitsToUint(10);
        if (ReadBit())
            return (int)ReadBitsToUint(17);

        return (int)ReadBitsToUint(31);
    }

    public int BitsRemaining
    {
        get
        {
            int remainingBitsInCurrentByte = 8 - _bitPosition;
            int remainingBytes = _bytes.Length - _bytePosition - 1;
            return remainingBytes * 8 + remainingBitsInCurrentByte;
        }
    }

    public bool ReadBit()
    {
        if (_bytePosition >= _bytes.Length)
        {
            throw new InvalidOperationException("End of stream reached.");
        }

        bool bit = (_bytes[_bytePosition] & (1 << _bitPosition)) != 0;
        _bitPosition++;

        if (_bitPosition >= 8)
        {
            _bitPosition = 0;
            _bytePosition++;
        }

        return bit;
    }



    public byte ReadByte(int bitCount)
    {
        byte result = 0;
        // TODO: Check byte alignment here, as it is better performance than raw bit access
        for (int i = 0; i < bitCount; i++)
        {
            if (ReadBit())
            {
                result |= (byte)(1 << i);
            }
        }

        if (IsReadingFields)
        {
            Console.WriteLine($"Readbyte: {result:B8}");
        }
        return result;
    }

    public byte ReadByte() => (byte)ReadUInt(8);
    

    public byte[] ReadBytes(uint byteCount)
    {
        List<byte> result = new List<byte>();
        for (int i = 0; i < byteCount; i++)
        {
            result.Add(ReadByte(8));
        }

        return result.ToArray();
    }

    public byte[] ReadToByteArray(uint bitCount)
    {
        byte[] result = new byte[CalculateBytesForBits((int)bitCount)];
        for (int i = 0; i < bitCount; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;

            // Read the next bit and set it in the result array
            if (ReadBit())
            {
                result[byteIndex] |= (byte)(1 << bitIndex);
            }
        }

        return result;
    }

    public (float x, float y, float z) Read3BitNormal()
    {
        float x = 0.0f, y = 0.0f;

        var hasX = ReadBit();
        var hasY = ReadBit();
        if (hasX)
            x = ReadNormal();
        if (hasY)
            y = ReadNormal();

        var negZ = ReadBit();
        var sumSqr = x * x + y * y;

        var z = sumSqr < 1.0f
            ? (float)Math.Sqrt(1.0 - sumSqr)
            : 0.0f;

        return (x, y, negZ ? -z : z);
    }

    public float ReadNormal()
    {
        var isNeg = ReadBit();
        var len = ReadBitsToUint(11);
        var ret = len * (1.0f / ((1 << 11) - 1));
        return isNeg ? -ret : ret;
    }

    public int ReadVarInt32()
    {
        var result = ReadVarUInt32();
        return (int)(result >> 1) ^ -(int)(result & 1);
    }

    public UInt32 ReadVarUInt32()
    {
        uint result = 0;
        var shift = 0;
        byte b = 0;
        do
        {
            b = ReadByte();
            string s = $"{b:b8}";
            result |= (uint)(b & 0x7F) << shift;
            shift += 7;
        } while ((b & 0x80) != 0);
        return result;
    }

    public uint ReadUBit()
    {
        uint ret = ReadBitsToUint(6);
        switch (ret & 48)
        {
            case 16:
                ret = (ret & 15) | (ReadBitsToUint(4) << 4);
                break;
            case 32:
                ret = (ret & 15) | (ReadBitsToUint(8) << 4);
                break;
            case 48:
                ret = (ret & 15) | (ReadBitsToUint(28) << 4);
                break;
        }

        return ret;
    }

    public bool ReadBoolean()
    {
        return ReadBit();
    }

    public string ReadString()
    {
        string result = "";
        for (int i = 0; i < MAX_STRING_LENGTH; i++)
        {
            byte b = ReadByte(8);
            if (b == 0) break;
            result += (char)b;
        }

        return result;
    }

    private int CalculateBytesForBits(int bitCount)
    {
        return (bitCount + 7) / 8;
    }
}