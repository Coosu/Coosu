#nullable enable
extern alias localbuild;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using LocalCoosuNs = localbuild::Coosu;
using NugetCoosuNs = Coosu;

namespace WritingOsuBenchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            var fi = new FileInfo(@"test.osu");
            //var fi = new FileInfo(@"F:\milkitic\Songs\1376486 Risshuu feat. Choko - Take\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu");
            if (!fi.Exists)
                throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
            Environment.SetEnvironmentVariable("test_osu_path", fi.FullName);
            var osu = LocalCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"test.osu").Result;
            osu.WriteOsuFile("new.osu");
            //var sb = new StringBuilder();
            //for (int i = 0; i < 5000; i++)
            //{
            //    osu.WriteOsuFile("new.osu");
            //    Console.WriteLine(i);
            //}

            //return;

            var osu2 = NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(@"test.osu").Result;
            osu2.WriteOsuFile("old.osu");

            var summary = BenchmarkRunner.Run<WritingTask>(/*config*/);
        }

        [SimpleJob(RuntimeMoniker.Net48)]
        [SimpleJob(RuntimeMoniker.NetCoreApp31)]
        [SimpleJob(RuntimeMoniker.Net60)]
        [MemoryDiagnoser]
        [Orderer(SummaryOrderPolicy.FastestToSlowest)]
        [MarkdownExporter]
        public class WritingTask
        {
            private readonly string _path;
            private readonly LocalCoosuNs.Beatmap.LocalOsuFile _latest;
            private readonly NugetCoosuNs.Beatmap.LocalOsuFile _nuget;
            private readonly Beatmap _osuParsers;

            public WritingTask()
            {
                var path = Environment.GetEnvironmentVariable("test_osu_path");
                _path = path;
                Console.WriteLine(_path);
                _latest = LocalCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(_path).Result;
                _nuget = NugetCoosuNs.Beatmap.OsuFile.ReadFromFileAsync(_path).Result;
                _osuParsers = OsuParsers.Decoders.BeatmapDecoder.Decode(_path);
            }

            [Benchmark(Baseline = true)]
            public async Task<object?> CoosuLatest_Write()
            {
                _latest.WriteOsuFile("CoosuLatest_Write.txt");
                return null;
            }

            [Benchmark]
            public async Task<object?> OsuParsers_Write()
            {
                _osuParsers.Save("OsuParsers_Write.txt");
                return null;
            }

            [Benchmark]
            public async Task<object?> CoosuV2_1_0_Write()
            {
                _nuget.WriteOsuFile("CoosuV2_1_0_Write.txt");
                return null;
            }
        }
    }
}
