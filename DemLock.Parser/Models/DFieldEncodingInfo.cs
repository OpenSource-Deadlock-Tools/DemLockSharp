namespace DemLock.Parser.Models;

public class DFieldEncodingInfo
{
    public string? VarEncoder { get; set; }
    public int BitCount { get; set; }
    public int EncodeFlags { get; set; }
    public float? LowValue { get; set; }
    public float? HighValue { get; set; }
}