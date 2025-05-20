using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Coosu.Beatmap.Internal;
using Coosu.Shared.IO;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Configurable;

public static class ConfigConvert
{
#if NET6_0_OR_GREATER
    public static T DeserializeObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        T>(string value, ReadOptions options) where T : Config
#else
    public static T DeserializeObject<T>(string value, ReadOptions options) where T : Config
#endif
    {
        using var sw = new StringReader(value);
        return DeserializeObject<T>(sw, options);
    }

#if NET6_0_OR_GREATER
    public static T DeserializeObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        T>(TextReader reader, ReadOptions options) where T : Config
#else
    public static T DeserializeObject<T>(TextReader reader, ReadOptions options) where T : Config
#endif
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        var reflectInfos = GetSectionsOfType<T>();
        var configType = typeof(T);
        var config = (T)Activator.CreateInstance(configType, true)!;

        //var options = new ReadOptions();
        //configureReadOptions?.Invoke(options);
        config.Options = options;

        if (reflectInfos == null)
            return config;
        if (options.IncludeMode == true && options.Include.Count == 0)
            return config;

        Type[] constructorParameter = { configType };
        Section? currentSection = null;
        bool isSkippingSection = false;

        using var lineReader = new EphemeralLineReader(reader);
        ReadOnlyMemory<char>? currentLineMemory;

        while ((currentLineMemory = lineReader.ReadLine()) != null)
        {
            ReadOnlySpan<char> lineSpan = currentLineMemory.Value.Span;
            if (lineSpan.IsEmpty || lineSpan.IsWhiteSpace())
            {
                continue;
            }

            if (MatchedSection(lineSpan, out var sectionNameSpan))
            {
                string sectionName = sectionNameSpan.ToString();
                if (options.IncludeMode == null)
                {
                    isSkippingSection = false;
                }
                else if (options.IncludeMode == true && !options.Include.Contains(sectionName))
                {
                    isSkippingSection = true;
                }
                else if (options.IncludeMode == false && options.Exclude.Contains(sectionName))
                {
                    isSkippingSection = true;
                }
                else
                {
                    isSkippingSection = false;
                }

                if (!isSkippingSection)
                {
                    if (reflectInfos.TryGetValue(sectionName, out var reflectInfo))
                    {
                        var constructor = reflectInfo.Type.GetConstructor(constructorParameter);
                        if (constructor != null)
                        {
                            currentSection = Activator.CreateInstance(reflectInfo.Type, config) as Section;
                        }
                        else
                        {
                            currentSection = Activator.CreateInstance(reflectInfo.Type) as Section;
                        }

                        reflectInfo.PropertyInfo.SetValue(config, currentSection);
                    }
                    else
                    {
                        currentSection = null;
                        config.HandleCustom(currentLineMemory.Value);
                    }
                }
            }
            else
            {
                if (!isSkippingSection)
                {
                    if (currentSection != null)
                    {
                        currentSection.Match(currentLineMemory.Value);
                    }
                    else
                    {
                        config.HandleCustom(currentLineMemory.Value);
                    }
                }
            }
        }

        config.OnDeserialized();
        return config;
    }

#if NET6_0_OR_GREATER
    public static IReadOnlyDictionary<string, ReflectInfo>? GetSectionsOfType<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        T>() where T : Config
#else
    public static IReadOnlyDictionary<string, ReflectInfo>? GetSectionsOfType<T>() where T : Config
#endif
    {
        var mainType = typeof(T);
        if (!mainType.IsSubclassOf(StaticTypes.Config))
            return null;

        var reflectInfos = new Dictionary<string, ReflectInfo>();
        var props = mainType
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var info in props)
        {
            AddSectionsIfPossible(info, reflectInfos);
        }

        return reflectInfos;
    }

    private static bool MatchedSection(ReadOnlySpan<char> line, out ReadOnlySpan<char> sectionNameSpan)
    {
        if (line.Length >= 2 && line[0] == '[' && line[line.Length - 1] == ']')
        {
            sectionNameSpan = line.Slice(1, line.Length - 2);
            return true;
        }

        sectionNameSpan = default;
        return false;
    }
    
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Aot", "IL2072", Justification = "The 'propType' is a derivative of 'Section' and is expected to have public constructors for Activator.CreateInstance. The ReflectInfo constructor requires its 'type' parameter (propType) to be annotated with PublicConstructors, but PropertyInfo.PropertyType does not carry this annotation.")]
#endif
    private static void AddSectionsIfPossible(PropertyInfo info, IDictionary<string, ReflectInfo> reflectInfos)
    {
        var propType = info.PropertyType;
        if (info.SetMethod == null) return;
        if (!propType.IsSubclassOf(StaticTypes.Section)) return;

        var isPublic = info.SetMethod.IsPublic;

        var ignore = info.GetCustomAttribute<SectionIgnoreAttribute>();
        if (ignore != null) return;
        ignore = info.PropertyType.GetCustomAttribute<SectionIgnoreAttribute>();
        if (ignore != null) return;

        var attr = info.GetCustomAttribute<SectionPropertyAttribute>() ??
                   propType.GetCustomAttribute<SectionPropertyAttribute>();
        if (attr == null && !isPublic) return;

        var name = attr?.Name ?? propType.Name;

        var reflectInfo = new ReflectInfo(info, propType, name);
        reflectInfos.Add(name, reflectInfo);
    }
}