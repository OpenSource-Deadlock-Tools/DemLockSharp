using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DemLock.Utils;

/// <summary>
/// Buffer that has been extracted from the C# demo parser for CS2, for some reason it handles reading bits in a
/// weird way that breaks during entity processing... will just use the code as is for now and update it to
/// a consolidated bit stream later
/// </summary>
public ref struct BitBuffer
{
    private static readonly uint[] BitMask;

    public static bool IsReadingFields = false;
    private int _bitsAvail = 0;
    private uint _buf = 0;
    private ReadOnlySpan<byte> _pointer;

    static BitBuffer()
    {
        BitMask = new uint[33];
        for (var i = 1; i < BitMask.Length - 1; ++i)
        {
            BitMask[i] = (1u << i) - 1;
        }

        BitMask[^1] = uint.MaxValue;
    }

    public BitBuffer(ReadOnlySpan<byte> pointer)
    {
        _pointer = pointer;
        FetchNext();
    }

    public int RemainingBytes => _pointer.Length + _bitsAvail / 8;
    public int BitsRemaining => _pointer.Length + _bitsAvail;

    private void FetchNext()
    {
        _bitsAvail = _pointer.Length >= 4 ? 32 : _pointer.Length * 8;
        UpdateBuffer();
    }

    public uint ReadBitsToUint(int numBits) => ReadUInt(numBits);
    public uint ReadUInt(int numBits)
    {
        if (_bitsAvail >= numBits)
        {
            var ret = _buf & BitMask[numBits];
            _bitsAvail -= numBits;
            if (_bitsAvail != 0)
            {
                _buf >>= numBits;
            }
            else
            {
                FetchNext();
            }

            if (IsReadingFields)
            {
                Console.Write($"ReadUint({numBits}): ");
                uint mask = 1;
                for (int i = 0; i < numBits; i++)
                {
                    Console.Write((mask & ret) != 0 ? "1" : "0");
                    mask <<= 1;
                }

                Console.WriteLine();
            }

            return ret;
        }
        else
        {
            var ret = _buf;
            numBits -= _bitsAvail;

            UpdateBuffer();

            ret |= (_buf & BitMask[numBits]) << _bitsAvail;
            _bitsAvail = 32 - numBits;
            _buf >>= numBits;

            if (IsReadingFields)
            {
                Console.Write($"ReadUint({numBits}): ");
                uint mask = 1;
                for (int i = 0; i < numBits; i++)
                {
                    Console.Write((mask & ret) != 0 ? "1" : "0");
                    mask <<= 1;
                }

                Console.WriteLine();
            }

            return ret;
        }
    }

    public byte ReadByte() => (byte)ReadUInt(8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void UpdateBuffer()
    {
        if (_pointer.Length < 4)
        {
            // .NET 8/PGO optimisation issue (https://github.com/dotnet/runtime/issues/95056)
            // We can't depend on stackalloc being zero-initialised here.
            fixed (uint* bufPtr = &_buf)
            {
                var bufBytes = (byte*)bufPtr;
                for (var i = 0; i < 4; ++i)
                {
                    bufBytes[i] = i < _pointer.Length ? _pointer[i] : default;
                }
            }

            _pointer = default;
        }
        else
        {
            _buf = MemoryMarshal.Read<uint>(_pointer[..4]);
            _pointer = _pointer[4..];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBit()
    {
        var ret = _buf & 1;
        if (--_bitsAvail == 0)
        {
            FetchNext();
        }
        else
        {
            _buf >>= 1;
        }

        return ret != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        var bits = ReadUInt(32);
        unsafe
        {
            return *(float*)&bits;
        }
    }

    public uint ReadUBitVar()
    {
        var ret = ReadUInt(6);
        switch (ret & (16 | 32))
        {
            case 16:
                ret = (ret & 15) | (ReadUInt(4) << 4);
                break;
            case 32:
                ret = (ret & 15) | (ReadUInt(8) << 4);
                break;
            case 48:
                ret = (ret & 15) | (ReadUInt(32 - 4) << 4);
                break;
        }

        return ret;
    }

    public uint ReadVarUInt32()
    {
        uint result = 0;
        var shift = 0;
        byte byteRead;

        do
        {
            byteRead = ReadByte();
            string s = $"{byteRead:b8}";
            result |= (uint)(byteRead & 0x7F) << shift;
            shift += 7;
        } while ((byteRead & 0x80) != 0);

        return result;
    }

    public int ReadVarInt32()
    {
        var result = ReadVarUInt32();
        return (int)(result >> 1) ^ -(int)(result & 1);
    }

    public void ReadBytes(scoped Span<byte> output)
    {
        for (var i = 0; i < output.Length; ++i)
        {
            output[i] = ReadByte();
        }
    }

    public void ReadBitsAsBytes(scoped Span<byte> output, int bits)
    {
        var bytes = bits / 8;
        var remainder = bits % 8;

        for (var i = 0; i < bytes; ++i)
        {
            output[i] = ReadByte();
        }

        if (remainder != 0)
        {
            output[bytes] = (byte)ReadUInt(remainder);
        }
    }

    public int ReadUBitVarFieldPath()
    {
        if (ReadBit())
            return (int)ReadUInt(2);
        if (ReadBit())
            return (int)ReadUInt(4);
        if (ReadBit())
            return (int)ReadUInt(10);
        if (ReadBit())
            return (int)ReadUInt(17);

        return (int)ReadUInt(31);
    }

    public ulong ReadUVarInt64()
    {
        var c = 0;
        var result = 0UL;
        byte b;

        do
        {
            b = ReadByte();
            if (c < 10)
                result |= (ulong)(b & 0x7f) << 7 * c;
            c += 1;
        } while ((b & 0x80) != 0);

        return result;
    }

    public long ReadVarInt64()
    {
        var result = ReadUVarInt64();
        return (long)(result >> 1) ^ -(long)(result & 1);
    }

    public float ReadAngle(int bits)
    {
        var max = (float)((1UL << bits) - 1);
        return 360.0f * (ReadUInt(bits) / max);
    }

    public float ReadCoord()
    {
        const int FRACT_BITS = 5;

        var hasInt = ReadBit();
        var hasFract = ReadBit();

        if (hasInt || hasFract)
        {
            var signBit = ReadBit();

            var intval = hasInt ? ReadUInt(14) + 1.0f : 0.0f;
            var fractval = hasFract ? ReadUInt(FRACT_BITS) : 0.0f;

            var value = intval + fractval * (1.0f / (1 << FRACT_BITS));
            return signBit ? -value : value;
        }

        return 0.0f;
    }

    public float ReadCoordPrecise()
    {
        return ReadUInt(20) * (360.0f / (1 << 20)) - 180.0f;
    }

    public string ReadStringUtf8()
    {
        // Allocate on the stack initially
        Span<byte> buf = stackalloc byte[260];

        var i = 0;
        byte b;
        while ((b = ReadByte()) != 0)
        {
            if (i == buf.Length)
            {
                var newBuf = new byte[buf.Length * 2];
                buf.CopyTo(newBuf);
                buf = newBuf;
            }

            buf[i++] = b;
        }

        // perf: tried using StringPool here, practically no difference
        return Encoding.UTF8.GetString(buf[..i]);
    }

    public float ReadNormal()
    {
        var isNeg = ReadBit();
        var len = ReadUInt(11);
        var ret = len * (1.0f / ((1 << 11) - 1));
        return isNeg ? -ret : ret;
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
}