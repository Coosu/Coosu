#nullable enable
extern alias localbuild;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using LocalCoosuNs = localbuild::Coosu;
using NugetCoosuNs = Coosu;

namespace ParsingPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var osu = LocalCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"C:\Users\milkitic\Downloads\1376486 Risshuu feat. Choko - Take [no video]\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu").Result;
            if (!osu.ReadSuccess) throw osu.ReadException;
            //var osu2 = NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"C:\Users\milkitic\Downloads\1376486 Risshuu feat. Choko - Take [no video]\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu").Result;
            //if (!osu.ReadSuccess) throw osu2.ReadException;
            //var xp = new CsvExporter(CsvSeparator.CurrentCulture);
            //var job = Job.Default
            //    .WithRuntime(CoreRuntime.Core31)
            //    .WithRuntime(ClrRuntime.Net472)
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

    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class ReadingTask
    {
        private readonly string _path;

        public ReadingTask()
        {
            _path = @"C:\Users\milkitic\Downloads\1376486 Risshuu feat. Choko - Take [no video]\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu";
        }

        [Benchmark(/*Baseline = true*/)]
        public async Task<object?> LocalCoosu()
        {
            var osu = await LocalCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(_path);
            if (!osu.ReadSuccess) throw osu.ReadException;
            return osu;
        }

        [Benchmark]
        public async Task<object?> NugetCoosu()
        {
            var osu = await NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(_path);
            if (!osu.ReadSuccess) throw osu.ReadException;
            return osu;
        }
    }
}
