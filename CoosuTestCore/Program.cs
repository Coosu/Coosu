using System;
using System.IO;
using Coosu.Beatmap;

namespace CoosuTestCore
{
    class Program
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

            var dir = fi.Directory;
            var pathWithCombine = Path.Combine(dir.FullName, fi.Name);
            Console.WriteLine(pathWithCombine);

        }
    }
}
