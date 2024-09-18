using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DemLock.Parser;

namespace DemLock.Tests;

public class BenchmarkTests
{
    private readonly DemoParserConfig config;
    private readonly DemoParser parser;
    private readonly string demosPath;

    public BenchmarkTests()
    {
        config = new DemoParserConfig
        {
            LogMessageReads = false,
            LogReadFrames = false
        };
        parser = new DemoParser(config);
        demosPath = Path.Combine(Directory.GetCurrentDirectory(), @"TestData");
    }

    [Benchmark]
    public void FullMatch()
    {
        // Arrange
        parser.Events.OnEntityUpdated += static (sender, eventArgs) => {};

        // Act
        parser.ProcessDemo(Path.Combine(demosPath, "benchmark_20240825_361e4b053.dem"));
    }
}

public class Program
{
    public static void Main()
    {
        _ = BenchmarkRunner.Run<BenchmarkTests>();
    }
}
