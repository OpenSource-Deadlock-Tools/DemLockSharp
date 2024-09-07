using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class DFixedSizeArray:FieldDecoder
{
    public string TypeName { get; set; }
    public int Length { get; set; }
    private FieldDecoder _childDecoder;
    /// <summary>
    /// Right now this is a byte array but this is very very incorrect and I just need to get the baseline parsed and I can
    /// then properly address this issue
    /// </summary>
    public FieldDecoder[] Data { get; set; }
    public DFixedSizeArray(string typeName, int length, FieldDecoder childDecoder)
    {
        _childDecoder = childDecoder;
        TypeName = typeName;
        Length = length;
        Data = new FieldDecoder[length];
    }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length == 0) return bs.ReadBit();

        if (path.Length >= 1)
        {
            return Data[path[0]].SetValue(path[1..], ref bs);
        }

        return null;
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        throw new Exception("You should not be calling this!");
    }
    public override FieldDecoder GetFieldDecoder(ReadOnlySpan<int> path)
    {
        if (path.Length == 0) return new DBool();
        return _childDecoder.GetFieldDecoder(path[1..]);
    }



}