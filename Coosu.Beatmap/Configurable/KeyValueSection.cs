using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Configurable;

public abstract class KeyValueSection : Section
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, SectionInfo>> TypePropertyInfoCache = new();

    protected readonly Dictionary<string, SectionInfo> PropertyInfos;

    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    public KeyValueSection()
    {
        var thisType = GetType();
        PropertyInfos = TypePropertyInfoCache.GetOrAdd(thisType, AddTypeSectionInfo);
    }

    public KeyValueSection(CultureInfo culture) : this()
    {
        _culture = culture;
    }

    [SectionIgnore]
    public Dictionary<string, string> UndefinedPairs { get; } = new();

    [SectionIgnore]
    protected virtual FlagRule FlagRule { get; } = FlagRules.Colon;

    [SectionIgnore]
    protected virtual IReadOnlyList<FlagRule> FuzzyFlagRules { get; } = FlagRules.FuzzyRules;

    public override void Match(string line)
    {
        MatchKeyValue(line, out var keySpan, out var valueSpan);
        var key = keySpan.ToString();

        if (!PropertyInfos.TryGetValue(key, out var sectionInfo))
        {
            UndefinedPairs.Add(key, valueSpan.ToString());
        }
        else
        {
            var prop = sectionInfo.PropertyInfo;
            var propType = prop.GetMethod!.ReturnType;
            var attr = prop.GetCustomAttribute<SectionConverterAttribute>();

            if (attr != null)
            {
                var converter = attr.GetConverter();
                prop.SetValue(this, converter.ReadSection(valueSpan, propType));
            }
            else if (propType.BaseType == StaticTypes.Enum)
            {
#if NET6_0_OR_GREATER
                prop.SetValue(this, Enum.Parse(propType, valueSpan));
#else
                prop.SetValue(this, Enum.Parse(propType, valueSpan.ToString()));
#endif
            }
            else
            {
                object? converted;
                try
                {
                    converted = ValueConvert.ConvertValue(valueSpan, propType, sectionInfo.UseSpecificFormat);
                }
                catch (Exception ex)
                {
                    throw new ValueConvertException(
                        $"Can not convert {{{key}}} key's value {{{valueSpan.ToString()}}} to type {propType}.", ex);
                }

                prop.SetValue(this, converted);
            }
        }
    }

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write('[');
        textWriter.Write(SectionName);
        textWriter.WriteLine(']');

        foreach (var kvp in PropertyInfos)
        {
            var name = kvp.Key;
            var sectionInfo = kvp.Value;
            AppendPair(textWriter, sectionInfo, name);
        }
    }

    protected void MatchKeyValue(string line, out ReadOnlySpan<char> keySpan, out ReadOnlySpan<char> valueSpan)
    {
        int index = MatchFlag(line, out var flagRule);
        if (index == -1) throw new Exception($"Unknown Key-Value: {line}");

        keySpan = line.AsSpan(0, index);
        if (flagRule!.TrimType is TrimType.Key or TrimType.Both)
        {
            keySpan = keySpan.Trim();
        }

        valueSpan = line.AsSpan(index + flagRule.SplitFlag.Length);
        if (flagRule.TrimType is TrimType.Value or TrimType.Both)
        {
            valueSpan = valueSpan.Trim();
        }
    }

    protected int MatchFlag(string line, out FlagRule? flagRule)
    {
        var index = line.IndexOf(FlagRule.SplitFlag, StringComparison.Ordinal);
        if (index == -1)
        {
            flagRule = null;
            foreach (var rule in FuzzyFlagRules)
            {
                index = line.IndexOf(rule.SplitFlag, StringComparison.Ordinal);
                if (index == -1) continue;
                flagRule = rule;
                return index;
            }
        }
        else
        {
            flagRule = FlagRule;
        }

        return index;
    }

    private void AppendPair(TextWriter textWriter, SectionInfo sectionInfo, string name)
    {
        var prop = sectionInfo.PropertyInfo;

        string? value = null;
        var rawObj = prop.GetValue(this);
        if (sectionInfo.Attribute?.Default != null)
        {
            if (sectionInfo.Attribute.Default is SectionPropertyAttribute.DefaultValue)
            {
                if (rawObj == default) return;
                if (rawObj is ICollection { Count: 0 }) return;
            }

            if (sectionInfo.Attribute.Default.GetHashCode() == rawObj?.GetHashCode())
                return;
        }

        var attr = prop.GetCustomAttribute<SectionConverterAttribute>(false);
        if (attr != null)
        {
            var converter = attr.GetConverter();

            textWriter.Write(name);
            textWriter.Write(FlagRule.SplitFlag);
            if (rawObj != null) converter.WriteSection(textWriter, rawObj);
            textWriter.WriteLine();
            return;
        }

        if (prop.GetMethod!.ReturnType.BaseType == StaticTypes.Enum && rawObj != null)
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
        else if (prop.GetMethod.ReturnType == StaticTypes.Boolean && rawObj != null)
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
            if (rawObj is float floatObj)
            {
                value = floatObj.ToString(_culture);
            }
            else if (rawObj is double doubleObj)
            {
                value = doubleObj.ToString(_culture);
            }
            else
            {
                value = rawObj.ToString();
            }
        }

        textWriter.Write(name);
        textWriter.Write(FlagRule.SplitFlag);
        textWriter.WriteLine(value);
    }

    private static Dictionary<string, SectionInfo> AddTypeSectionInfo(Type type)
    {
        var propertyInfos = new Dictionary<string, SectionInfo>();
        var props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        for (var i = 0; i < props.Length; i++)
        {
            var propertyInfo = props[i];
            if (propertyInfo.SetMethod == null) continue;
            var isPublic = propertyInfo.SetMethod.IsPublic;
            if (!isPublic)
            {
                var sectionPropertyAttr = propertyInfo.GetCustomAttribute<SectionPropertyAttribute>();
                if (sectionPropertyAttr == null) continue;
                propertyInfos.Add(sectionPropertyAttr.Name ?? propertyInfo.Name, new SectionInfo(propertyInfo)
                {
                    UseSpecificFormat = sectionPropertyAttr.UseSpecificFormat,
                    Attribute = sectionPropertyAttr
                });
            }
            else
            {
                var sectionIgnoreAttr = propertyInfo.GetCustomAttribute<SectionIgnoreAttribute>();
                if (sectionIgnoreAttr != null) continue;
                var sectionPropertyAttr = propertyInfo.GetCustomAttribute<SectionPropertyAttribute>();
                propertyInfos.Add(sectionPropertyAttr?.Name ?? propertyInfo.Name, new SectionInfo(propertyInfo)
                {
                    UseSpecificFormat = sectionPropertyAttr?.UseSpecificFormat ?? false,
                    Attribute = sectionPropertyAttr,
                });
            }
        }

        return propertyInfos;
    }
}