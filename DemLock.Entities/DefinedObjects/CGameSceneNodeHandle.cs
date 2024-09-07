using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CGameSceneNodeHandle: FieldDecoder
{
    private UInt32 _value;
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        
        return bs.ReadVarUInt32();
    }
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        return bs.ReadVarUInt32();
    }

    public override string ToString()
    {
        return $"[CGameSceneNodeHandle : {_value}]";
    }
}