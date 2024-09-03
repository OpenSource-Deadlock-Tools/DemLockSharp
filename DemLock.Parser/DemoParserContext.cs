using DemLock.Entities;
using DemLock.Parser.Models;
using DemLock.Utils;

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
    public DemoParserConfig Config { get; set; }
    public int ClassIdSize { get; set; }
    public int MaxPlayers { get; set; }
    public float TickInterval { get; set; }
    public uint CurrentTick { get; set; }
    
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
        Config = new DemoParserConfig();
        _stringTables = new();
        _classes = new List<DClass>();
        _entities = new();
        _fieldTypes = new List<DFieldType>();
        _fields = new List<DField>();
        _serializers = new();
        _instanceBaselines = new();
        EntityManager = new EntityManager(this); 
    }
    
    public DemoParserContext(DemoParserConfig config)
    {
        Config = config;
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

    // TODO: Optimize this look up and figure out what these god damn versions are all about and if I should care about lower order versions (manta doesn't)
    public DSerializer GetSerializerByClassName(string className, bool latestVersion = true) => 
        _serializers.Where(x => x.Name == className ).OrderByDescending(x=>x.Version).FirstOrDefault();
    
    public DSerializer GetSerializerByClassName(string className, int version) => 
        _serializers.FirstOrDefault(x => x.Name == className && x.Version == version);
    
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
        //foreach (var field in _fields)
        //{
        //    Console.WriteLine($"[{i}] {field.Name}::{field.SerializerName}::{field.SendNode}::{field.FieldType.Name}");
        //    i++;
        //    if (i % 50 == 0) Console.ReadKey();
        //}

        //var e = _fieldTypes.Where(x => x.GenericType != null);
        //Console.WriteLine($"====Generic Types [{e.Count()}]====");
        //int i = 0;
        //foreach (var v in e)
        //{
        //    Console.WriteLine($"[{++i,-3}] {v}");
        //}

        //var e2 = e.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.Count());

        //Console.WriteLine($"==== Base Generics [{e2.Count}] ====");
        //Console.WriteLine("[Count]  type | usages");
        //i = 0;
        //foreach (var kv in e2)
        //{
        //    Console.WriteLine($"[{i++,-3}] {kv.Key} | {kv.Value,-4}");
        //    
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

    public void DebugFunction()
    {
        //CalculateUnsupportedEntities();
    }
    public void PrintSerializers()
    {
        return;
        foreach (var serverClass in _classes.Where(x=>x.ClassName == "CNPC_TrooperNeutral"))
        {
            Console.WriteLine($"=========={serverClass.ClassName}");
            try
            {
                var baseline = GetInstanceBaseline(serverClass.ClassId);
                if (baseline == null) continue;
                var entityBuffer = new BitBuffer(baseline);
                //var v = EntityManager.CreateEntity(ref entityBuffer, serverClass.ClassName);
                //Console.WriteLine(v.ToJson());
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR PARSING: {serverClass.ClassName} Probably something not supported yet!");
                Console.WriteLine($"{ex.Message}");
            }
            
            Console.WriteLine("====================================");
            Console.WriteLine($"Press any key to try to read another class...");
            Console.ReadKey();
        }
    }

    public void DumpObjectBaselines()
    {
        string dumpFolder = @"C:\tmp\DemoParserResults\BaselineDumps";
        foreach (var v in _instanceBaselines)
        {
            var outputFile = Path.Combine(dumpFolder, $"{v.Key}.DAT");
            File.WriteAllBytes(outputFile, v.Value);
        }
    }

    /// <summary>
    /// Debug function that will do every costly things to calculate exactly how many entities we are able to parse given
    /// they have a baseline from the string table that applies to them
    ///
    /// This is run after the whole demo plays out, and so might be prone to errors and long run times, but is
    /// useful for seeing how close the system support is to being able to actually get useful data
    /// </summary>
    private void CalculateUnsupportedEntities()
    {
        int entitiesChecked = 0;
        int baselinesMissing = 0;
        int successfullyParsed = 0;
        int failedToParse = 0;
        HashSet<string> errorTypes = new HashSet<string>();
        string[] targetClasses =
        {
            //"CCitadel_Ability_PrimaryWeapon_Bebop", // Parsing (unvalidated)
            //"CCitadelPlayerController", // Parsing (unvalidated)
            //"CCitadelTeam", // Parsing (unvalidated)
            //"CParticleSystem", // Parsing (unvalidated)
        };
        
        // Dynamic filter building to make it easier to back track on debugging
        var filteredClasses = _classes.AsEnumerable();
        
        // If we have targets, only show the targets
        if(targetClasses.Any())
            filteredClasses = filteredClasses.Where(x => targetClasses.Contains(x.ClassName));
        
        foreach (var serverClass in filteredClasses)
        {
            entitiesChecked++;
            try
            {
                var baseline = GetInstanceBaseline(serverClass.ClassId);
                if (baseline == null)
                {
                    baselinesMissing++;
                    continue;
                }
                var entityBuffer = new BitBuffer(baseline);
            }
            catch(Exception ex)
            {
                errorTypes.Add($"[{serverClass.ClassName}]{ex.Message}");
                failedToParse++;
            }
            successfullyParsed++;
        }
        
        Console.WriteLine($"BASELINE PARSING TEST RESULTS");
        Console.WriteLine($"CLASSES CHECKED   :\t{entitiesChecked,-4}");
        Console.WriteLine($"MISSING BASELINE  :\t{baselinesMissing,-4}");
        Console.WriteLine($"SUCCESSFUL        :\t{successfullyParsed,-4}");
        Console.WriteLine($"FAILED            :\t{failedToParse,-4}");
        Console.WriteLine($"UNIQUE EXCEPTIONS :\t{errorTypes.Count(),-4}");
        
        Console.WriteLine("====Exceptions====");
        foreach (var errorType in errorTypes)
        {
            Console.WriteLine(errorType);
        }
        
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