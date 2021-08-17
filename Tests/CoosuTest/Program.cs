using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions;
using Coosu.Storyboard.Extensions.Optimizing;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var layer = new Layer();
            var sprite = layer.CreateSprite("sb");
            sprite.Fade(0, 0, 300, 0, 1);
            sprite.MoveX(EasingType.BackIn, 0, 300, 320, 240);
            sprite.MoveY(new PowerEase() { Power = 2 }, 100, 300, 320, 240);
            sprite.MoveXBy(0, 300, 100);
            sprite.VectorBy(new ElasticEase
            {
                EasingMode = EasingMode.EaseOut,
                Oscillations = 5,
                Springiness = 3d * 2
            }, 0, 300, 1.2, 1);
            await layer.WriteScriptAsync(Console.Out);
            Console.WriteLine("==================");
            var compressor = new SpriteCompressor(layer);
            await compressor.CompressAsync();
            await layer.WriteScriptAsync(Console.Out);
            return;
            var text = await File.ReadAllTextAsync(
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
        }
    }
}
