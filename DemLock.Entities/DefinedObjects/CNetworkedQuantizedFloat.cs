using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CNetworkedQuantizedFloat: DObject
{
    private FieldEncodingInfo _encodingInfo;
    private float _value;

    public CNetworkedQuantizedFloat(FieldEncodingInfo encodingInfo)
    {
        _encodingInfo = encodingInfo;
    }
    
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        var encoding = QuantizedFloatEncoding.Create(_encodingInfo);
        _value = encoding.Decode(ref bs);
        IsSet = true;
    }

    public override object GetValue() => _value;

    public override string ToString()
    {
        return $"[CNetworkedQuantizedFloat : {_value}] ";
    }
}