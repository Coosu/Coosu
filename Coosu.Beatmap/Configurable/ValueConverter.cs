using System;
using System.IO;

namespace Coosu.Beatmap.Configurable;

public abstract class ValueConverter
{
    public abstract object ReadSection(ReadOnlySpan<char> value, Type targetType);
    public abstract void WriteSection(TextWriter textWriter, object value);
}

public abstract class ValueConverter<T> : ValueConverter
{
    public abstract T ReadSection(ReadOnlySpan<char> value);

    public abstract void WriteSection(TextWriter textWriter, T value);

    public sealed override object ReadSection(ReadOnlySpan<char> value, Type targetType)
    {
        return ReadSection(value);
    }

    public sealed override void WriteSection(TextWriter textWriter, object value)
    {
        WriteSection(textWriter, (T)value);
    }
}