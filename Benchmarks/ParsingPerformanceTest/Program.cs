#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Beatmap;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;

namespace ParsingPerformanceTest;

class Program
{
    static void Main(string[] args)
    {
        var config = new ManualConfig()
            .AddExporter(MarkdownExporter.GitHub) 
            .AddLogger(ConsoleLogger.Default)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .WithOptions(ConfigOptions.DisableLogFile);
        BenchmarkRunner.Run<ReadingTask>(config);
    }
}

[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ReadingTask
{
    private string _path = null!;

    [GlobalSetup]
    public void Setup()
    {
        var path = Environment.GetEnvironmentVariable("test_osu_path");
        _path = path ?? @"test.osu";
    }

    [Benchmark(Baseline = true)]
    public async Task<object> Coosu()
    {
        var osu = await OsuFile.ReadFromFileAsync(_path);
        return osu;
    }

    [Benchmark]
    public object OsuParsers()
    {
        Beatmap beatmap = BeatmapDecoder.Decode(_path);
        return beatmap;
    }
}