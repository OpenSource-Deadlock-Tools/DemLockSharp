namespace DemLock.Parser.Models.StringTableEntries;

public abstract class StringTableEntry
{
    public StringTableEntry()
    {
    }

    public StringTableEntry(int index, string key, byte[] value)
    {
        Index = index;
        Key = key;
        Value = value;
    }

    public int Index { get; set; }
    public string Key { get; set; }
    public byte[] Value { get; set; }

    /// <summary>
    /// Factory method that will create a new entry based on the table type since we want to be able to
    /// use abstraction to make it easier and tighter to identify entries specific data later
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="index"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringTableEntry CreateNewEntry(string tableName, int index, string key, byte[] value)
    {
        // Switch statement that will produce the correct entry based on the table name
        switch (tableName)
        {
            case "ActiveModifiers":
                return new ActiveModifierTableEntry(index, key, value);
            case "userinfo":
                return new UserInfoTableEntry(index, key, value);
        }

        // If we don't have a proper factory method, just return the generic structure
        return new GenericTableEntry(index, key, value);
    }

    public abstract void SetValue(byte[] data);
}

public class GenericTableEntry : StringTableEntry
{
    public GenericTableEntry(int index, string key, byte[] value)
    {
        Index = index;
        Key = key;
        SetValue(value);
    }

    public sealed override void SetValue(byte[] data)
    {
        Value = data;
    }
}

public class ActiveModifierTableEntry : StringTableEntry
{
    private CModifierTableEntry _parsedData;

    #region Mapped Values

    public MODIFIER_ENTRY_TYPE EntryType => _parsedData.EntryType;
    public uint Parent => _parsedData.Parent;
    public uint SerialNumber => _parsedData.SerialNumber;
    public uint ModifierSubclass => _parsedData.ModifierSubclass;
    public int StackCount => _parsedData.StackCount;
    public int MaxStackCount => _parsedData.MaxStackCount;
    public float LastAppliedTime => _parsedData.LastAppliedTime;
    public float Duration => _parsedData.Duration;
    public uint Caster => _parsedData.Caster;
    public uint Ability => _parsedData.Ability;
    public int AuraProviderSerialNumber => _parsedData.AuraProviderSerialNumber;
    public object AuraProviderEhandle => _parsedData.AuraProviderEhandle;
    public object AbilitySubclass => _parsedData.AbilitySubclass;
    public object Bool1 => _parsedData.Bool1;
    public object Bool2 => _parsedData.Bool2;
    public object Bool3 => _parsedData.Bool3;
    public object Bool4 => _parsedData.Bool4;
    public object Int1 => _parsedData.Int1;
    public object Int2 => _parsedData.Int2;
    public object Int3 => _parsedData.Int3;
    public object Int4 => _parsedData.Int4;
    public object Float1 => _parsedData.Float1;
    public object Float2 => _parsedData.Float2;
    public object Float3 => _parsedData.Float3;
    public object Float4 => _parsedData.Float4;
    public object Float5 => _parsedData.Float5;
    public object Float6 => _parsedData.Float6;
    public object Float7 => _parsedData.Float7;
    public object Float8 => _parsedData.Float8;
    public object Float9 => _parsedData.Float9;
    public object Float10 => _parsedData.Float10;
    public object Uint1 => _parsedData.Uint1;
    public object Uint2 => _parsedData.Uint2;
    public object Uint3 => _parsedData.Uint3;
    public object Uint4 => _parsedData.Uint4;
    public object Vec1 => _parsedData.Vec1;
    public object Vec2 => _parsedData.Vec2;
    public object Vec3 => _parsedData.Vec3;
    public object Vec4 => _parsedData.Vec4;
    public object String1 => _parsedData.String1;
    public object String2 => _parsedData.String2;
    public object String3 => _parsedData.String3;
    public object String4 => _parsedData.String4;

    #endregion

    public ActiveModifierTableEntry(int index, string key, byte[] value) : base(index, key, value)
    {
        _parsedData = CModifierTableEntry.Parser.ParseFrom(value);
    }

    public override void SetValue(byte[] data)
    {
        _parsedData = CModifierTableEntry.Parser.ParseFrom(data);
        Value = data;
    }
}

public class UserInfoTableEntry : StringTableEntry
{
    private CMsgPlayerInfo _parsedData;
    public string Name => _parsedData.Name;
    public ulong Xuid => _parsedData.Xuid;
    public int Userid => _parsedData.Userid;
    public ulong Steamid => _parsedData.Steamid;
    public bool Fakeplayer => _parsedData.Fakeplayer;
    public bool Ishltv => _parsedData.Ishltv;

    public UserInfoTableEntry(int index, string key, byte[] value) : base(index, key, value)
    {
        _parsedData = CMsgPlayerInfo.Parser.ParseFrom(value);
    }

    public override void SetValue(byte[] data)
    {
        _parsedData = CMsgPlayerInfo.Parser.ParseFrom(data);
    }
}