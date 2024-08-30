using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.FieldDecoders;

public abstract class FieldDecoder
{
    public abstract DObject DecodeField(FieldEncodingInfo encodingInfo,ref BitBuffer buffer);

    public static FieldDecoder GetDecoder(string typeName)
    {
        if (typeName == "float32")
        {
            return new Float32Decoder();
        }
        return new DefaultDecoder();
    }
    
}