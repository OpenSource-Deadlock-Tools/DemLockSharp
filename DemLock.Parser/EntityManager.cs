using DemLock.Entities;
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
    public int ClassId { get; set; }
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

    private Dictionary<ulong, string> _witness;
    private Dictionary<int, FieldDecoder> _fieldDecoders;
    //private Dictionary<int, BaseEntity> _mappedEntities;

    public EntityManager(DemoParserContext context)
    {
        _witness = new();
        //_mappedEntities = new();
        _context = context;
        _metaData = new();
        _fieldDecoders = new Dictionary<int, FieldDecoder>();
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
            //_mappedEntities[index] = new CCitadelPlayerPawn();
        }

        _metaData[index] = new EntityMetaData()
        {
            ClassName = serverClass.ClassName
        };
    }

    public void DeleteEntity(int index)
    {
        _entities[index] = null!;
        _deserializerMap[index] = null!;
        _metaData[index] = null!;
        //_mappedEntities[index] = null!;
    }

    public void UpdateAtIndex(int index, byte[] entityData)
    {
        var bb = new BitBuffer(entityData);
        UpdateAtIndex(index, ref bb);
    }

    public object UpdateAtIndex(int index, ref BitBuffer entityData)
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

        //BaseEntity targetEntity = null;

        //if (_mappedEntities.ContainsKey(index))
        //{
        //    targetEntity = _mappedEntities[index];
        //}

        for (var idx = 0; idx < fieldPaths.Length; idx++)
        {
            var fieldPath = fieldPaths[idx];
            var fieldHash = fieldPath.GetHash(metaData.ClassId);

            var pathSpan = fieldPath.AsSpan();

            var deserializer = _deserializerMap[index];
            FieldDecoder decoder = deserializer.GetFieldDecoder(pathSpan);
            var value = decoder.ReadValue(ref entityData);
            //targetEntity?.UpdateProperty(pathSpan, value);


            continue;
            if (metaData.ClassName == "CCitadelPlayerPawn")
            {
                var hash = fieldPath.GetHash();
                string fieldName = null;
                EntityFieldData fieldData;
                if (_entities[index].TryGetValue(hash, out fieldData))
                {
                    if (string.IsNullOrEmpty(fieldData.FieldName))
                        deserializer.ReadFieldName(pathSpan, ref fieldName);
                    fieldData.FieldValue = value;
                }
                else
                {
                    deserializer.ReadFieldName(pathSpan, ref fieldName);
                }

                Console.WriteLine($"\t[{string.Join(",", pathSpan.ToArray())}] ({decoder}){fieldName}::{value}");
            }
        }

        return null;
    }
}