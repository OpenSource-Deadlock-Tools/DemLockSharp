using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CNetworkUtlVectorBase: DObject
{
    public int Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        // I have almost no idea what this is or means
        if (path.Length == 1)
        {
            var newSize = (int)bs.ReadVarUInt32();
            Value = newSize;
        }
    }

    public override object GetValue() => Value;

    public override string ToString()
    {
        return $"[CNetworkUtlVectorBase : {Value}]";
    }
}