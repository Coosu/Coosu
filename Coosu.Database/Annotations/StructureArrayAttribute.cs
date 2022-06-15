using System;

namespace Coosu.Database.Annotations;

public sealed class StructureArrayAttribute : Attribute
{
    public StructureArrayAttribute(Type itemType, string lengthMemberName)
    {
        ItemType = itemType;
        LengthMemberName = lengthMemberName;
    }

    public Type ItemType { get; }
    public string LengthMemberName { get; }

    public DataType SubDataType { get; set; } = DataType.Unknown;
    public Type? Converter { get; set; }
    public Type? ValueHandler { get; set; }
}