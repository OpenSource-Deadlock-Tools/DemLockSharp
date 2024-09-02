using System.Text;
using DemLock.Entities.FieldDecoders;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class Vector2D : DObject
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

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        X = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
        Y = FloatDecoder.ReadFloat(ref bs, _encodingInfo);
    }

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine($"\"X\": \"{X}\",");
        sb.AppendLine($"\"Y\": \"{Y}\",");
        sb.AppendLine("}");

        return sb.ToString();
    }
    public override object GetValue()
    {
        return new {X = X, Y = Y};
    }
}