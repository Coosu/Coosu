using System.Collections.Generic;
using System.Diagnostics;

namespace Coosu.Database.Internal;

[DebuggerDisplay("Class: {NodeId}, {Path}")]
internal sealed class ObjectStructure : IDbStructure
{
    public ObjectStructure(int nodeId, string name, string path, IDbStructure? baseStructure)
    {
        NodeId = nodeId;
        Name = name;
        Path = path;
        BaseStructure = baseStructure;
    }

    public int NodeId { get; }
    public string Name { get; }
    public string Path { get; }
    public IDbStructure? BaseStructure { get; }

    public List<IDbStructure> Structures { get; } = new();

    internal Dictionary<string, int> MemberNameIdMapping { get; } = new();
}