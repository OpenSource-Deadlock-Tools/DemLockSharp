using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    private List<FieldDecoder> _entities;

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
        _classes.Add(dClass);
    }

    public DClass? GetClassById(int classId) => _classes.FirstOrDefault(c => c.ClassId == classId);
    public DClass? GetClassByName(string className) => _classes.FirstOrDefault(c => c.ClassName == className);
    public void AddField(DField field) => _fields.Add(field);
    public void AddSerializerRange(params DSerializer[] serializer) => _serializers.AddRange(serializer);
    public void AddSerializerRange(IEnumerable<DSerializer> serializer) => _serializers.AddRange(serializer);
    public void AddStringTable(StringTable stringTable) => _stringTables.Add(stringTable);

    /// <summary>
    /// Get back the instance baseline for the given classId if it exists, otherwise
    /// it will return null to indicate we don't have an instance baseline for the class
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    public byte[]? GetInstanceBaseline(int classId) =>
        _instanceBaselines.ContainsKey(classId) ? _instanceBaselines[classId] : null;

    // TODO: Optimize this look up and figure out what these god damn versions are all about and if I should care about lower order versions (manta doesn't)
    public DSerializer GetSerializerByClassName(string className, bool latestVersion = true) =>
        _serializers.Where(x => x.Name == className).OrderByDescending(x => x.Version).FirstOrDefault();

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
        if (!_fieldTypes.Contains(fieldType))
            _fieldTypes.Add(fieldType);
    }

    public void RefreshInstanceBaselines()
    {
        RefreshInstanceBaselines(_stringTables.FirstOrDefault(x => x.Name == "instanceBaselines"));
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

    public void DumpClassDefinitions(string outputDirectory)
    {
        foreach (var v in _serializers.Where(x => new List<string>()
                 {
                     "CCitadelPlayerPawn", "CEntityIdentity", "CCitadelAbilityComponent",
                     "ViewAngleServerChange_t", "ViewAngleServerChange_t", "FullSellPriceAbilityUpgrades_t","CBodyComponentBaseAnimGraph"
                 }.Contains(x.Name)))
        {
            string outputPath = Path.Combine(outputDirectory, $"{v.Name}.{v.Version}.class.json");

            JsonObject serializerObj = new JsonObject();
            serializerObj["ClassName"] = v.Name;
            serializerObj["Version"] = v.Version;


            JsonObject fieldObj = new JsonObject();
            serializerObj["Fields"] = fieldObj;

            Queue<string> context = new Queue<string>();

            for (int i = 0; i < v.Fields.Length; i++)
            {
                var field = v.Fields[i];
                List<string> fieldPath = new();
                if (!string.IsNullOrWhiteSpace(field.SendNode) && field.SendNode.Trim().Length > 0)
                {
                    foreach (var path in field.SendNode.Split('.'))
                    {
                        context.Enqueue(path);
                    }
                }

                context.Enqueue(field.Name ?? "");

                JsonObject current = fieldObj;
                while (context.Count > 0)
                {
                    var val = context.Dequeue();
                    if (current[val] == null)
                        current[val] = new JsonObject();
                    current = current[val]!.AsObject();
                }

                current["Path"] = i;
                current["Type"] = $"{field.PropertyType()}";
                current["NetworkType"] = $"{field.FieldType.ToString()}";
            }

            var options = new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder
                    .UnsafeRelaxedJsonEscaping, // Allow unsafe characters
                WriteIndented = true // Pretty print
            };
            File.WriteAllText(outputPath, JsonSerializer.Serialize(serializerObj, options));
        }
    }
}