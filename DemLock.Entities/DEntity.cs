using DemLock.Entities.Primitives;

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
    private Dictionary<string, DObject> _fields { get; set; } = new Dictionary<string, DObject>();
    public Dictionary<string, DObject>  Fields { get => _fields; }
    public DEntity()
    { }

    public void AddField(string fieldName)
    {
        if(!_fields.ContainsKey(fieldName))
            _fields.Add(fieldName, new DNull());
    }

}
