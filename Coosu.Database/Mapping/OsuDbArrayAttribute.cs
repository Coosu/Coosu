using System;

namespace Coosu.Database.Mapping;

public class OsuDbArrayAttribute : Attribute
{
    public OsuDbArrayAttribute(Type type)
    {
        SubItemType = type;
    }
    public Type SubItemType { get; }
    public bool IsObject { get; set; }
    public string? LengthDeclaration { get; set; }
    public Type? Converter { get; set; }
    public Type? ValueHandler { get; set; }
}