using System;

namespace OSharp.Beatmap.Configurable
{
    public abstract class ValueConverter
    {
        public abstract object ReadSection(string value, Type targetType);
        public abstract string WriteSection(object value);
    }

    public abstract class ValueConverter<T> : ValueConverter
    {
        public abstract T ReadSection(string value);

        public abstract string WriteSection(T value);

        public sealed override object ReadSection(string value, Type targetType)
        {
            return ReadSection(value);
        }

        public sealed override string WriteSection(object value)
        {
            return WriteSection((T)value);
        }
    }
}
