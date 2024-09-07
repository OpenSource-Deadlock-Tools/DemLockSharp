using DemLock.Entities.FieldDecoders;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class Vector4D : FieldDecoder
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }

    private FieldEncodingInfo _encodingInfo;

    public Vector4D(FieldEncodingInfo encodingInfo)
    {
        _encodingInfo = encodingInfo;
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException("Vector4D::SetValue");
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        X = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Y = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Z = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        W = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        
        return (X, Y, Z, W);
    }
    
    public override object ReadValue(ref BitBuffer bs)
    {
        X = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Y = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Z = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        W = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        
        return (X, Y, Z, W);
    }
}