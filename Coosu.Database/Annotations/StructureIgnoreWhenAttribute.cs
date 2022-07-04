using System;

namespace Coosu.Database.Annotations;

// Not ready for public
internal sealed class StructureIgnoreWhenAttribute : Attribute
{
    public enum Condition
    {
        Unequals, NotContains
    }

    public StructureIgnoreWhenAttribute(string memberName, Condition ignoreCondition, object value)
    {
        MemberName = memberName;
        IgnoreCondition = ignoreCondition;
        Value = value;
    }

    public string MemberName { get; }
    public Condition IgnoreCondition { get; }
    public object Value { get; }
    internal int NodeId { get; set; } = -1;
}