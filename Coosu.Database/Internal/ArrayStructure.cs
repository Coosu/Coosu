using System;
using System.Diagnostics;

namespace Coosu.Database.Internal;

[DebuggerDisplay("Array: {NodeId}, {Path}")]
internal sealed class ArrayStructure : IDbStructure
{
    public ArrayStructure(int nodeId, string name, string path, IDbStructure baseStructure,
        Type itemType, DataType subDataType, bool isObjectArray, int lengthNodeId)
    {
        NodeId = nodeId;
        Name = name;
        Path = path;
        BaseStructure = baseStructure;
        ItemType = itemType;
        SubDataType = subDataType;
        IsObjectArray = isObjectArray;
        LengthNodeId = lengthNodeId;
    }

    public int NodeId { get; }
    public string Name { get; }
    public string Path { get; }
    public IDbStructure BaseStructure { get; }

    public Type ItemType { get; }
    public DataType SubDataType { get; internal set; }
    public bool IsObjectArray { get; }
    public int LengthNodeId { get; }

    public ObjectStructure? ObjectStructure { get; set; }
    public PropertyStructure? PropertyStructure { get; set; }
}