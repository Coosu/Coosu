using System;
using System.Reflection;

namespace Coosu.Beatmap.Configurable
{
    internal class ReflectInfo
    {
        public ReflectInfo(PropertyInfo propertyInfo, Type type, ExecuteType executeType, string name, Attribute[] attributes)
        {
            PropertyInfo = propertyInfo;
            Type = type;
            ExecuteType = executeType;
            Name = name;
            Attributes = attributes;
        }

        public PropertyInfo PropertyInfo { get; }
        public Type Type { get; }
        public ExecuteType ExecuteType { get; }
        public string Name { get; }
        public Attribute[] Attributes { get; }
    }
}