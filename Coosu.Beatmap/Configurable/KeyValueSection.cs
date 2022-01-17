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
        public Dictionary<string, string> UndefinedPairs { get; private set; }

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
            if (!MatchKeyValue(line, out var key, out var value))
                throw new Exception("Unknown Key-Value: " + line);

            var prop = _propertyInfos.FirstOrDefault(k => k.name == key).propInfo;
            if (prop == null)
            {
                if (UndefinedPairs == null)
                    UndefinedPairs = new Dictionary<string, string>();
                UndefinedPairs.Add(key, value);
            }
            else
            {
                var propType = prop.GetMethod.ReturnType;
                var attr = prop.GetCustomAttribute<SectionConverterAttribute>();

                if (attr != null)
                {
                    var converter = attr.GetConverter();
                    prop.SetValue(this, converter.ReadSection(value, propType));
                }
                else if (propType.BaseType == typeof(Enum))
                {
                    prop.SetValue(this, Enum.Parse(propType, value));
                }
                else
                {
                    if (ValueConvert.ConvertValue(value, propType, out var converted))
                    {
                        prop.SetValue(this, converted);
                    }
                    else
                    {
                        throw new MissingMethodException($"Can not convert {{{value}}} to type {propType}.");
                    }
                }
            }
        }

        protected bool MatchKeyValue(string line, out string key, out string value)
        {
            int index = MatchFlag(line);
            if (index == -1)
            {
                key = null;
                value = null;
                return false;
            }

            key = line.Substring(0, index);
            if (TrimPairs)
                key = key.Trim();
            value = line.Substring(index + KeyValueFlag.Length);
            if (TrimPairs)
                value = value.Trim();
            return true;
        }

        protected int MatchFlag(string line)
        {
            var index = line.IndexOf(KeyValueFlag, StringComparison.InvariantCulture);
            return index;
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.WriteLine($"[{SectionName}]");

            foreach (var (prop, name) in _propertyInfos)
            {
                string key = name;
                string value = null;
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
                        switch (enumAttr.Option)
                        {
                            case EnumParseOption.Index:
                                value = ((int)rawObj).ToString();
                                break;
                            case EnumParseOption.String:
                                value = rawObj.ToString();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else if (prop.GetMethod.ReturnType == typeof(bool))
                {
                    var boolAttr = prop.GetCustomAttribute<SectionBoolAttribute>(false);
                    if (boolAttr != null)
                    {
                        switch (boolAttr.Option)
                        {
                            case BoolParseOption.ZeroOne:
                                value = Convert.ToInt32(rawObj).ToString();
                                break;
                            case BoolParseOption.String:
                                value = rawObj.ToString();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (value == null)
                    value = rawObj.ToString();
                textWriter.WriteLine($"{key}{KeyValueFlag}{value}");
            }
        }

        protected virtual string KeyValueFlag { get; } = ":";
        protected virtual bool TrimPairs { get; } = false;
        private readonly List<(PropertyInfo propInfo, string name)> _propertyInfos;
    }
}
