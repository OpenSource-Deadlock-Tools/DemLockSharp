using System.Diagnostics;
using DemLock.Entities;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

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
    private Dictionary<int, DEntity> _entities;

    public EntityManager(DemoParserContext context)
    {
        _context = context;
        _entities = new Dictionary<int, DEntity>();
    }

    public void AddNewEntity(int index, DClass serverClass, uint serial)
    {
        var entity = _context.GetSerializerByClassName(serverClass.ClassName)?.Instantiate(serial);
        _entities[index] = entity;
    }

    public void DeleteEntity(int index)
    {
        _entities[index] = null!;
    }

    public void UpdateAtIndex(int index, byte[] entityData)
    {
        var bb = new BitBuffer(entityData);
        UpdateAtIndex(index, ref bb);
    }
    public void UpdateAtIndex(int index, ref BitBuffer entityData)
    {
        
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
             _entities[index].SetValue(pathSpan, ref entityData);
         }
    }

    public DEntity GetEntityAtIndex(int index)
    {
        return _entities[index];
    }

    public List<DEntity> GetEntityByName(string className)
    {
        return _entities.Values.Where(x => x.ClassName == className).ToList();
    }
}