using System.Diagnostics;
using DemLock.Entities;
using DemLock.Parser;
using Newtonsoft.Json;

namespace DemLock;

class Program
{
    static void Main(string[] args)
    {

        TestParseDemo();

    }

    static void TestParseDemo()
    {
         
        Stopwatch sw = Stopwatch.StartNew();
        DemoParserConfig config = new DemoParserConfig();

        config.LogMessageReads = false;
        config.LogReadFrames = false;
        
        DemoParser parser = new DemoParser(config);

        Dictionary<int, int> eventIdCounts = new Dictionary<int, int>();

        parser.GameEvents.OnSource1LegacyGameEvent += (e) =>
        {
            if(!eventIdCounts.TryAdd(e.Eventid, 1)) eventIdCounts[e.Eventid]++;
        };
        
        Dictionary<int, string> eventNames = new Dictionary<int, string>(); 
        parser.GameEvents.OnSource1LegacyGameEventList += (e) =>
        {
            foreach (var v in e.Descriptors)
            {
                eventNames[v.Eventid] = v.Name;
            }
        };
        
        
        
        parser.ProcessDemo("C:\\tmp\\DeadlockDemos\\534870CS.dem");

        foreach (var ev in eventIdCounts)
        {
            Console.WriteLine($"{eventNames[ev.Key]} :: {ev.Value}");
            
        }
        Console.WriteLine($"Processed demo in {sw.Elapsed.TotalSeconds} seconds");
        // 14011DEMLOCK.dem
        
        //parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");       
    }
}