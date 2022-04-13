using System.Collections.Generic;

namespace Coosu.Database.Mapping;

internal class ClassMapping : IMapping
{
    public IMapping? BaseMapping { get; set; }
    public int CurrentMemberIndex { get; set; } = -1;
    public List<IMapping> Mapping { get; set; } = new();

    public string Name { get; set; }
    public string Path { get; set; }
}