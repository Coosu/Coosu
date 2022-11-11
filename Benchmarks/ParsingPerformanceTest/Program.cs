#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Beatmap;
using OsuParsers.Decoders;

namespace ParsingPerformanceTest;

class Program
{
    static void Main(string[] args)
    {
        var fi = new FileInfo(@"test.osu");
        //var fi = new FileInfo(@"F:\milkitic\Songs\1376486 Risshuu feat. Choko - Take\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu");
        if (!fi.Exists)
            throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
        Environment.SetEnvironmentVariable("test_osu_path", fi.FullName);
        var osu = OsuFile.ReadFromFileAsync(fi.FullName).Result;
        osu.Save("new.osu");
        //var arr = new bool[15000]
        //    .AsParallel()
        //    .WithDegreeOfParallelism(Environment.ProcessorCount)
        //    .Select(k =>
        //    {
        //        return LocalCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"test.osu").Result;
        //    })
        //    .ToArray();
        //return;

        //list.Clear();
        //GC.Collect();
        //for (int i = 0; i < 100; i++)
        //{
        //    var osu = NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"test.osu").Result;
        //    list.Add(osu);
        //}

        //var osu2 = NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"test.osu").Result;
        //if (!osu.ReadSuccess) throw osu2.ReadException;
        //var xp = new CsvExporter(CsvSeparator.CurrentCulture);
        //var job = Job.Default
        //    .WithRuntime(CoreRuntime.Core31)
        //    .WithRuntime(ClrRuntime.Net48)
        //    .WithRuntime(CoreRuntime.Core60);
        //var config = DefaultConfig.Instance
        //    .AddJob(job)
        //    .AddExporter(xp)
        //    .AddDiagnoser(MemoryDiagnoser.Default);

        //var summary2 = BenchmarkRunner.Run<SavingTask>(ManualConfig
        //    .Create(new DebugBuildConfig())
        //    .AddExporter(xp)
        //    .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
        //);

        var summary = BenchmarkRunner.Run<ReadingTask>(/*config*/);
    }
}

[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MarkdownExporter]
public class ReadingTask
{
    private readonly string _path;

    public ReadingTask()
    {
        var path = Environment.GetEnvironmentVariable("test_osu_path");
        _path = path;
        Console.WriteLine(_path);
    }

    [Benchmark(Baseline = true)]
    public async Task<object?> CoosuLatest_Beatmap()
    {
        var osu = await OsuFile.ReadFromFileAsync(_path);
        return osu;
    }

    [Benchmark]
    public async Task<object?> OsuParsers_Beatmap()
    {
        var osu = BeatmapDecoder.Decode(_path);
        return osu;
    }
}