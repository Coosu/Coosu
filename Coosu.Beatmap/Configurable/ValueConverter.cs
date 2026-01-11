using System;
using System.IO;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Configurable;

public abstract class ValueConverter
{
#if NET6_0_OR_GREATER
    public abstract object ReadSection(ReadOnlySpan<char> value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type targetType);
#else
    public abstract object ReadSection(ReadOnlySpan<char> value, Type targetType);
#endif
    public abstract void WriteSection(TextWriter textWriter, object value);
}

public abstract class ValueConverter<T> : ValueConverter
{
    public abstract T ReadSection(ReadOnlySpan<char> value);

    public abstract void WriteSection(TextWriter textWriter, T value);

#if NET6_0_OR_GREATER
    public sealed override object ReadSection(ReadOnlySpan<char> value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type targetType)
#else
    public sealed override object ReadSection(ReadOnlySpan<char> value, Type targetType)
#endif
    {
        return ReadSection(value)!;
    }

    public sealed override void WriteSection(TextWriter textWriter, object value)
    {
        WriteSection(textWriter, (T)value);
    }
}