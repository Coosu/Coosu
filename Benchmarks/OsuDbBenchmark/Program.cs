// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Database;
using Coosu.Database.Serialization;
using OsuDbBenchmark;
using OsuParsers.Decoders;

//var ok = DatabaseDecoder.DecodeOsu(@"E:\Games\osu!\osu!.db");
//ok.BeatmapCount = 3000;
//ok.Beatmaps = ok.Beatmaps.Take(3000).ToList();
//ok.Save(@"D:\osu!small.db");

var summary = BenchmarkRunner.Run<OsuDbReadingTask>(/*config*/);

[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60/*, baseline: true*/)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class OsuDbReadingTask
{
    private readonly byte[] _allBytes;

    public OsuDbReadingTask()
    {
        _allBytes = File.ReadAllBytes(@"D:\osu!small.db");
        //_allBytes = File.ReadAllBytes(@"E:\Games\osu!\osu!.db");
    }

    [Benchmark]
    public object CoosuCustom()
    {
        using var ms = new MemoryStream(_allBytes);
        using var reader = new OsuDbReader(ms);
        return reader.EnumerateTinyBeatmaps().ToArray();
    }

    [Benchmark(Baseline = true)]
    public object CoosuDefault()
    {
        using var ms = new MemoryStream(_allBytes);
        return OsuDb.ReadFromStream(ms);
    }

    [Benchmark]
    public object Holly_osu_database_reader()
    {
        using var ms = new MemoryStream(_allBytes);
        return osu_database_reader.BinaryFiles.OsuDb.Read(ms);
    }

    [Benchmark]
    public object OsuParsers()
    {
        using var ms = new MemoryStream(_allBytes);
        return DatabaseDecoder.DecodeOsu(ms);
    }
}