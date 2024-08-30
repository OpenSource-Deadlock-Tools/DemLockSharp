using DemLock.Entities;
using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// Context object that will join together all of the data into a nice
/// and tidy interface so as to avoid reallocating objects on the heap
/// when they are needed for processing sharing data.
///
/// This is because there are a few tables we will need to keep track of the
/// entire time, and updates to one table can effect others, which is
/// really hard to track.
/// </summary>
public class DemoParserContext
{
    /// <summary>
    /// Absolutely zero idea why this is 17, but every parser I reference has this set.
    /// For reference please check demofile-net, or Manta for the value... just going
    /// to set it and hope it works for now.
    /// </summary>
    internal const int NumEHandleSerialNumberBits = 17;
    public int ClassIdSize { get; set; }
    public int MaxPlayers { get; set; }
    private List<DClass> _classes;
    private List<DFieldType> _fieldTypes;
    private List<DField> _fields;
    private List<DSerializer> _serializers;
    private List<StringTable> _stringTables;
    private List<DObject> _entities;
    public EntityManager EntityManager { get; }

    private Dictionary<int, byte[]> _instanceBaselines;

    public DemoParserContext()
    {
        _stringTables = new();
        _classes = new List<DClass>();
        _entities = new();
        _fieldTypes = new List<DFieldType>();
        _fields = new List<DField>();
        _serializers = new();
        _instanceBaselines = new();
        EntityManager = new EntityManager(this); 
    }

    public void AddClass(DClass dClass)
    {
        var serializer = _serializers.FirstOrDefault(x => x.Name == dClass.ClassName);
        if (serializer != null)
        {
            dClass.Fields = serializer.Fields;
        }
        _classes.Add(dClass);  
    } 
    public DClass? GetClassById(int classId) => _classes.FirstOrDefault(c => c.ClassId == classId);
    public DClass? GetClassByName(string className) => _classes.FirstOrDefault(c => c.ClassName == className);
    public void AddField(DField field)=> _fields.Add(field);
    public void AddSerializerRange(params DSerializer[] serializer) => _serializers.AddRange(serializer);
    public void AddSerializerRange(IEnumerable<DSerializer> serializer) => _serializers.AddRange(serializer);
    public void AddStringTable(StringTable stringTable) => _stringTables.Add(stringTable);

    /// <summary>
    /// Get back the instance baseline for the given classId if it exists, otherwise
    /// it will return null to indicate we don't have an instance baseline for the class
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    public byte[]? GetInstanceBaseline(int classId) => _instanceBaselines.ContainsKey(classId) ? _instanceBaselines[classId] : null;

    public DSerializer GetSerializerByClassName(string className) => _serializers.FirstOrDefault(x => x.Name == className);
    
    // TODO: Some sorta error handling would be nice here
    public StringTable GetStringTableByIndex(int index) => _stringTables[index];

    /// <summary>
    /// Given a string table index, and a set of raw data, pass in the raw data to the string table so
    /// that it can get updated to the latest values
    /// </summary>
    /// <param name="index">The string table index we want to target</param>
    /// <param name="rawData">The raw data containing the updates to be parsed</param>
    /// <param name="numberOfUpdates">The number of changes that are in the raw data</param>
    public void UpdateStringTableAtIndex(int index, byte[] rawData, int numberOfUpdates)
    {
        _stringTables[index].Update(rawData, numberOfUpdates);

        // If we just updated our instance baselines, lets refresh the instance baseline tables
        if (_stringTables[index].Name == "instancebaseline")
        {
            RefreshInstanceBaselines(_stringTables[index]);
        }
    } 

    public void AddFieldType(DFieldType fieldType)
    {
        if(!_fieldTypes.Contains(fieldType))
            _fieldTypes.Add(fieldType);
    }

    public void RefreshInstanceBaselines()
    {
        RefreshInstanceBaselines(_stringTables.FirstOrDefault(x=>x.Name == "instanceBaselines"));
    }
    public void RefreshInstanceBaselines(StringTable? table)
    {
        if (table == null)
        {
            Console.WriteLine("Instance baseline table does not exist yet");
            return;
        }
        
        foreach (var entry in table.GetEntries())
        {
            if (int.TryParse(entry.Key, out int classId))
            {
                _instanceBaselines[classId] = entry.Value;
            }
        }
    }

    public void ClearContext()
    {
        _classes.Clear();
        _entities.Clear();
        _serializers.Clear();
    }

    public void PrintClasses()
    {
        Console.WriteLine("-------Class Table--------");
        //int i = 0;
        //foreach (DClass @class in _classes)
        //{
        //    Console.WriteLine($"[{i}] {@class.ClassId}::{@class.ClassName}");
        //    i++;
        //    if (i % 50 == 0) Console.ReadKey();
        //}
    }

    public void PrintFields()
    {
        //Console.WriteLine($"Fields--------[{_fields.Count}]");
        int i = 0;
        //foreach (var field in _fields)
        //{
        //    Console.WriteLine($"[{i}] {field.Name}::{field.SerializerName}::{field.SendNode}::{field.FieldType.Name}");
        //    i++;
        //    if (i % 50 == 0) Console.ReadKey();
        //}
    }
    public void PrintFieldTypes()
    {
        Console.WriteLine("Field Types--------");
        int i = 0;
        //foreach (var fieldType in _fieldTypes)
        //{
        //    Console.WriteLine($"[{i}] {fieldType.Name}::{fieldType.Count}::{fieldType.IsPointer}");
        //    i++;
        //    if (i % 50 == 0) Console.ReadKey();
        //}
    }
    
    public void PrintSerializers()
    {
        //Console.WriteLine($"Serializers --------[{_serializers.Count}]");
        
        int i = 0;
        //foreach (var sz in _serializers)
        //{
        //    Console.WriteLine($"[{i}] {sz.Name}::{sz.Version}::{sz.Fields.Length}");
        //    i++;
        //    if (i % 50 == 0) Console.ReadKey();
        //}
    }

    public void PrintStringTables()
    {
        Console.WriteLine($"===String Tables===");
        Console.WriteLine($"COUNT: {_stringTables.Count}");
        //int i = 0;
        //foreach (var st in _stringTables)
        //{
        //    Console.WriteLine($"[{st.Name}::{i}] - {st.EntryCount}");
        //    i++;
        //}
    }

    public void PrintInstanceBaselines()
    {
        Console.WriteLine("=====Instance Baselines=====");
        //Console.WriteLine($"COUNT: {_instanceBaselines.Count}");
        //foreach (var bl in _instanceBaselines)
        //{
        //    Console.WriteLine($"{bl.Key}::{bl.Value}");
        //}
    }
}