using DemLock.Entities;
using DemLock.Entities.ClassMappings;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

public class EntityFieldData
{
    public EntityFieldData(string fieldName, object fieldValue)
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }

    public string FieldName { get; set; }
    public object FieldValue { get; set; }

    public override string ToString()
    {
        return $"{FieldName}::{FieldValue}";
    }
}

public class EntityMetaData
{
    public string ClassName { get; set; }
    public EntityBinder Binder { get; set; }
}

/// <summary>
/// Manager for entities to consolidate all of the logic needed to instantiating, and updating
/// the objects
/// </summary>
public class EntityManager
{
    private DemoParserContext _context;

    /// <summary>
    /// The entities that are being tracked by the system, a simple array would probably work here,
    /// however I have opted for a dictionary, as this makes it easier to reason about, and deals with
    /// cases where there might end up being gaps, which again could be handled but this is easier
    /// just to get things running
    /// </summary>
    private Dictionary<int, Dictionary<ulong, EntityFieldData>> _entities;
    private Dictionary<int, EntityMetaData> _metaData;
    private Dictionary<int, EntityDecoder> _deserializerMap;
    private Dictionary<ulong, FieldDecoder> _fieldDecoderMap;
    private Dictionary<int, Entity> _mappedEntities;

    public EntityManager(DemoParserContext context)
    {
        _mappedEntities = new();
        _context = context;
        _metaData = new();
        _fieldDecoderMap = new Dictionary<ulong, FieldDecoder>();
        _deserializerMap = new Dictionary<int, EntityDecoder>();
        _entities = new();
    }

    public void AddNewEntity(int index, DClass serverClass, uint serial)
    {
        var entity = _context.GetSerializerByClassName(serverClass.ClassName)?.Instantiate(serial);
        _deserializerMap[index] = entity;
        _entities[index] = new Dictionary<ulong, EntityFieldData>();
        if (serverClass.ClassName == "CCitadelPlayerPawn")
        {
            _mappedEntities[index] = new CCitadelPlayerPawn();
        }
        _metaData[index] = new EntityMetaData()
        {
            ClassName = serverClass.ClassName
        };

        if (_context.EntityBinders.TryGetValue(serverClass.ClassName, out EntityBinder entityBinder))
        {
            _metaData[index].Binder = entityBinder;
        }
    }

    public void DeleteEntity(int index)
    {
        _entities[index] = null!;
        _deserializerMap[index] = null!;
        _metaData[index] = null!;
        _mappedEntities[index] = null!;
    }

    public void UpdateAtIndex(int index, byte[] entityData)
    {
        var bb = new BitBuffer(entityData);
        UpdateAtIndex(index, ref bb);
    }

    public (EntityMetaData MetaData, EntityFieldData[] Fields) UpdateAtIndex(int index, ref BitBuffer entityData)
    {
        List<EntityFieldData> entityDataList = new();
        var metaData = _metaData[index];
        
        Span<FieldPath> fieldPaths = stackalloc FieldPath[512];
        var fp = FieldPath.Default;
        // Keep reading field paths until we reach an op with a null reader.
        // The null reader signifies `FieldPathEncodeFinish`.
        var fpi = 0;
        while (FieldPathEncoding.ReadFieldPathOp(ref entityData) is { Reader: { } reader })
        {
            if (fpi == fieldPaths.Length)
            {
                var newArray = new FieldPath[fieldPaths.Length * 2];
                fieldPaths.CopyTo(newArray);
                fieldPaths = newArray;
            }

            reader.Invoke(ref entityData, ref fp);
            fieldPaths[fpi++] = fp;
        }

        fieldPaths = fieldPaths[..fpi];

        for (var idx = 0; idx < fieldPaths.Length; idx++)
        {
            var fieldPath = fieldPaths[idx];
            var pathSpan = fieldPath.AsSpan();
            
            var deserializer = _deserializerMap[index];
            var value = deserializer.SetValue(pathSpan, ref entityData);
            var hash = fieldPath.GetHash();

            string fieldName = null;
            EntityFieldData fieldData;

            if (_mappedEntities.ContainsKey(index) && _mappedEntities[index] != null)
            {
                _mappedEntities[index].UpdateProperty(pathSpan, value);
                continue;
            }
            if (_entities[index].TryGetValue(hash, out fieldData))
            {
                if (string.IsNullOrEmpty(fieldData.FieldName))
                    deserializer.ReadFieldName(pathSpan, ref fieldName);
                fieldData.FieldValue = value;
            }
            else
            {
                deserializer.ReadFieldName(pathSpan, ref fieldName);
                fieldData = new EntityFieldData(fieldName, value);
                _entities[index][hash] = fieldData;
            }

            entityDataList.Add(fieldData);
        }
        
        if(metaData.Binder != null) metaData.Binder?.RaiseEntityChangedEvent(_entities[index].Values.ToList());
        
        return (metaData, _entities[index].Values.ToArray());
    }

}