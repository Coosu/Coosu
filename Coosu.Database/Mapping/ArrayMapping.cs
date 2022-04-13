using System;

namespace Coosu.Database.Mapping;

internal class ArrayMapping : IMapping
{
    public int CurrentItemIndex { get; set; } = 0;
    public IMapping BaseMapping { get; set; }
    public Type SubItemType { get; set; }
    public DataType SubDataType { get; set; }
    public ClassMapping? ClassMapping { get; set; }
    public PropertyMapping? PropertyMapping { get; set; }
    public bool IsObjectArray { get; set; }
    public Func<object>? CreateArray { get; set; }
    public int Length { get; set; }
    public string LengthDeclarationMember { get; set; }

    public string Name { get; set; }
    public string Path { get; set; }
}