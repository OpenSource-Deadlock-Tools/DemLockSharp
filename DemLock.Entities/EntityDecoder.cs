using System.Diagnostics;
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

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs, ref UpdateDelta returnDelta)
    {
        if(returnDelta == null) returnDelta = new UpdateDelta();
        
        if (path.Length >= 1)
        {
            if (string.IsNullOrEmpty(returnDelta.Field)) returnDelta.Field = _fieldNames[0];
            else returnDelta.Field += $".{_fieldNames[0]}";
            var targetField = _fields[path[0]];
            targetField.SetValue(path[1..], ref bs, ref returnDelta);
        }
        
        // If path length is 0, then the entity setter is to check if the object is set or not
        if (path.Length == 0)
        {
            IsSet = bs.ReadBit();
            returnDelta.Field += "@IsSet";
            returnDelta.Value = IsSet;
        }
    }
    /// <summary>
    /// Converts the entity to a JSON object for rendering in the console.
    ///
    /// Note, this will likely be really slow because I am doing it in a pretty lazy way as it should not
    /// be needed often outside of debugging
    /// </summary>
    /// <returns></returns>
    public override string ToJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        List<string> fieldPairs = new();
        fieldPairs.Add($"\"@IsSet\":\"{IsSet}\"");
        fieldPairs.Add($"\"@ClassName\":\"{ClassName}\"");
        
        for (int i = 0; i < _fields.Count; ++i)
        {
            var field = _fields[i];
            var name = _fieldNames[i];
            StringBuilder sbfp = new StringBuilder();
            sbfp.Append('"').Append($"[{i}]").Append(name).Append('"');
            sbfp.Append(':');
            sbfp.Append(field.ToJson());
            
            fieldPairs.Add(sbfp.ToString());
        }
        sb.AppendLine(string.Join(",\n", fieldPairs));
        sb.AppendLine("}");


        return sb.ToString();
    }
    public override JsonNode ToJsonNode()
    {
        JsonObject jObj = new JsonObject();
        for (int i = 0; i < _fields.Count; i++)
        {
            jObj[_fieldNames[i]] = _fields[i].ToJsonNode();
        }
        return jObj;
    }
    
    public override object GetValue()
    {
        return _fields;
    }
}
