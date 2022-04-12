// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Database;

var summary = BenchmarkRunner.Run<TestTask>(/*config*/);

[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class TestTask
{
    [Benchmark]
    public void Test()
    {
        using var ok = new OsuDbReader(@"E:\Games\osu!\osu!.db");
        while (ok.Read())
        {
            var name = ok.Name;
            var value = ok.Value;
            var nodeType = ok.NodeType;
            var dataType = ok.DataType;
        }
    }
}