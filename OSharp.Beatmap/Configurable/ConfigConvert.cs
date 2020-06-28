using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OSharp.Beatmap.Configurable
{
    public static class ConfigConvert
    {
        public static T DeserializeObject<T>(string value, bool sequential = false) where T : Config
        {
            using (StringReader sw = new StringReader(value))
            {
                return DeserializeObject<T>(sw);
            }
        }

        public static T DeserializeObject<T>(TextReader reader, Action<ReadOptions> readOptionFactory = null, bool sequential = false) where T : Config
        {
            var reflectInfos = AnalyzeType<T>();
            var type = typeof(T);
            var instance = (T)Activator.CreateInstance(type, true);
            var line = reader.ReadLine();
            Section currentSection = null;
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

        private static List<ReflectInfo> AnalyzeType<T>()
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
                new AttributeComparer()
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

            ExecuteType executeType;
            //if (info.DeclaringType?.IsSubclassOf(typeof(KeyValueSection)) == true)
            //{
            //    executeType = ExecuteType.Match;
            //}
            //else if (info.DeclaringType?.IsSubclassOf(typeof(ICustomSection)) == true)
            //{
            //    executeType = ExecuteType.Match;
            //}

            executeType = ExecuteType.Match;
            string name = null;
            if (propAttr != null)
                name = propAttr.Name;

            if (name == null)
                name = propType.Name;

            var propName = propType.Name;

            var reflectInfo = new ReflectInfo(info, propType, executeType, name, attributes);
            reflectInfos.Add(reflectInfo);
        }

        private static bool GetProperties(IEnumerable<Attribute> attributes,
            out bool ignored,
            out SectionPropertyAttribute propAttr)
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
                else if (attribute is SectionPropertyAttribute)
                {
                    isDefined = true;
                    propAttr = (SectionPropertyAttribute)attribute;
                }
            }

            return isDefined;
        }
    }
}
