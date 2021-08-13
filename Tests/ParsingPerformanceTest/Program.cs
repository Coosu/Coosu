using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace ParsingPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var xp = new HtmlExporter()
            {

            };

            var summary = BenchmarkRunner.Run<ParsingTask>(ManualConfig
                .Create(new DebugBuildConfig())
                .AddExporter(xp)
            );
        }
    }

    [HtmlExporter]
    [MemoryDiagnoser]
    public class ParsingTask
    {
        private readonly string _text;
        private readonly MethodInfo? _method1;
        private readonly MethodInfo? _method2;

        public ParsingTask()
        {
            _text = File.ReadAllText(
                @"D:\GitHub\ReOsuStoryboardPlayer\ReOsuStoryboardPlayer.Core.UnitTest\TestData\IOSYS feat. 3L - Miracle-Hinacle (_lolipop).osb");
            var ctx = new System.Runtime.Loader.AssemblyLoadContext("old", false);
            var asm1 = ctx.LoadFromAssemblyPath(@"C:\Users\milkitic\Desktop\net472\netstandard2.0\Coosu.Storyboard.dll");
            var ctx2 = new System.Runtime.Loader.AssemblyLoadContext("new", false);
            var asm2 = ctx2.LoadFromAssemblyPath(@"D:\GitHub\Coosu\Coosu.Storyboard\bin\Release\netstandard2.0\Coosu.Storyboard.dll");

            var eg = asm1.GetType("Coosu.Storyboard.Management.ElementGroup");
            var eg2 = asm2.GetType("Coosu.Storyboard.VirtualLayer");
            _method1 = eg?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            _method2 = eg2?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            var ret1 = _method1?.Invoke(null, new object?[] { _text });
            var ret2 = _method2?.Invoke(null, new object?[] { _text });
        }

        [Benchmark]
        public object? OldCoosu() => _method1?.Invoke(null, new object?[] { _text });

        [Benchmark]
        public object? NewCoosu() => _method2?.Invoke(null, new object?[] { _text });
    }
}
