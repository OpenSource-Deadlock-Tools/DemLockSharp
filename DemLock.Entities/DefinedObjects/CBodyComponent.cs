using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class CBodyComponent: DObject
{
    public DUInt16 CellX { get; set; }
    public DUInt16 CellY { get; set; }
    public DUInt16 CellZ { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        throw new NotImplementedException();
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }
}