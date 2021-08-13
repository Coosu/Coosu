using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Optimizing;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = await File.ReadAllTextAsync(
                @"D:\GitHub\ReOsuStoryboardPlayer\ReOsuStoryboardPlayer.Core.UnitTest\TestData\Denkishiki Karen Ongaku Shuudan - Gareki no Yume (Dored).osb");
            //var text = await File.ReadAllTextAsync(
            //    @"C:\Users\milkitic\Desktop\optimizer not well\huge timing.txt");
            //var text = await File.ReadAllTextAsync(
            //    @"C:\Users\milkitic\Desktop\optimizer not well\should not del first F.txt");
            //var text = await File.ReadAllTextAsync(
            //    @"C:\Users\milkitic\Desktop\optimizer not well\wtf.txt");
            await OutputNewOsb(text);
            await OutputOldOsb(text);
        }

        private static async Task OutputNewOsb(string text)
        {
            var layer = await VirtualLayer.ParseAsyncTextAsync(text);
            var g = new SpriteCompressor(layer);
            g.ErrorOccured = (s, e) =>
            {
                Console.WriteLine(e.Message);
            };
            g.SituationFound = (s, e) =>
            {
                Console.WriteLine(e.Message);
            };
            await g.CompressAsync();
            await layer.SaveScriptAsync(Path.Combine(Environment.CurrentDirectory, "output_new.osb"));
        }

        private static async Task OutputOldOsb(string text)
        {
            var ctx = new System.Runtime.Loader.AssemblyLoadContext("old", false);
            var asm = ctx.LoadFromAssemblyPath(@"C:\Users\milkitic\Desktop\net472\netstandard2.0\Coosu.Storyboard.dll");
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
