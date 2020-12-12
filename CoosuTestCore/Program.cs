using Coosu.Storyboard.Management;
using Coosu.Storyboard.OsbX;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CoosuTestCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var sr =
                new StreamReader(@"D:\Games\osu!\Songs\406217 Chata - enn\largetest.osb");
            var sw = Stopwatch.StartNew();
            var ggg = await ElementGroup.ParseFromFileAsync(
                @"D:\Games\osu!\Songs\406217 Chata - enn\Chata - enn (EvilElvis).osb");
            var time = sw.ElapsedMilliseconds;
            Console.WriteLine(time);
            sw.Restart();
            var em = await OsbxConvert.DeserializeObjectAsync(sr);
            time = sw.ElapsedMilliseconds;
            Console.WriteLine(time);


            var osb = em.ToString();
            var osbx = await OsbxConvert.SerializeObjectAsync(em);
        }
    }
}
