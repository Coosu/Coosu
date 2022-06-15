using System;
using System.Reflection;

namespace Coosu.Beatmap.Configurable
{
    public sealed class ReflectInfo
    {
        public ReflectInfo(PropertyInfo propertyInfo, Type type, string name)
        {
            PropertyInfo = propertyInfo;
            Type = type;
            Name = name;
        }

        public PropertyInfo PropertyInfo { get; }
        public Type Type { get; }
        public string Name { get; }
    }
}