using System.Collections.Generic;
using System.IO;
using Coosu.Database.Annotations;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Serialization;

public class ScoresDb
{
    public int OsuVersion { get; set; }
    internal int CollectionCount => Beatmaps?.Count ?? 0;

    [StructureArray(typeof(ScoreBeatmap), nameof(CollectionCount),
        SubDataType = DataType.Object)]
    public List<ScoreBeatmap> Beatmaps { get; set; } = new();

    public static ScoresDb ReadFromFile(string path)
    {
        return ReadFromStream(File.OpenRead(path));
    }

    public static ScoresDb ReadFromStream(Stream stream)
    {
        var collectionDb = new ScoresDb();
        using var reader = new OsuDbReader(stream, StructureHelperPool.TypeScoresDb);

        int itemIndex = -1;
        int collectionCount = 0;
        while (!reader.IsEndOfStream && reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;

            if (nodeType is NodeType.ArrayStart or NodeType.KeyValue)
            {
                itemIndex++;
            }
            else
            {
                continue;
            }

            if (itemIndex == 0) collectionDb.OsuVersion = reader.GetInt32();
            else if (itemIndex == 1) collectionCount = reader.GetInt32();
            else if (itemIndex == 2)
            {
                collectionDb.Beatmaps.Capacity = collectionCount;
                collectionDb.Beatmaps.AddRange(reader.EnumerateScoreBeatmaps());
            }
        }

        return collectionDb;
    }
}