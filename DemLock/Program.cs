using System.Diagnostics;
using DemLock.Entities;
using DemLock.Parser;

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
        parser.Events.OnEntityUpdated += (sender, eventArgs) =>
        {
            //if (eventArgs.Entity is CCitadelPlayerPawn player)
            //{
            //    Console.WriteLine($"{player.m_flSimulationTime}==>{player.m_iHealth}/{player.m_iMaxHealth}");
            //}
        };
        string classDefPath = @"E:\Projects\DeadlockToolbox\DemLock\DemLock.Entities\ClassDefinitions";
        
        parser.DumpClassDefinitions("C:\\tmp\\DeadlockDemos\\534870CS.dem", classDefPath);
        //parser.ProcessDemo("C:\\tmp\\DeadlockDemos\\534870CS.dem");
        Console.WriteLine($"Processed demo in {sw.Elapsed.TotalSeconds} seconds");
        // 14011DEMLOCK.dem
        
        //parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");       
    }
}