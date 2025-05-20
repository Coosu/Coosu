using System;
using Coosu.Beatmap.Internal;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Configurable;

public sealed class SectionConverterAttribute : Attribute
{
    public bool SharedCreation { get; set; } = true;

    private ValueConverter? _sharedInstance;

#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    private readonly Type _converterType;

    private readonly object[] _param;

    public SectionConverterAttribute(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] 
#endif
        Type converterType, params object[] param)
    {
        if (!converterType.IsSubclassOf(StaticTypes.ValueConverter))
            throw new Exception($"Type {converterType} isn\'t a converter.");
        _converterType = converterType;
        _param = param;
    }

    public SectionConverterAttribute(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type converterType) : this(converterType, Array.Empty<object>())
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