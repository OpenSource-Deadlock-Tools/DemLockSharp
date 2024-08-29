using System.Text;
using DemLock.Utils;
using Snappier;

namespace DemLock.Parser.Models;

public class StringTable
{
    public string Name { get; set; } = string.Empty;
    public bool UserDataFixedSize { get; set; }
    public bool UsingVarintBitCounts { get; set; }
    public int UserDataSize { get; set; }
    public int Flags { get; set; }
    public int EntryCount => _data.Count;

    /// <summary>
    /// Dictionary for storing our entries, even though entries contain index number, this will
    /// make it easier to target specific indexes (can't be dicked to use a normal array)
    /// </summary>
    private readonly Dictionary<int,StringTableEntry> _data = new();

    /// <summary>
    /// Get the key for the entry provided it exists, otherwise return back an empty string
    /// NOTE: Should this be null return in that case?
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public string? KeyAtEntry(int i)
    {
        if (_data.ContainsKey(i))
            return _data[i].Key;
        
        return string.Empty;
    }

    public void SetIndex(int index, string? key, byte[] data)
    {
        if (!_data.ContainsKey(index))
        {
            _data.Add(index,StringTableEntry.CreateNewEntry(Name,index, key, data));
            return;
        }
        
        var existingEntry = _data[index];
        
        if(!String.IsNullOrWhiteSpace(key) && existingEntry.Key != key)
            existingEntry.Key = key;
        
        if(data.Length > 0) 
            existingEntry.SetValue(data);
    }

    // TODO: update this to be a indexor override so it looks like I know what I'm doing
    public IEnumerable<StringTableEntry> GetEntries()
    {
        foreach(var entry in _data)
            yield return entry.Value;
    }
    public void Update(byte[] rawData, int numberChanges)
    {
        ParseBuffer(rawData, numberChanges);
    }
    
    private void ParseBuffer(byte[] rawdata, int numberChanges)
    {
        var data = new BitStream(rawdata);
        List<string> keys = new List<string>();
        int index = -1;
        if (data.BitsRemaining == 0) return;
        
        for (int i = 0; i < numberChanges; i++)
        {
            var key = "";
            var value = new byte[0];

            if (data.ReadBoolean()) index++;
            else index = (int)data.ReadVarUInt32() + 1;

            // Check if it has a key or not
            if (data.ReadBoolean())
            {
                if (data.ReadBoolean())
                {
                    var pos = data.ReadBitsToUint(5);
                    var size = data.ReadBitsToUint(5);

                    if (pos >= keys.Count)
                        key += data.ReadString();
                    else
                    {
                        var s = keys[(int)pos];
                        if (size > s.Length)
                        {
                            key += s + data.ReadString();
                        }
                        else
                        {
                            key += (s?.Substring(0, (int)size) ?? "") + data.ReadString();
                        }
                    }
                }
                else
                    key = data.ReadString();
            }
            else
                key = KeyAtEntry(index);

            if (!keys.Contains(key))
                keys.Add(key);

            var hasValue = data.ReadBoolean();
            if (hasValue)
            {
                uint bitSize = 0;
                var isCompressed = false;

                if (UserDataFixedSize)
                    bitSize = (uint)UserDataSize;
                else
                {
                    if ((Flags & 0x1) != 0)
                        isCompressed = data.ReadBoolean();
                    if (UsingVarintBitCounts)
                        bitSize = data.ReadUBit() * 8;
                    else
                        bitSize = data.ReadBitsToUint(17) * 8;
                }
                value = data.ReadToByteArray((uint)bitSize);
                if (isCompressed)
                    value = Snappy.DecompressToArray(value);
            }
            SetIndex(index, key, value);
        }
    }
}
