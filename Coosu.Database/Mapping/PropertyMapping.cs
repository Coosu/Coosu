using System;
using Coosu.Database.Mapping.Converting;

namespace Coosu.Database.Mapping;

internal class PropertyMapping : IMapping
{
    public IMapping BaseMapping { get; set; }
    public Type TargetType { get; set; }
    public DataType TargetDataType { get; set; }
    public IValueHandler ValueHandler { get; set; }

    public string Name { get; set; }
    public string Path { get; set; }
}