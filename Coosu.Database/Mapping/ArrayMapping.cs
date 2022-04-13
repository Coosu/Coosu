using System;

namespace Coosu.Database.Mapping;

internal class ArrayMapping : IMapping
{
    public int CurrentItemIndex { get; set; } = -1;
    public ClassMapping BaseClass { get; set; }
    public Type SubItemType { get; set; }
    public ClassMapping? ClassMapping { get; set; }
    public PropertyMapping? PropertyMapping { get; set; }
    public bool IsObjectArray { get; set; }
    public Func<object>? CreateArray { get; set; }
    public int Length { get; set; }
    public string LengthDeclarationMember { get; set; }
}