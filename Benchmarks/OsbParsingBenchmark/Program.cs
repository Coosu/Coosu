#nullable enable
extern alias localbuild;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using LocalCoosuNs = localbuild::Coosu;
using NugetCoosuNs = Coosu;

namespace OsbParsingBenchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            var fi = new FileInfo(@"light.osb");
            //var fi = new FileInfo(@"test.osb");
            if (!fi.Exists)
                throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
            Environment.SetEnvironmentVariable("test_osb_path", fi.FullName);

            //var i = 0;
            //var obj = new object();
            //Enumerable.Range(0, 100).AsParallel().ForAll((a) =>
            //{
            //    var osu = LocalCoosuNs.Storyboard.Layer.ParseFromFileAsync(fi.FullName).Result;
            //    lock (obj)
            //    {
            //        i++;
            //        Console.WriteLine(i);
            //    }
            //});
            //return;

            //osu.SaveScriptAsync("new.osb").Wait();
            //var osu2 = NugetCoosuNs.Storyboard.Layer.ParseFromFileAsync(fi.FullName).Result;
            //osu2.SaveScriptAsync("old.osb").Wait();
            var summary = BenchmarkRunner.Run<ReadingTask>(/*config*/);
        }

        [SimpleJob(RuntimeMoniker.Net48)]
        [SimpleJob(RuntimeMoniker.NetCoreApp31)]
        [SimpleJob(RuntimeMoniker.Net60)]
        [MemoryDiagnoser]
        [Orderer(SummaryOrderPolicy.FastestToSlowest)]
        [MarkdownExporter]
        public class ReadingTask
        {
            private readonly string _path;

            public ReadingTask()
            {
                var path = Environment.GetEnvironmentVariable("test_osb_path");
                _path = path;
                Console.WriteLine(_path);
            }

            [Benchmark(Baseline = true)]
            public async Task<object?> CoosuLatest_Storyboard()
            {
                var osu = await LocalCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path);
                return osu;
            }

            //[Benchmark]
            //public async Task<object?> OsuParsers_Storyboard()
            //{
            //    var osu = StoryboardDecoder.Decode(_path);
            //    return osu;
            //}

            [Benchmark]
            public async Task<object?> CoosuOld_Storyboard()
            {
                var osu = await NugetCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path);
                return osu;
            }
        }
    }
}
