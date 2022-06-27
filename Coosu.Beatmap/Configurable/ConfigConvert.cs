using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Configurable
{
    public static class ConfigConvert
    {

        public static T DeserializeObject<T>(string value, ReadOptions options) where T : Config
        {
            using var sw = new StringReader(value);
            return DeserializeObject<T>(sw, options);
        }

        public static T DeserializeObject<T>(TextReader reader, ReadOptions options) where T : Config
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

            var currentLine = reader.ReadLine();
            while (currentLine != null)
            {
                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    currentLine = reader.ReadLine();
                    continue;
                }

                if (MatchedSection(currentLine, out var sectionName))
                {
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
                            config.HandleCustom(currentLine);
                        }
                    }
                }
                else
                {
                    if (!isSkippingSection)
                    {
                        if (currentSection != null)
                        {
                            currentSection.Match(currentLine);
                        }
                        else
                        {
                            config.HandleCustom(currentLine);
                        }
                    }
                }

                currentLine = reader.ReadLine();
            }

            config.OnDeserialized();
            return config;
        }

        public static IReadOnlyDictionary<string, ReflectInfo>? GetSectionsOfType<T>()
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

        private static bool MatchedSection(string line, out string sectionName)
        {
            if (line[0] == '[' && line[line.Length - 1] == ']')
            {
                sectionName = line.Substring(1, line.Length - 2);
                return true;
            }

            sectionName = null!;
            return false;
        }

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
}