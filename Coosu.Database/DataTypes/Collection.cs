using System.Collections.Generic;
using Coosu.Database.Annotations;

namespace Coosu.Database.DataTypes;

public record Collection
{
    public string Name { get; set; } = null!;
    internal int BeatmapHashCount => BeatmapHashes?.Count ?? 0;

    [StructureArray(typeof(string), nameof(BeatmapHashCount))]
    public List<string> BeatmapHashes { get; set; } = new();
}