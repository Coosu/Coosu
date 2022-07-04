using System;

namespace Coosu.Database.Annotations;

// Not ready for public
internal sealed class StructureIgnoreWhenAttribute : Attribute
{
    public enum Condition
    {
        Equals, Contains
    }

    public string MemberName { get; }
    public Condition IgnoreCondition { get; }
    public object Value { get; }

    public StructureIgnoreWhenAttribute(string memberName, Condition ignoreCondition, object value)
    {
        MemberName = memberName;
        IgnoreCondition = ignoreCondition;
        Value = value;
    }
}