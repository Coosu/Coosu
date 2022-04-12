using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Database;
using Coosu.Database.Serialization;
using Coosu.Storyboard;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions.Computing;
using Coosu.Storyboard.Extensions.Optimizing;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var list = new List<ValueTuple<string?, object?, string, string>>(8294626);
            var osuDb = OsuDb.ReadFromFile(@"D:\osu!small.db");

            using (var ok = new OsuDbReader(@"E:\Games\osu!\osu!.db"))
            {
                var allBeatmaps = ok.EnumerateBeatmaps().ToList();
             
                //while (ok.Read())
                //{
                //    var name = ok.Name;
                //    var value = ok.Value;
                //    var nodeType = ok.NodeType;
                //    var dataType = ok.DataType;
                //    var valueTuple = (name, value, nodeType.ToString(), dataType.ToString());
                //    list.Add(valueTuple);
                //    Console.WriteLine(valueTuple);
                //}
            }

            var dir = new OsuDirectory(@"E:\Games\osu!\Songs\take yf");
            await dir.InitializeAsync();
            var osuFile = dir.OsuFiles.FirstOrDefault(k => k.Metadata.Version?.Contains("~") == true);
            var slider1 = osuFile.HitObjects.HitObjectList.FirstOrDefault(k => k.ObjectType == HitObjectType.Slider);
            var slider = osuFile.HitObjects.HitObjectList.First(k => k.Offset == 48819);
            var gg = slider.SliderInfo.GetSliderSlides();


            var allHs = dir.OsuFiles.AsParallel().Select(k =>
            {
                return dir.GetHitsoundNodesAsync(k).Result;
            }).SelectMany(k => k).ToArray();

            var hitsounds = await dir.GetHitsoundNodesAsync(osuFile);

            var text1 = File.ReadAllText(
                "D:\\GitHub\\ReOsuStoryboardPlayer\\ReOsuStoryboardPlayer.Core.UnitTest\\TestData\\" +
                "test.osb");
            var sw = Stopwatch.StartNew();
            var layer1 = await Layer.ParseAsyncTextAsync(text1);
            Console.WriteLine("parse: " + sw.Elapsed);
            var s1 = await layer1.ToScriptStringAsync();
            var c = new SpriteCompressor(layer1);
            sw.Restart();
            await c.CompressAsync();
            Console.WriteLine("compress: " + sw.Elapsed);
            using (var swriter =
                new StreamWriter(
                    "D:\\GitHub\\ReOsuStoryboardPlayer\\ReOsuStoryboardPlayer.Core.UnitTest\\TestData\\test1.osb"))
            {
                await layer1.WriteFullScriptAsync(swriter);
            }
            //var s2 = await layer1.ToScriptStringAsync();
            //var len1 = s1.Length;
            //var len2 = s2.Length;
            //var percent = (len2 / (double)len1).ToString("P2");
            //Console.WriteLine(percent);

            var layer = new Layer();
            layer.Camera2.RotateBy(new BackEase() { Amplitude = 2 }, startTime: 0, endTime: 500, (float)(Math.PI / 2));
            layer.Camera2.MoveBy(startTime: 0, endTime: 500, x: 300, y: 30);
            layer.Camera2.StandardizeEvents();
            //            layer = Layer.ParseFromText(@"
            //Sprite,Foreground,Centre,""sb\cg\waifu.png"",320,240
            // MX,0,4960,4992,342.24,344.448
            // MX,0,4992,5000,344.448,345
            // MX,0,5000,5024,345,345.12
            // MX,0,5024,5056,345.12,345.28
            // MX,0,5056,5088,345.28,345.44
            //");
            var r = new Random();
            Func<double, double, double> Random = (x, y) => { return r.NextDouble() * (y - x) + x; };
            for (int i = 0; i < 1; i++)
            {
                var sprite = layer.CreateSprite("one_plane.jpg");
                sprite.MoveXBy(new PowerEase()
                {
                    Power = 30,
                    EasingMode = EasingMode.EaseOut
                }, 0, 300, 300);

                sprite.MoveXBy(new PowerEase()
                {
                    Power = 30,
                    EasingMode = EasingMode.EaseInOut
                }, 100, 400, 300);
                //sprite.MoveBy(new PowerEase()
                //{
                //    Power = 10,
                //    EasingMode = EasingMode.EaseInOut
                //}, 0, 3000 + Random(-1000, 1000), 400 + Random(-50, 50), 150 + Random(-50, 50));
                sprite.MoveXBy(0, 400, 100);
            }

            foreach (var sprite in layer)
            {

            }

            await layer.WriteScriptAsync(Console.Out);
            Console.WriteLine("==================");
            string? preP = null;
            var compressor = new SpriteCompressor(layer, new CompressOptions
            {
                DiscretizingAccuracy = 0,
                ThreadCount = Environment.ProcessorCount + 1
            });

            compressor.ErrorOccured += (s, e) =>
            {
                //Console.WriteLine(e.SourceSprite.GetHeaderString() + ": " + e.Message);
                //Console.ReadLine();
            };
            compressor.ProgressChanged += (s, e) =>
            {
                //var p = (e.Progress / (double)e.TotalCount).ToString("P0");
                //if (preP == p) return;
                //Console.WriteLine(p);
                //preP = p;
            };
            var canceled = await compressor.CompressAsync();
            await layer.WriteScriptAsync(Console.Out);
            return;
            var text = File.ReadAllText(
                "C:\\Users\\milkitic\\Downloads\\" +
                "1037741 Denkishiki Karen Ongaku Shuudan - Gareki no Yume\\" +
                "Denkishiki Karen Ongaku Shuudan - Gareki no Yume (Dored).osb"
            );
            await OutputNewOsb(text);
            //await OutputOldOsb(text);
        }

        private static async Task OutputNewOsb(string text)
        {
            var layer = await Layer.ParseAsyncTextAsync(text);
            var g = new SpriteCompressor(layer);
            //g.SituationFound += (s, e) => { Console.WriteLine(e.Message); };
            g.ErrorOccured += (s, e) => { Console.WriteLine(e.Message); };
            await g.CompressAsync();
            await layer.SaveScriptAsync(Path.Combine(Environment.CurrentDirectory, "output_new.osb"));
        }

        private static async Task OutputOldOsb(string text)
        {
#if NET5_0_OR_GREATER
            var ctx = new System.Runtime.Loader.AssemblyLoadContext("old", false);
            var path = Path.Combine(Environment.CurrentDirectory, @"..\..\..\V1.0.0.0\Coosu.Storyboard.dll");
            var asm = ctx.LoadFromAssemblyPath(path);
            //ctx.Unload();
            var egT = asm.GetType("Coosu.Storyboard.Management.ElementGroup");
            var method = egT?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            var eg = method?.Invoke(null, new object?[] { text });
            var type = eg?.GetType();
            var compressorT = asm.GetType("Coosu.Storyboard.Management.ElementCompressor");
            var compressor = (dynamic)Activator.CreateInstance(compressorT, eg);
            var task1 = (Task)compressor.CompressAsync();
            await task1;
            var methodSave = type?.GetMethod("SaveOsbFileAsync");
            var task = (Task)methodSave.Invoke(eg,
                new object?[] { Path.Combine(Environment.CurrentDirectory, "output_old.osb") });
            await task;
#endif
        }
    }
}
