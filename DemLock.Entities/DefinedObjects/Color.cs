using System.Drawing;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class DColor: DObject
{
    public Color Value { get; set; }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        IsSet = true;
        var rgba = bs.ReadVarUInt32();
        uint rr = (rgba & 0xFF000000) >> 24;
        uint gg = (rgba & 0x00FF0000) >> 16;
        uint bb = (rgba & 0x0000FF00) >> 8;
        uint aa = (rgba & 0x000000FF);
        Value = Color.FromArgb((int)((aa << 24) | (rr << 16) | (gg << 8) | bb));
    }

    public override object GetValue() => Value;
}