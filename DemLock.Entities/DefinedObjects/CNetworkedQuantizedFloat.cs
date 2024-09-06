using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CNetworkedQuantizedFloat: FieldDecoder
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

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        var encoding = QuantizedFloatEncoding.Create(_encodingInfo);
        return encoding.Decode(ref bs);
    }


    public override string ToString()
    {
        return $"[CNetworkedQuantizedFloat : {_value}] ";
    }
}