using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.Utils;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var layer = new Layer();
            //layer.Camera.RotateBy(startTime: 0, endTime: 500, degree: 90);
            //layer.Camera.MoveBy(startTime: 0, endTime: 500, x: 300, y: 30);

//            var layer = Layer.ParseFromText(@"
//Sprite,Foreground,Centre,""sb\cg\waifu.png"",320,240
// MX,0,-100,-64,0,18
// MX,0,-64,-32,18,34
// MX,0,-32,0,34,50
// MX,0,0,32,50,66
// MX,0,32,64,66,82
// MX,0,64,96,82,98
// MX,0,96,100,98,100
//");
            for (int i = 0; i < 1; i++)
            {
                //var sprite = (Sprite)layer.SceneObjects[0];
                var sprite = layer.CreateSprite(@"sb\cg\waifu.png");

                //sprite.MoveX(0, 60, 64, 100, 100);
                //sprite.MoveX(0, 64, 96, 100, 100);
                //sprite.MoveXBy(0, 60, 80, 100);


                sprite.MoveXBy(0, 30, 60, 100);
                sprite.MoveXBy(0, 60, 80, 100);
                sprite.MoveX(0, 0, 100, 0, 0);
                sprite.MoveXBy(0, -100, 100, 100);
                sprite.MoveXBy(0, -200, 200, 100);

                //sprite.MoveXBy(0, 0, 60, 100);
                //sprite.MoveX(0, 40, 100, 0, 100);

                //sprite.MoveXBy(0, 40, 100, 100);
                //sprite.MoveX(0, 0, 60, 0, 100);

                //sprite.MoveXBy(new QuadraticEase(), 0, 16 * 10, 100);
                //sprite.MoveX(new QuadraticEase(), 16 * 10.3, 16 * 20, 100, 300);
                //sprite.MoveXBy(new QuinticEase
                //{
                //    EasingMode = EasingMode.EaseOut
                //}, 12, 320, -100);
                //sprite.MoveX(new QuinticEase(), 320, 300, -500, -600);
            }

            //for (int i = 0; i < 1; i++)
            //{
            //    var sprite = layer.CreateSprite(@"sb\cg\waifu.png");
            //    sprite.MoveXBy(30, 270, 100);
            //    sprite.MoveX(-100, 70, 0, 400);
            //    sprite.MoveX(230, 400, 400, 800);
            //}

            await layer.WriteScriptAsync(Console.Out);
            Console.WriteLine("==================");
            string? preP = null;
            var compressor = new SpriteCompressor(layer)
            {
                ThreadCount = Environment.ProcessorCount + 1,
                ErrorOccured = (s, e) =>
                {
                    Console.WriteLine(e.SourceSprite.GetHeaderString() + ": " + e.Message);
                    //Console.ReadLine();
                },
                ProgressChanged = (s, e) =>
                {
                    var p = (e.Progress / (double)e.TotalCount).ToString("P0");
                    if (preP == p) return;
                    Console.WriteLine(p);
                    preP = p;
                }
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
            var g = new SpriteCompressor(layer)
            {
                ErrorOccured = (s, e) => { Console.WriteLine(e.Message); },
                //SituationFound = (s, e) => { Console.WriteLine(e.Message); }
            };
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
