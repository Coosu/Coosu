// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Database;
using Coosu.Database.Internal;
using Coosu.Database.Serialization;
using osu.Shared.Serialization;
using OsuParsers.Decoders;

//var ok = DatabaseDecoder.DecodeOsu(@"E:\Games\osu!\osu!.db");
//ok.BeatmapCount = 3000;
//ok.Beatmaps = ok.Beatmaps.Take(3000).ToList();
//ok.Save(@"D:\osu!small.db");

var summary = BenchmarkRunner.Run<TestTask>(/*config*/);

//[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60/*, baseline: true*/)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class TestTask
{
    [Benchmark(Baseline = true)]
    public object Coosu()
    {
        return OsuDb.ReadFromFile(@"D:\osu!small.db");
    }

    [Benchmark]
    public object Holly()
    {
        return osu_database_reader.BinaryFiles.OsuDb.Read(@"D:\osu!small.db");
    }

    [Benchmark]
    public object OsuParsers()
    {
        return DatabaseDecoder.DecodeOsu(@"D:\osu!small.db");
    }
}