using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.FieldDecoders;

/// <summary>
/// This is the default decoder that can be returned and is mainly used to error handle in a situation
/// where we have a field that does not have a valid mapping to a real decoder.
/// </summary>
public class DefaultDecoder: FieldDecoder
{
    public override DObject DecodeField(FieldEncodingInfo encodingInfo,ref BitBuffer buffer)
    {
        throw new NotImplementedException();
    }
}