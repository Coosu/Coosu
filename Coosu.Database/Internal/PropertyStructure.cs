using System;
using Coosu.Database.Annotations;

namespace Coosu.Database.Internal;

internal sealed class PropertyStructure : IDbStructure
{
    public PropertyStructure(int nodeId, string name, string path, IDbStructure baseStructure,
        Type targetType, DataType targetDataType, IValueHandler valueHandler)
    {
        NodeId = nodeId;
        Name = name;
        Path = path;
        BaseStructure = baseStructure;
        TargetType = targetType;
        TargetDataType = targetDataType;
        ValueHandler = valueHandler;
    }

    public int NodeId { get; }
    public string Name { get; }
    public string Path { get; }
    public IDbStructure BaseStructure { get; }

    public Type TargetType { get; }
    public DataType TargetDataType { get; }
    public IValueHandler ValueHandler { get; }
}