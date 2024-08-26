using System.Text.Json;
using System.Text.Json.Serialization;
using DemLock.Parser;

namespace DemLock;

class Program
{
    static void Main(string[] args)
    {
        DemoParser parser = new DemoParser();

        parser.Events.OnFileHeader += (sender, eventArgs) =>
        {
            Console.WriteLine(JsonSerializer.Serialize(eventArgs, new JsonSerializerOptions() { WriteIndented = true }));
        };
        
        
        parser.ProcessDemo("C:/tmp/DeadlockDemos/534870CS.dem");
    }
}