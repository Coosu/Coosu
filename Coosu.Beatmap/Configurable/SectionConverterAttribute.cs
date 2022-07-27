using System;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Configurable;

public sealed class SectionConverterAttribute : Attribute
{
    public bool SharedCreation { get; set; } = true;

    private ValueConverter? _sharedInstance;
    private readonly Type _converterType;
    private readonly object[] _param;

    public SectionConverterAttribute(Type converterType, params object[] param)
    {
        if (!converterType.IsSubclassOf(StaticTypes.ValueConverter))
            throw new Exception($"Type {converterType} isn\'t a converter.");
        _converterType = converterType;
        _param = param;
    }

    public SectionConverterAttribute(Type converterType) : this(converterType, Array.Empty<object>())
    {

    }

    public ValueConverter GetConverter()
    {
        if (SharedCreation)
        {
            return _sharedInstance ??= (ValueConverter)Activator.CreateInstance(_converterType, _param)!;
        }

        return (ValueConverter)Activator.CreateInstance(_converterType, _param)!;
    }

}