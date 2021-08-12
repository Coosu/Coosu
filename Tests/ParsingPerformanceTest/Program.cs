using System;
using System.IO;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Coosu.Storyboard;
using Coosu.Storyboard.Management;

namespace ParsingPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ParsingTask>(new DebugBuildConfig());
        }
    }

    [MemoryDiagnoser]
    public class ParsingTask
    {
        private readonly string _text;

        public ParsingTask()
        {
            _text = File.ReadAllText(
                @"D:\GitHub\ReOsuStoryboardPlayer\ReOsuStoryboardPlayer.Core.UnitTest\TestData\IOSYS feat. 3L - Miracle-Hinacle (_lolipop).osb");
        }

        [Benchmark]
        public ElementGroup OldCoosu() => ElementGroup.Parse(new StringReader(_text));

        //[Benchmark]
        //public VirtualLayer NewCoosu() => VirtualLayer.Parse(new StringReader(_text));
    }
}
