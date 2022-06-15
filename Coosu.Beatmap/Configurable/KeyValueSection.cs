using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Configurable
{
    public abstract class KeyValueSection : Section
    {
        [SectionIgnore]
        public Dictionary<string, string> UndefinedPairs { get; } = new();

        public KeyValueSection()
        {
            var thisType = GetType();
            var props = thisType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < props.Length; i++)
            {
                var propertyInfo = props[i];
                if (propertyInfo.SetMethod == null) continue;
                var isPublic = propertyInfo.SetMethod.IsPublic;
                if (!isPublic)
                {
                    var sectionPropertyAttr = propertyInfo.GetCustomAttribute<SectionPropertyAttribute>();
                    if (sectionPropertyAttr == null) continue;
                    PropertyInfos.Add(sectionPropertyAttr?.Name ?? propertyInfo.Name, new SectionInfo(propertyInfo)
                    {
                        Attribute = sectionPropertyAttr
                    });
                }
                else
                {
                    var sectionIgnoreAttr = propertyInfo.GetCustomAttribute<SectionIgnoreAttribute>();
                    if (sectionIgnoreAttr != null) continue;
                    var sectionPropertyAttr = propertyInfo.GetCustomAttribute<SectionPropertyAttribute>();
                    PropertyInfos.Add(sectionPropertyAttr?.Name ?? propertyInfo.Name, new SectionInfo(propertyInfo)
                    {
                        Attribute = sectionPropertyAttr,
                    });
                }
            }
        }

        public override void Match(string line)
        {
            MatchKeyValue(line, out var keySpan, out var valueSpan);
            var key = keySpan.ToString();
#if !NETCOREAPP3_1_OR_GREATER

#endif
            if (!PropertyInfos.TryGetValue(key, out var sectionInfo))
            {
                UndefinedPairs.Add(key, valueSpan.ToString());
            }
            else
            {
                var prop = sectionInfo.PropertyInfo;
                var propType = prop.GetMethod.ReturnType;
                var attr = prop.GetCustomAttribute<SectionConverterAttribute>();

                if (attr != null)
                {
                    var converter = attr.GetConverter();
                    prop.SetValue(this, converter.ReadSection(valueSpan, propType));
                }
                else if (propType.BaseType == typeof(Enum))
                {
#if NET6_0_OR_GREATER
                    prop.SetValue(this, Enum.Parse(propType, valueSpan));
#else
                    prop.SetValue(this, Enum.Parse(propType, valueSpan.ToString()));
#endif
                }
                else
                {
                    if (ValueConvert.ConvertValue(valueSpan, propType, out var converted))
                    {
                        prop.SetValue(this, converted);
                    }
                    else
                    {
                        throw new MissingMethodException($"Can not convert {{{valueSpan.ToString()}}} to type {propType}.");
                    }
                }
            }
        }

        protected void MatchKeyValue(string line, out ReadOnlySpan<char> keySpan, out ReadOnlySpan<char> valueSpan)
        {
            int index = MatchFlag(line);
            if (index == -1)
                throw new Exception($"Unknown Key-Value: {line}");
            keySpan = line.AsSpan(0, index);
            if (TrimPairs)
                keySpan = keySpan.Trim();
            valueSpan = line.AsSpan(index + KeyValueFlag.Length);
            if (TrimPairs)
                valueSpan = valueSpan.Trim();
        }

        protected int MatchFlag(string line)
        {
            var index = line.IndexOf(KeyValueFlag, StringComparison.Ordinal);
            return index;
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write("[");
            textWriter.Write(SectionName);
            textWriter.WriteLine("]");

            foreach (var kvp in PropertyInfos)
            {
                var name = kvp.Key;
                var sectionInfo = kvp.Value;
                var prop = sectionInfo.PropertyInfo;

                string? value = null;
                var rawObj = prop.GetValue(this);
                if (sectionInfo.Attribute?.Default != null)
                {
                    if (sectionInfo.Attribute.Default is SectionPropertyAttribute.DefaultValue)
                    {
                        if (rawObj == default) continue;
                        if (rawObj is ICollection { Count: 0 }) continue;
                    }

                    if (sectionInfo.Attribute.Default.GetHashCode() == rawObj?.GetHashCode())
                        continue;
                }

                var attr = prop.GetCustomAttribute<SectionConverterAttribute>(false);
                if (attr != null)
                {
                    var converter = attr.GetConverter();

                    textWriter.Write(name);
                    textWriter.Write(KeyValueFlag);
                    if (rawObj != null) converter.WriteSection(textWriter, rawObj);
                    textWriter.WriteLine();
                    continue;
                }

                if (prop.GetMethod.ReturnType.BaseType == typeof(Enum) && rawObj != null)
                {
                    var enumAttr = prop.GetCustomAttribute<SectionEnumAttribute>(false);
                    if (enumAttr != null)
                    {
                        switch (enumAttr.Option)
                        {
                            case EnumParseOption.Index:
                                var type = Enum.GetUnderlyingType(rawObj.GetType());
                                value = Convert.ChangeType(rawObj, type).ToString();
                                break;
                            case EnumParseOption.String:
                                value = rawObj.ToString();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else if (prop.GetMethod.ReturnType == typeof(bool) && rawObj != null)
                {
                    var boolAttr = prop.GetCustomAttribute<SectionBoolAttribute>(false);
                    if (boolAttr != null)
                    {
                        value = boolAttr.Type switch
                        {
                            BoolParseType.ZeroOne => Convert.ToInt32(rawObj).ToString(),
                            BoolParseType.String => rawObj.ToString(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                }

                if (value == null && rawObj != null)
                {
                    value = rawObj.ToString();
                }

                textWriter.Write(name);
                textWriter.Write(KeyValueFlag);
                textWriter.WriteLine(value);
            }
        }

        protected virtual string KeyValueFlag { get; } = ":";
        protected virtual bool TrimPairs { get; } = false;
        protected readonly Dictionary<string, SectionInfo> PropertyInfos = new();
    }

    public class SectionInfo
    {

        public SectionInfo(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }
        public SectionPropertyAttribute? Attribute { get; set; }
    }
}
