using System.Collections.Generic;

namespace Coosu.Database.Internal;

internal sealed class ClassStructure : IDbStructure
{
    public ClassStructure(int nodeId, string name, string path, IDbStructure? baseStructure)
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