using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Configurable;

public abstract class KeyValueSection : Section
{
    private static readonly ConcurrentDictionary<Type, SectionPropertyLookup> TypePropertyLookupCache = new();

    protected readonly SectionPropertyLookup PropertiesLookup;

#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Aot", "IL2067",
        Justification =
            "The 'thisType' (and therefore the 'type' in the lambda) is passed to AddTypeSectionInfo, which is annotated to require PublicProperties and NonPublicProperties. The AOT analyzer cannot see this through the lambda, but the requirement is met.")]
#endif
    public KeyValueSection()
    {
        var thisType = GetType();
        PropertiesLookup = TypePropertyLookupCache.GetOrAdd(thisType,
            type => new SectionPropertyLookup(AddTypeSectionInfo(type)));
    }

    [SectionIgnore]
    public Dictionary<string, string> UndefinedPairs { get; } = new();

    [SectionIgnore]
    protected virtual FlagRule FlagRule { get; } = FlagRules.Colon;

    [SectionIgnore]
    protected virtual IReadOnlyList<FlagRule> FuzzyFlagRules { get; } = FlagRules.FuzzyRules;

    public override void Match(ReadOnlyMemory<char> memory)
    {
        var lineSpan = memory.Span;
        MatchKeyValue(lineSpan, out var keySpan, out var valueSpan);

        if (!PropertiesLookup.TryGetValue(keySpan, out var sectionInfo) || sectionInfo == null)
        {
            UndefinedPairs.Add(keySpan.ToString(), valueSpan.ToString());
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
            else if (propType.IsEnum)
            {
#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
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
                        $"Can not convert {{{keySpan.ToString()}}} key's value {{{valueSpan.ToString()}}} to type {propType}.",
                        ex);
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

        foreach (var kvp in PropertiesLookup.OriginalMap)
        {
            var name = kvp.Key;
            var sectionInfo = kvp.Value;
            AppendPair(textWriter, sectionInfo, name);
        }
    }

    protected void MatchKeyValue(ReadOnlySpan<char> lineSpan, out ReadOnlySpan<char> keySpan,
        out ReadOnlySpan<char> valueSpan)
    {
        int index = MatchFlag(lineSpan, out var flagRule);
        if (index == -1) throw new Exception($"Unknown Key-Value: {lineSpan.ToString()}");

        keySpan = lineSpan.Slice(0, index);
        if (flagRule!.TrimType is TrimType.Key or TrimType.Both)
        {
            keySpan = keySpan.Trim();
        }

        valueSpan = lineSpan.Slice(index + flagRule.SplitFlag.Length);
        if (flagRule.TrimType is TrimType.Value or TrimType.Both)
        {
            valueSpan = valueSpan.Trim();
        }
    }

    protected int MatchFlag(ReadOnlySpan<char> lineSpan, out FlagRule? flagRule)
    {
        var index = lineSpan.IndexOf(FlagRule.SplitFlag.AsSpan());
        if (index == -1)
        {
            flagRule = null;
            foreach (var rule in FuzzyFlagRules)
            {
                index = lineSpan.IndexOf(rule.SplitFlag.AsSpan());
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
                value = sectionInfo.UseSpecificFormat
                    ? floatObj.ToEnUsFormatString()
                    : floatObj.ToString(CultureInfo.CurrentCulture);
            }
            else if (rawObj is double doubleObj)
            {
                value = sectionInfo.UseSpecificFormat
                    ? doubleObj.ToEnUsFormatString()
                    : doubleObj.ToString(CultureInfo.CurrentCulture);
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

#if NET6_0_OR_GREATER
    private static Dictionary<string, SectionInfo> AddTypeSectionInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type type)
#else
    private static Dictionary<string, SectionInfo> AddTypeSectionInfo(Type type)
#endif
    {
        var propertyInfos = new Dictionary<string, SectionInfo>();
        var props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in props)
        {
            if (propertyInfo.SetMethod == null) continue;

            var sectionIgnoreAttr = propertyInfo.GetCustomAttribute<SectionIgnoreAttribute>();
            if (sectionIgnoreAttr != null) continue;

            var sectionPropertyAttr = propertyInfo.GetCustomAttribute<SectionPropertyAttribute>();
            string keyName = sectionPropertyAttr?.Name ?? propertyInfo.Name;
            bool useSpecificFormat = sectionPropertyAttr?.UseSpecificFormat ?? false;

            if (!propertyInfo.SetMethod.IsPublic && sectionPropertyAttr == null)
            {
                continue;
            }

            propertyInfos.Add(keyName, new SectionInfo(propertyInfo)
            {
                UseSpecificFormat = useSpecificFormat,
                Attribute = sectionPropertyAttr,
            });
        }

        return propertyInfos;
    }
}