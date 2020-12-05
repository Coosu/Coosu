using System;
using System.IO;
using Coosu.Beatmap;
using Coosu.Storyboard.Camera;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Management;

namespace CoosuTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var o = OsuFile.ReadFromFileAsync(@"D:\ValveUnhandledExceptionFilter.txt").Result;

            var fileName = "op.avi";
            var fi = new FileInfo(@"F:\项目\GitHub\CoosuLocal\folder \" + fileName);
            if (fi.Exists)
            {
                Console.WriteLine(fi.FullName);
            }

            var directDirName = fi.DirectoryName;
            var dir = fi.Directory;
            var dirFullName = dir.FullName;
            var dirName = dir.Name;
            var toStringResult = dir.ToString();
            var pathWithCombine = Path.Combine(dirFullName, fi.Name);
            var pathWithCombine2 = Path.Combine(toStringResult, fi.Name);
            Console.WriteLine(pathWithCombine);
            Console.WriteLine(pathWithCombine2);

            var sb = OsuFile.ReadFromFileAsync(
                @"D:\Games\osu!\Songs\EastNewSound - Gensoukyou Matsuribayashi (Aki)\EastNewSound - Gensoukyou Matsuribayashi (Aki) (yf_bmp) [test].osu").Result;
            foreach (var rawHitObject in sb.HitObjects.HitObjectList)
            {
                var ticks = rawHitObject.SliderInfo.Ticks;
                var slids = rawHitObject.SliderInfo.BallTrail;
            }
        }
    }
}
