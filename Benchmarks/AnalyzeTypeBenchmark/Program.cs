#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Coosu.Beatmap;
using Coosu.Beatmap.Configurable;

namespace AnalyzeTypeBenchmark;

internal class Program
{
    static void Main(string[] args)
    {
        //var list = ConfigConvert.AnalyzeType<OsuFile>();
        //var list2 = ConfigConvertOld.AnalyzeType<OsuFile>();
        //if (list.Count != list2.Count) throw new Exception();
        //for (var i = 0; i < list2.Count; i++)
        //{
        //    var reflectInfo1 = list[i];
        //    var reflectInfo2 = list2[i];
        //    if (!reflectInfo2.Equals(reflectInfo1)) throw new Exception();
        //}
        var summary = BenchmarkRunner.Run<ReflectionTask>(/*config*/);
    }
}

[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ReflectionTask
{
    [Benchmark(/*Baseline = true*/)]
    public async Task<object?> NewRead()
    {
        var list = ConfigConvert.GetSectionsOfType<OsuFile>();
        return list;
    }

    [Benchmark]
    public async Task<object?> OldRead()
    {
        var list = ConfigConvertOld.AnalyzeType<OsuFile>();
        return list;
    }
}

public static class ConfigConvertOld
{
    public static T DeserializeObject<T>(string value, bool sequential = false) where T : Config
    {
        using StringReader sw = new StringReader(value);
        return DeserializeObject<T>(sw);
    }

    public static T DeserializeObject<T>(TextReader reader,
        Action<ReadOptions>? readOptionFactory = null,
        bool sequential = false) where T : Config
    {
        var reflectInfos = AnalyzeType<T>();
        var type = typeof(T);
        var instance = (T)Activator.CreateInstance(type, true);
        var line = reader.ReadLine();

        Section? currentSection = null;
        bool skippingSection = false;
        var options = new ReadOptions();
        readOptionFactory?.Invoke(options);
        instance.Options = options;
        var list = new List<string>(options.Include);

        while (line != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                line = reader.ReadLine();
                continue;
            }

            if (MatchedSection(line, out var sectionName))
            {
                if (options.IncludeMode == true && list.Count == 0)
                {
                    break;
                }

                if (options.IncludeMode == null)
                {
                    skippingSection = false;
                }
                else if (options.IncludeMode == true && !options.Include.Contains(sectionName))
                {
                    skippingSection = true;
                    list.Remove(sectionName);
                }
                else if (options.IncludeMode == false && options.Exclude.Contains(sectionName))
                {
                    skippingSection = true;
                }
                else
                {
                    skippingSection = false;
                }

                var matched = reflectInfos.SingleOrDefault(k => k.Name == sectionName);
                if (matched != null)
                {
                    if (!skippingSection)
                    {
                        var constructors = matched.Type.GetConstructor(new[] { type });
                        if (constructors != null)
                            currentSection = Activator.CreateInstance(matched.Type, instance) as Section;
                        else
                            currentSection = Activator.CreateInstance(matched.Type) as Section;
                        matched.PropertyInfo.SetValue(instance, currentSection);
                    }
                }
                else
                {
                    instance.HandleCustom(line);
                }
            }
            else
            {
                if (!skippingSection)
                {
                    if (currentSection != null)
                        currentSection.Match(line);
                    else
                    {
                        instance.HandleCustom(line);
                    }
                }
            }

            line = reader.ReadLine();
        }

        return instance;
    }

    private static bool MatchedSection(string line, out string sectionName)
    {
        if (line.StartsWith("[") && line.EndsWith("]"))
        {
            sectionName = line.Substring(1, line.Length - 2);
            return true;
        }

        sectionName = null;
        return false;
    }

    public static List<ReflectInfo> AnalyzeType<T>()
    {
        var reflectInfos = new List<ReflectInfo>();
        var mainType = typeof(T);

        if (mainType.IsSubclassOf(typeof(Config)))
        {
            var privateProp = mainType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            var publicProp = mainType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in privateProp)
            {
                AddInfo(info, reflectInfos, false);
            }

            foreach (var info in publicProp)
            {
                AddInfo(info, reflectInfos, true);
            }
        }

        return reflectInfos;
    }

    private static void AddInfo(PropertyInfo info, List<ReflectInfo> reflectInfos, bool isPublic)
    {
        var attributes = info.GetCustomAttributes().Union(
            info.PropertyType.GetCustomAttributes(),
            new AttributeTypeComparer()
        ).ToArray();
        bool isDefined = GetProperties(attributes,
            out var ignored,
            out var propAttr);
        if (ignored)
        {
            return;
        }

        if (!isDefined && !isPublic)
        {
            return;
        }

        var propType = info.PropertyType;
        if (propType?.IsSubclassOf(typeof(Section)) != true)
        {
            return;
        }

        string? name = null;
        if (propAttr != null)
            name = propAttr.Name;

        name ??= propType.Name;

        var reflectInfo = new ReflectInfo(info, propType, name);
        reflectInfos.Add(reflectInfo);
    }

    private static bool GetProperties(IEnumerable<Attribute> attributes,
        out bool ignored,
        out SectionPropertyAttribute? propAttr)
    {
        ignored = false;
        propAttr = null;
        bool isDefined = false;
        foreach (var attribute in attributes)
        {
            if (attribute is SectionIgnoreAttribute)
            {
                ignored = true;
            }
            else if (attribute is SectionPropertyAttribute sectionPropertyAttr)
            {
                isDefined = true;
                propAttr = sectionPropertyAttr;
            }
        }

        return isDefined;
    }
}