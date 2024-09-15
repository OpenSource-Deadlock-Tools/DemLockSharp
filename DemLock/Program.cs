using System.Diagnostics;
using DemLock.Entities;
using DemLock.Entities.Generated;
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
        parser.Events.OnCCitadelUserMsg_PostMatchDetails += (sender, e) =>
        {
            Console.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented));
        };
        
        parser.Events.OnEntityUpdated += (sender, eventArgs) =>
        {
            //if (eventArgs.Entity is CCitadelPlayerPawn player)
            //{
            //    Console.WriteLine($"{player.m_flSimulationTime}==>{player.m_iHealth}/{player.m_iMaxHealth}");
            //}
        };
        parser.ProcessDemo("C:\\tmp\\DeadlockDemos\\534870CS.dem");
        Console.WriteLine($"Processed demo in {sw.Elapsed.TotalSeconds} seconds");
        // 14011DEMLOCK.dem
        
        //parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");       
    }
}