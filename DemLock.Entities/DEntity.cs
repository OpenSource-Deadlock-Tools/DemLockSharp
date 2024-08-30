using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities;

/// <summary>
/// The base entity, which can, on it's own contain all of the entity data gotten
/// through the deserialization process, and can have specific versions derived
/// from it to describe more common functionality
/// </summary>
public class DEntity: DObject
{
    /// <summary>
    /// The class name that this entity is derived from
    /// </summary>
    public string ClassName { get; set; }
    /// <summary>
    /// The list of instantiated fields that are attached to this entity
    /// </summary>
    private List<ActiveField> _fields { get; set; } = new ();
    
    public List<ActiveField>  Fields { get => _fields; set => _fields = value; }
    public DEntity()
    { }
    public void AddField(DObject value, string fieldName)
    {
        _fields.Add(new ActiveField(fieldName, value));
    }
    
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        // If our path length is 1, we are setting a direct field
        if (path.Length >= 1)
        {
            var targetField = _fields[path[0]];
            targetField.Value.SetValue(path[1..], ref bs);
        }
        // If path length is 0, then the entity setter is to check if the object is set or not
        if (path.Length == 0)
        {
            IsSet = bs.ReadBit();
        }
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }
}
