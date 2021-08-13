using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Coosu.Storyboard;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = await File.ReadAllTextAsync(
                @"D:\GitHub\ReOsuStoryboardPlayer\ReOsuStoryboardPlayer.Core.UnitTest\TestData\NOMA - LOUDER MACHINE (Skystar).osb");
            await OutputOldOsb(text);
            await OutputNewOsb(text);
        }

        private static async Task OutputNewOsb(string text)
        {
            var layer = await VirtualLayer.ParseAsyncTextAsync(text);
            await layer.SaveScriptAsync(Path.Combine(Environment.CurrentDirectory, "output_new.osb"));
        }

        private static async Task OutputOldOsb(string text)
        {
            var ctx = new System.Runtime.Loader.AssemblyLoadContext("old", false);
            var asm = ctx.LoadFromAssemblyPath(@"C:\Users\milkitic\Desktop\net472\netstandard2.0\Coosu.Storyboard.dll");
            //ctx.Unload();
            var eg = asm.GetType("Coosu.Storyboard.Management.ElementGroup");
            var method = eg?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            var egOld = method?.Invoke(null, new object?[] { text });
            var type = egOld?.GetType();

            var methodSave = type?.GetMethod("SaveOsbFileAsync");
            var task = (Task)methodSave.Invoke(egOld,
                new object?[] { Path.Combine(Environment.CurrentDirectory, "output_old.osb") });
            await task;
        }
    }
}
