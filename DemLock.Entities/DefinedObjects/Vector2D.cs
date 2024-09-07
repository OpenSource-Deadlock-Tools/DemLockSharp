using System.Text;
using DemLock.Entities.FieldDecoders;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class Vector2D : FieldDecoder
{
    public float X { get; set; }
    public float Y { get; set; }

    private FieldEncodingInfo _encodingInfo;

    public Vector2D(FieldEncodingInfo encodingInfo)
    {
        _encodingInfo = encodingInfo;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        
        X = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Y = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        return (X, Y);
    }
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        X = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Y = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        return (X, Y);
    }

}