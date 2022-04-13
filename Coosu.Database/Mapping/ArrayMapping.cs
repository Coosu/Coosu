using System;

namespace Coosu.Database.Mapping;

internal class ArrayMapping : IMapping
{
    public ClassMapping BaseClass { get; set; }
    public string LengthDeclarationMember { get; set; }
    public Type SubItemType { get; set; }
    public ClassMapping? ClassMapping { get; set; }
    public PropertyMapping? PropertyMapping { get; set; }
    public bool IsObjectArray { get; set; }
    public Func<object>? ArrayCreation { get; set; }
}