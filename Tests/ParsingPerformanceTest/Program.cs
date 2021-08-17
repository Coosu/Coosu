using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;

namespace ParsingPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var xp = new CsvExporter(CsvSeparator.CurrentCulture)
            {

            };

            //var summary2 = BenchmarkRunner.Run<SavingTask>(ManualConfig
            //    .Create(new DebugBuildConfig())
            //    .AddExporter(xp)
            //    .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
            //);

            var summary = BenchmarkRunner.Run<ParsingTask>(ManualConfig
                .Create(new DebugBuildConfig())
                .AddExporter(xp)
                .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
            );
        }
    }

    public class SavingTask
    {
        private readonly string _text;
        private readonly MethodInfo? _method1;
        private readonly MethodInfo? _method2;
        private readonly MethodInfo _methodSave1;
        private readonly MethodInfo _methodSave2;
        private readonly object? ret1;
        private readonly object? ret2;

        public SavingTask()
        {
            _text = File.ReadAllText(
                @"D:\GitHub\ReOsuStoryboardPlayer\ReOsuStoryboardPlayer.Core.UnitTest\TestData\IOSYS feat. 3L - Miracle-Hinacle (_lolipop).osb");
            var ctx = new System.Runtime.Loader.AssemblyLoadContext("old", false);
            var asm1 = ctx.LoadFromAssemblyPath(@"D:\GitHub\Coosu\Tests\CoosuTest\V1.0.0.0\Coosu.Storyboard.dll");
            var ctx2 = new System.Runtime.Loader.AssemblyLoadContext("new", false);
            var asm2 = ctx2.LoadFromAssemblyPath(@"D:\GitHub\Coosu\Coosu.Storyboard\bin\Release\netstandard2.0\Coosu.Storyboard.dll");

            var eg = asm1.GetType("Coosu.Storyboard.Management.ElementGroup");
            var eg2 = asm2.GetType("Coosu.Storyboard.VirtualLayer");
            _method1 = eg?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            _method2 = eg2?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            ret1 = _method1?.Invoke(null, new object?[] { _text });
            ret2 = _method2?.Invoke(null, new object?[] { _text });

            var type1 = ret1?.GetType();
            _methodSave1 = type1?.GetMethod("SaveOsbFileAsync");

            var type2 = ret2?.GetType();
            _methodSave2 = type2?.GetMethod("SaveScriptAsync");
        }

        [Benchmark]
        public object? OldCoosu()
        {
            var task = (Task)_methodSave1.Invoke(ret1,
                new object?[] { Path.Combine(Environment.CurrentDirectory, "output_old.osb") });
            task.Wait();

            return null;
        }

        [Benchmark]
        public object? NewCoosu()
        {
            var task = (Task)_methodSave2.Invoke(ret2,
                new object?[] { Path.Combine(Environment.CurrentDirectory, "output_New.osb") });
            task.Wait();

            return null;
        }
    }

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
            var eg2 = asm2.GetType("Coosu.Storyboard.Layer");
            _method1 = eg?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            _method2 = eg2?.GetMethod("ParseFromText", BindingFlags.Static | BindingFlags.Public);
            var ret1 = _method1?.Invoke(null, new object?[] { _text });
            var ret2 = _method2?.Invoke(null, new object?[] { _text });
        }

        [Benchmark]
        public object? OldCoosu()
        {
            var o = _method1?.Invoke(null, new object?[] { _text });
            return o;
        }

        [Benchmark]
        public object? NewCoosu()
        {
            var o = _method2?.Invoke(null, new object?[] { _text });
            return o;
        }
    }
}
