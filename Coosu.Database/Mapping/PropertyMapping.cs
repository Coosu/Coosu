using System;
using Coosu.Database.Mapping.Converting;

namespace Coosu.Database.Mapping;

internal class PropertyMapping : IMapping
{
    public ClassMapping BaseClass { get; set; }
    public Type TargetType { get; set; }
    public IValueHandler? ValueHandler { get; set; }
    public string FullName { get; set; }
}