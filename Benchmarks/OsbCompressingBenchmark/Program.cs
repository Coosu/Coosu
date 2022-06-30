#nullable enable
extern alias localbuild;

using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using LocalCoosuNs = localbuild::Coosu;
using NugetCoosuNs = Coosu;

namespace OsbCompressingBenchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var fi = new FileInfo(@"Rin - Prism Magical (DJ SHARPNEL hardrave remix) (regenz).osb");
            var fi = new FileInfo(@"Denkishiki Karen Ongaku Shuudan - Gareki no Yume (Dored).osb");
            //var fi = new FileInfo(@"test.osb");
            //var fi = new FileInfo(@"minmin - O.M.E.N. (Mafiamaster).osb");
            if (!fi.Exists)
                throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
            Environment.SetEnvironmentVariable("test_osb_path", fi.FullName);

            var layer = LocalCoosuNs.Storyboard.Layer.ParseFromFileAsync(fi.FullName).Result;
            var compressor = new LocalCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(layer,
                new LocalCoosuNs.Storyboard.Extensions.Optimizing.CompressOptions()
                {
                    ThreadCount = 1
                });
            compressor.CompressAsync().Wait();
            layer.SaveScriptAsync("new.osb").Wait();

            var osu2 = NugetCoosuNs.Storyboard.Layer.ParseFromFileAsync(fi.FullName).Result;
            var compressor2 = new NugetCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(osu2);
            compressor2.CompressAsync().Wait();
            osu2.SaveScriptAsync("old.osb").Wait();
            var summary = BenchmarkRunner.Run<CompressingTask>(/*config*/);
        }

        [SimpleJob(RuntimeMoniker.Net48)]
        [SimpleJob(RuntimeMoniker.NetCoreApp31)]
        [SimpleJob(RuntimeMoniker.Net60)]
        [MemoryDiagnoser]
        [Orderer(SummaryOrderPolicy.FastestToSlowest)]
        [MarkdownExporter]
        public class CompressingTask
        {
            private readonly string _path;
            private readonly LocalCoosuNs.Storyboard.Layer _osuLocal;
            private readonly NugetCoosuNs.Storyboard.Layer _osuNuget;

            public CompressingTask()
            {
                var path = Environment.GetEnvironmentVariable("test_osb_path");
                _path = path;
                Console.WriteLine(_path);
                _osuLocal = LocalCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path).Result;
                _osuNuget = NugetCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path).Result;
            }

            [Benchmark(Baseline = true)]
            public async Task<object?> CoosuLatest_Compress()
            {
                var compressor = new LocalCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(_osuLocal,
                    new LocalCoosuNs.Storyboard.Extensions.Optimizing.CompressOptions()
                    {
                        ThreadCount = 1
                    });
                compressor.CompressAsync().Wait();
                return null;
            }

            [Benchmark]
            public async Task<object?> CoosuOld_Compress()
            {
                var compressor2 = new NugetCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(_osuNuget, new NugetCoosuNs.Storyboard.Extensions.Optimizing.CompressOptions()
                {
                    ThreadCount = 1
                });
                compressor2.CompressAsync().Wait();
                return null;
            }

            [Benchmark]
            public async Task<object?> CoosuLatest_ParseAndCompress()
            {
                var osuLocal = await LocalCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path);
                var compressor = new LocalCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(osuLocal,
                    new LocalCoosuNs.Storyboard.Extensions.Optimizing.CompressOptions()
                    {
                        ThreadCount = 1
                    });
                await compressor.CompressAsync();
                return null;
            }

            [Benchmark]
            public async Task<object?> CoosuOld_ParseAndCompress()
            {
                var osuNuget = await NugetCoosuNs.Storyboard.Layer.ParseFromFileAsync(_path);
                var compressor2 = new NugetCoosuNs.Storyboard.Extensions.Optimizing.SpriteCompressor(osuNuget, new NugetCoosuNs.Storyboard.Extensions.Optimizing.CompressOptions()
                {
                    ThreadCount = 1
                });
                await compressor2.CompressAsync();
                return null;
            }
        }
    }
}
