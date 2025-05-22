#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Beatmap;
using Coosu.Beatmap.Configurable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OsuParsers.Beatmaps;

namespace WritingOsuBenchmark;

public class Program
{
    static void Main(string[] args)
    {
        var config = new ManualConfig()
            .AddExporter(MarkdownExporter.GitHub)
            .AddLogger(ConsoleLogger.Default)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .WithOptions(ConfigOptions.DisableLogFile);
        BenchmarkRunner.Run<WritingTask>(config);
    }

    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class WritingTask
    {
        private string _path = null!;
        private LocalOsuFile _coosu = null!;
        private Beatmap _osuParsers = null!;
        private JsonSerializer _serializer = null!;

        [GlobalSetup]
        public void Setup()
        {
            var path = Environment.GetEnvironmentVariable("test_osu_path");
            _path = path ?? "test.osu";

            _coosu = OsuFile.ReadFromFileAsync(_path).Result;
            _osuParsers = global::OsuParsers.Decoders.BeatmapDecoder.Decode(_path);
            _serializer = new JsonSerializer
            {
                ContractResolver = new MyContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            _serializer.Converters.Add(new Vector2Converter());
        }

        [Benchmark(Baseline = true)]
        public object? Coosu()
        {
            _coosu.Save("CoosuLatest_Write.osu");
            return _coosu;
        }

        [Benchmark]
        public object? JsonDotNet()
        {
            using var sw = new StreamWriter(@"JsonDotNet_Write.json");
            _serializer.Serialize(sw, _coosu);
            return _serializer;
        }

        [Benchmark]
        public object? OsuParsers()
        {
            _osuParsers.Save("OsuParsers_Write.osu");
            return _osuParsers;
        }
    }
}

internal class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.X);
        writer.WriteValue(value.Y);
        writer.WriteEndArray();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class MyContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var list = new List<JsonProperty>();
        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (var i = 0; i < propertyInfos.Length; i++)
        {
            var k = propertyInfos[i];
            var ignoreAttr = k.GetCustomAttribute<SectionIgnoreAttribute>();
            if (ignoreAttr != null)
                continue;
            var propAttr = k.GetCustomAttribute<SectionPropertyAttribute>();
            if (k.GetMethod.IsPrivate && propAttr == null)
                continue;

            var property = CreateProperty(k, memberSerialization);
            property.PropertyName = propAttr?.Name ?? k.Name;
            list.Add(property);
        }

        return list;
    }
}