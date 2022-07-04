using System;
using System.Diagnostics;
using Coosu.Database.Annotations;

namespace Coosu.Database.Internal;

[DebuggerDisplay("Property: {NodeId}, {Path}")]
internal sealed class PropertyStructure : IDbStructure
{
    public PropertyStructure(int nodeId, string name, string path, IDbStructure baseStructure,
        Type targetType, DataType targetDataType, IValueHandler valueHandler,
        StructureIgnoreWhenAttribute? ignoreWhenAttribute)
    {
        NodeId = nodeId;
        Name = name;
        Path = path;
        BaseStructure = baseStructure;
        TargetType = targetType;
        TargetDataType = targetDataType;
        ValueHandler = valueHandler;
        IgnoreWhenAttribute = ignoreWhenAttribute;
    }

    public int NodeId { get; }
    public string Name { get; }
    public string Path { get; }
    public IDbStructure BaseStructure { get; }

    public Type TargetType { get; }
    public DataType TargetDataType { get; }
    public IValueHandler ValueHandler { get; }
    public StructureIgnoreWhenAttribute? IgnoreWhenAttribute { get; }
}