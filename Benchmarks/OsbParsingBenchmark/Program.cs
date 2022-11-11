#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Storyboard;
using OsuParsers.Decoders;

namespace OsbParsingBenchmark;

public class Program
{
    static void Main(string[] args)
    {
        var fi = new FileInfo(@"UltraLight.osb");
        //var fi = new FileInfo(@"light.osb");
        //var fi = new FileInfo(@"test.osb");
        //var fi = new FileInfo(@"rrt.osb");
        if (!fi.Exists)
            throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
        Environment.SetEnvironmentVariable("test_osb_path", fi.FullName);
        //Generate();
        var osu = Layer.ParseFromFileAsync(fi.FullName).Result;
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
    [SimpleJob(RuntimeMoniker.Net70)]
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
            var osu = await Layer.ParseFromFileAsync(_path);
            return osu;
        }

        [Benchmark]
        public async Task<object?> OsuParsers_Storyboard()
        {
            var osu = StoryboardDecoder.Decode(_path);
            return osu;
        }
    }

    private static void Generate()
    {
        var layer = new Layer();
        var f = 0f;
        byte b = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var sprite = new Sprite((LayerType)i, (OriginType)j, $"{f++}.jpg", f++, f++);
                layer.AddSprite(sprite);
                sprite.Color(0, f++, f++, b++, b++, b++, b++, b++, b++);
                sprite.Move(1, f++, f++, f++, f++, f++, f++);
                sprite.Vector(2, f++, f++, f++, f++, f++, f++);
                sprite.FlipH(f++, f++);
                sprite.FlipV(f++, f++);
                sprite.Additive(f++, f++);
                sprite.StartLoop((int)f++, 5);
                sprite.MoveX(3, f++, f++, f++, f++);
                sprite.MoveY(4, f++, f++, f++, f++);
                sprite.Rotate(5, f++, f++, f++, f++);
                sprite.Scale(6, f++, f++, f++, f++);
                sprite.Fade(7, f++, f++, f++, f++);
                sprite.EndLoop();

                var animation = new Animation((LayerType)i, (OriginType)j, $"{f++}.jpg", f++, f++, (int)(f++), f++,
                    LoopType.LoopOnce);
                layer.AddSprite(animation);
                animation.Color(8, f++, f++, b++, b++, b++, b++, b++, b++);
                animation.Move(9, f++, f++, f++, f++, f++, f++);
                animation.Vector(10, f++, f++, f++, f++, f++, f++);
                animation.FlipH(f++, f++);
                animation.FlipV(f++, f++);
                animation.Additive(f++, f++);
                animation.StartLoop((int)f++, 5);
                animation.MoveX(11, f++, f++, f++, f++);
                animation.MoveY(12, f++, f++, f++, f++);
                animation.Rotate(13, f++, f++, f++, f++);
                animation.Scale(14, f++, f++, f++, f++);
                animation.Fade(15, f++, f++, f++, f++);
                animation.EndLoop();

                var animation2 = new Animation((LayerType)i, (OriginType)j, $"{f++}.jpg", f++, f++, (int)(f++), f++,
                    LoopType.LoopForever);
                layer.AddSprite(animation2);
                animation2.Color(16, f++, f++, b++, b++, b++, b++, b++, b++);
                animation2.Move(17, f++, f++, f++, f++, f++, f++);
                animation2.Vector(18, f++, f++, f++, f++, f++, f++);
                animation2.FlipH(f++, f++);
                animation2.FlipV(f++, f++);
                animation2.Additive(f++, f++);
                animation2.StartLoop((int)f++, 5);
                animation2.MoveX(19, f++, f++, f++, f++);
                animation2.MoveY(20, f++, f++, f++, f++);
                animation2.Rotate(21, f++, f++, f++, f++);
                animation2.Scale(22, f++, f++, f++, f++);
                animation2.Fade(23, f++, f++, f++, f++);
                animation2.EndLoop();
            }
        }

        using var textWriter = new StringWriter();
        layer.WriteFullScriptAsync(textWriter).Wait();
        var str = textWriter.ToString();
    }
}