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
    private List<DClass> _classes;
    private List<DEntity> _entities;
    private List<DSerializer> _serializers;

    public DemoParserContext()
    {
        _classes = new List<DClass>();  
        _entities = new List<DEntity>();
        _serializers = new List<DSerializer>();
    }

    public void AddClass(DClass @class) => _classes.Add(@class);

    public void ClearContext()
    {
        _classes.Clear();
        _entities.Clear();
        _serializers.Clear();
    }

    public void PrintClasses()
    {
        Console.WriteLine("Class Table--------");
        int i = 0;
        foreach (DClass @class in _classes)
        {
            Console.WriteLine($"[{i}] {@class.ClassId}::{@class.ClassName}");
            i++;
            if (i % 50 == 0) Console.ReadKey();
        }
    }
}