using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Configurable
{
    public abstract class KeyValueSection : Section
    {
        [SectionIgnore]
        public Dictionary<string, string>? UndefinedPairs { get; private set; }

        public KeyValueSection()
        {
            var type = GetType();
            var publicProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (publicProps.Length != 0)
            {
                _propertyInfos = publicProps
                    .Where(k => k.GetCustomAttribute<SectionIgnoreAttribute>() == null)
                    .Select(k =>
                    {
                        var attr = k.GetCustomAttribute<SectionPropertyAttribute>();
                        return (k, attr == null ? k.Name : attr.Name);
                    })
                    .ToList();
            }
            else
                _propertyInfos = new List<(PropertyInfo propInfo, string name)>();

            var privateProps = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            if (privateProps.Length != 0)
            {
                _propertyInfos.AddRange(privateProps
                    .Where(k => k.GetCustomAttribute<SectionIgnoreAttribute>() == null &&
                                k.GetCustomAttribute<SectionPropertyAttribute>() != null)
                    .Select(k =>
                    {
                        var attr = k.GetCustomAttribute<SectionPropertyAttribute>();
                        return (k, attr.Name);
                    })
                );
            }
        }

        public override void Match(string line)
        {
            MatchKeyValue(line, out var keySpan, out var valueSpan);
            var key = keySpan.ToString();
#if !NETCOREAPP3_1_OR_GREATER
            
#endif
            var prop = _propertyInfos.FirstOrDefault(k => k.name == key).propInfo;
            if (prop == null)
            {
                UndefinedPairs ??= new Dictionary<string, string>();
                UndefinedPairs.Add(key, valueSpan.ToString());
            }
            else
            {
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
            var index = line.IndexOf(KeyValueFlag, StringComparison.InvariantCulture);
            return index;
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write("[");
            textWriter.Write(SectionName);
            textWriter.WriteLine("]");

            foreach (var (prop, name) in _propertyInfos)
            {
                string key = name;
                string? value = null;
                var rawObj = prop.GetValue(this);
                if (rawObj is null) continue;

                var attr = prop.GetCustomAttribute<SectionConverterAttribute>(false);
                if (attr != null)
                {
                    var converter = attr.GetConverter();
                    value = converter.WriteSection(rawObj);
                }
                else if (prop.GetMethod.ReturnType.BaseType == typeof(Enum))
                {
                    var enumAttr = prop.GetCustomAttribute<SectionEnumAttribute>(false);
                    if (enumAttr != null)
                    {
                        value = enumAttr.Option switch
                        {
                            EnumParseOption.Index => ((int)rawObj).ToString(),
                            EnumParseOption.String => rawObj.ToString(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                }
                else if (prop.GetMethod.ReturnType == typeof(bool))
                {
                    var boolAttr = prop.GetCustomAttribute<SectionBoolAttribute>(false);
                    if (boolAttr != null)
                    {
                        value = boolAttr.Option switch
                        {
                            BoolParseOption.ZeroOne => Convert.ToInt32(rawObj).ToString(),
                            BoolParseOption.String => rawObj.ToString(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                }

                value ??= rawObj.ToString();
                textWriter.Write(key);
                textWriter.Write(KeyValueFlag);
                textWriter.WriteLine(value);
            }
        }

        protected virtual string KeyValueFlag { get; } = ":";
        protected virtual bool TrimPairs { get; } = false;
        private readonly List<(PropertyInfo propInfo, string name)> _propertyInfos;
    }
}
