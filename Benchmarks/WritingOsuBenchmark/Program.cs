#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Beatmap;
using Coosu.Beatmap.Configurable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OsuParsers.Beatmaps;

namespace WritingOsuBenchmark;

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

public class Program
{
    static void Main(string[] args)
    {
        //var fi = new FileInfo(@"test.osu");
        var fi = new FileInfo(@"F:\milkitic\Songs\1376486 Risshuu feat. Choko - Take\Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu");
        if (!fi.Exists)
            throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
        Environment.SetEnvironmentVariable("test_osu_path", fi.FullName);
        var osu = OsuFile.ReadFromFileAsync(fi.FullName).Result;
        osu.Save("new.osu");
        using (var file = File.CreateText(@"new.json"))
        {
            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new MyContractResolver(),
            };
            serializer.Converters.Add(new Vector2Converter());
            serializer.Serialize(file, osu);
        }
        //var sb = new StringBuilder();
        //for (int i = 0; i < 5000; i++)
        //{
        //    osu.WriteOsuFile("new.osu");
        //    Console.WriteLine(i);
        //}

        //return;

        var osu2 = OsuFile.ReadFromFileAsync(fi.FullName).Result;
        osu2.Save("old.osu");

        var summary = BenchmarkRunner.Run<WritingTask>(/*config*/);
    }

    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MarkdownExporter]
    public class WritingTask
    {
        private readonly string _path;
        private readonly LocalOsuFile _latest;
        private readonly Beatmap _osuParsers;
        private readonly JsonSerializer _serializer;

        public WritingTask()
        {
            var path = Environment.GetEnvironmentVariable("test_osu_path");
            _path = path;
            Console.WriteLine(_path);
            _latest = OsuFile.ReadFromFileAsync(_path).Result;
            _osuParsers = OsuParsers.Decoders.BeatmapDecoder.Decode(_path);
            _serializer = new JsonSerializer
            {
                //Formatting = Formatting.Indented,
                ContractResolver = new MyContractResolver(),
            };
            _serializer.Converters.Add(new Vector2Converter());
        }

        [Benchmark(Baseline = true)]
        public async Task<object?> CoosuLatest_Write()
        {
            _latest.Save("CoosuLatest_Write.txt");
            return null;
        }

        [Benchmark]
        public async Task<object?> JsonDotNet_Write()
        {
            using var sw = new StreamWriter(@"JsonDotNet_Write.json");
            _serializer.Serialize(sw, _latest);
            return null;
        }

        [Benchmark]
        public async Task<object?> OsuParsers_Write()
        {
            _osuParsers.Save("OsuParsers_Write.txt");
            return null;
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