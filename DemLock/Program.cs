using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using DemLock.Entities.ClassMappings;
using DemLock.Parser;
using DemLock.Parser.Models;

namespace DemLock;

class Program
{
    static void Main(string[] args)
    {
        
        Stopwatch sw = Stopwatch.StartNew();
        DemoParserConfig config = new DemoParserConfig();
        
        config.LogMessageReads = false;
        config.LogReadFrames = false;
        
        DemoParser parser = new DemoParser(config);
        parser.Events.OnEntityUpdated += (sender, eventArgs) =>
        {
            if (eventArgs.Updates is CCitadelPlayerPawn pawn)
            {
                Console.WriteLine($"{pawn.SimulationTime}");
                Console.WriteLine($"\t{pawn.CBodyComponent.EyeAngles}");
            }
        };
        parser.DumpClassDefinitions("C:\\tmp\\DeadlockDemos\\534870CS.dem", "C:\\tmp\\ClassDefinitions");
        Console.WriteLine($"Processed demo in {sw.Elapsed.TotalSeconds} seconds");
        // 14011DEMLOCK.dem
        
        //parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");
    }
}