#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Optimizing;

namespace OsbCompressingBenchmark;

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

        var osu2 = Layer.ParseFromFileAsync(fi.FullName).Result;
        var compressor2 = new SpriteCompressor(osu2);
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
        private readonly Layer _osuNuget;

        public CompressingTask()
        {
            var path = Environment.GetEnvironmentVariable("test_osb_path");
            _path = path;
            Console.WriteLine(_path);
            _osuNuget = Layer.ParseFromFileAsync(_path).Result;
        }


        [Benchmark]
        public async Task<object?> CoosuLatest_Compress()
        {
            var compressor2 = new SpriteCompressor(_osuNuget, k => k.ThreadCount = 1);
            compressor2.CompressAsync().Wait();
            return null;
        }

        [Benchmark]
        public async Task<object?> CoosuLatest_ParseAndCompress()
        {
            var osuNuget = await Layer.ParseFromFileAsync(_path);
            var compressor2 = new SpriteCompressor(osuNuget, k => k.ThreadCount = 1);
            await compressor2.CompressAsync();
            return null;
        }
    }
}