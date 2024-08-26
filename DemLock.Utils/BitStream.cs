namespace DemLock.Utils;

public class BitStream
{
        private readonly byte[] _bytes;
        private int _bitPosition;
        private int _bytePosition;

        private const int MAX_STRING_LENGTH = 4082;
    
        public BitStream(byte[] bytes)
        {
            _bytes = bytes;
            _bitPosition = 0;
            _bytePosition = 0;
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
            return result;
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
    
            return result;
        }
    
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
    
        public UInt32 ReadVarUInt32()
        {
            UInt32 result = 0;
            int count = 0;
            uint b = 0;
            while (count < 5)
            {
                b = ReadBitsToUint(8);
                result |= (b & 127) << (7 * count);
                count += 1;
                if ((b & 0x80) == 0) break;
            }
    
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