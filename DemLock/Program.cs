using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        //parser.Events.OnEntityUpdated += (sender, eventArgs) =>
        //{
        //    if (eventArgs.EntityType == "CCitadelPlayerPawn")
        //    {
        //        var v = eventArgs.Updates.Where(x => x.FieldName == "m_iHealth").FirstOrDefault();
        //        Console.WriteLine($"[{eventArgs.Tick}] {v}");
        //    }
        //};


        new EntityBindingBuilder()
            .Entity("CCitadelPlayerPawn")
            .WithFields(fieldBindings =>
            {
                fieldBindings.Field("CBodyComponent").Field("m_cellX").As<uint>();
                fieldBindings.Field("m_iHealth").As<int>();
                fieldBindings.Field("m_iHealthMax").As<int>("MaxHealth");
            })
            .Entity("CCitadelPlayerController")
            .WithFields(fieldBindings =>
            {
                fieldBindings.Field("m_steamID").As<ulong>("SteamID");
            }).OnUpdate(data =>
            {
                Console.WriteLine($"{data["SteamID"]}");
                Console.WriteLine($"{data["m_iHealth"]}/{data["MaxHealth"]}");
            });


        new EntityBindingBuilder<CCitadelPlayerController>()
            .Entity("CCitadelPlayerPawn")
            .WithFields(fieldBindings =>
            {
                fieldBindings.Field("CBodyComponent").Field("m_cellX");
                fieldBindings.Field("m_iHealth").As<int>().MapTo(player => player.Health);
                fieldBindings.Field("m_iHealthMax").As<int>("MaxHealth").MapTo(player => player.MaxHealth);
            })
            .Entity("CCitadelPlayerController")
            .WithFields(fieldBindings =>
            {
                fieldBindings.Field("m_steamID").As<ulong>("SteamID").MapTo(player => player.SteamID);
            }).OnUpdate(data =>
            {
                Console.WriteLine($"{data.SteamID}");
                Console.WriteLine($"{data.Health}/{data.MaxHealth}");
            });
        
        parser.BindEntity("CCitadelPlayerPawn", fields =>
        {
            foreach (var v in fields.Where(x=>x.FieldName == "m_iHealth"))
            {
                Console.WriteLine(v);
            }
        });
        

        parser.ProcessDemo("C:\\tmp\\DeadlockDemos\\534870CS.dem");
        
        Console.WriteLine($"Processed demo in {sw.Elapsed.TotalSeconds} seconds");
        // 14011DEMLOCK.dem
        
        //parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");
    }
}