using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Nodes;
using DemLock.Entities.Generics;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities;

/// <summary>
/// The base entity, which can, on it's own contain all of the entity data gotten
/// through the deserialization process, and can have specific versions derived
/// from it to describe more common functionality
/// </summary>
public class EntityDecoder: FieldDecoder
{
    /// <summary>
    /// The class name that this entity is derived from
    /// </summary>
    public string ClassName { get; set; }
    public uint Serial { get; set; }
    
    /// <summary>
    /// The list of instantiated fields that are attached to this entity
    /// </summary>
    private List<FieldDecoder> _fields { get; set; } = new ();

    /// <summary>
    /// Dictionary that will contain a mapping of the field name to the field that it points to
    /// </summary>
    private List<string> _fieldNames = new();
    
    public List<FieldDecoder>  Fields { get => _fields; set => _fields = value; }
    public EntityDecoder()
    { }
    public void AddField(FieldDecoder value, string fieldName)
    {
        if (value is DNull)
        {
            throw new Exception($"Attempted to add DNull value to a field");
        }
        // Getting the count before we add the value is akin to letting us get the index of the new value ahead of time
        _fieldNames.Add(fieldName);
        _fields.Add(value);
    }
    
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
    public override object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        if (path.Length >= 1)
        {
            var targetField = _fields[path[0]];
            return targetField.SetValue(path[1..], ref bs);
        }
        // If path length is 0, then the entity setter is to check if the object is set or not
        if (path.Length == 0)
        {
            return bs.ReadBit();
        }

        return null;
    }
    
    public override FieldDecoder GetFieldDecoder(ReadOnlySpan<int> path)
    {
        if (path.Length >= 1)
        {
            var targetField = _fields[path[0]];
            return targetField.GetFieldDecoder(path[1..]);
        }
        // If path length is 0, then the entity setter is to check if the object is set or not
        if (path.Length == 0)
        {
            return new DBool();
        }

        return null;
    }

    public void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs, ref object entity)
    {
        if (path.Length >= 1)
        {
            var targetField = _fields[path[0]];
            var d =  targetField.SetValue(path[1..], ref bs);
        }
        // If path length is 0, then the entity setter is to check if the object is set or not
        if (path.Length == 0)
        {
            var v = bs.ReadBit();
        }
    }

    public override object ReadValue(ref BitBuffer bs)
    {
        throw new NotImplementedException("You should not be reading a value of an entity like this!");
    }

    public override void ReadFieldName(ReadOnlySpan<int> path, ref string fieldName)
    {
        
        if (path.Length >= 1)
        {
            if(string.IsNullOrEmpty(fieldName)) fieldName = _fieldNames[path[0]];
            else fieldName += "." + _fieldNames[path[0]];
            
            _fields[path[0]].ReadFieldName(path[1..], ref fieldName);
             return;
        }
        base.ReadFieldName(path, ref fieldName);
    }
}
