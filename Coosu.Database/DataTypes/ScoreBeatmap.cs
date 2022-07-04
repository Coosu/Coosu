using System.Collections.Generic;
using Coosu.Database.Annotations;

namespace Coosu.Database.DataTypes;

public class ScoreBeatmap
{
    public string Hash { get; set; }
    internal int ScoreCount => Scores?.Count ?? 0;

    [StructureArray(typeof(Score), nameof(ScoreCount),
        SubDataType = DataType.Object)]
    public List<Score> Scores { get; set; } = new();
}