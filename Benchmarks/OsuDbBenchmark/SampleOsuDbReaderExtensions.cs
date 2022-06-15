using Coosu.Database;
using Coosu.Database.DataTypes;

namespace OsuDbBenchmark;

public static class SampleOsuDbReaderExtensions
{
    public static IEnumerable<SimpleBeatmap> EnumerateTinyBeatmaps(this OsuDbReader reader)
    {
        SimpleBeatmap? beatmap = null;

        while (reader.Read())
        {
            if (reader.NodeType == NodeType.ObjectStart)
            {
                beatmap = new SimpleBeatmap();
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && beatmap != null)
            {
                yield return beatmap;
                beatmap = null;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == 7) yield break;
            if (beatmap == null) continue;
            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue)) continue;
            FillProperty(reader,  beatmap);
        }
    }

    private static void FillProperty(OsuDbReader reader,  SimpleBeatmap beatmap)
    {
        // Use `reader.NodeId` comparision for performance.
        // You can check the full list by debugging
        // `Coosu.Database.Serialization.OsuDbReaderExtensions.EnumerateBeatmaps`,
        // and watch the `BeatmapStructure` field.
        // 
        // For maintainability, use `reader.Name` or `reader.Path`,
        // then `reader.Name.Equals("Artist", StringComparison.Ordinal)`
        var nodeId = reader.NodeId;

        if (nodeId == 9) beatmap.Artist = reader.GetString();
        else if (nodeId == 10) beatmap.ArtistUnicode = reader.GetString();
        else if (nodeId == 11) beatmap.Title = reader.GetString();
        else if (nodeId == 12) beatmap.TitleUnicode = reader.GetString();
        else if (nodeId == 13) beatmap.Creator = reader.GetString();
        else if (nodeId == 14) beatmap.Version = reader.GetString();
        else if (nodeId == 15) beatmap.AudioFileName = reader.GetString();
        else if (nodeId == 17) beatmap.BeatmapFileName = reader.GetString();
        else if (nodeId == 29) SetDefaultStarRating(beatmap, reader, DbGameMode.Circle);
        else if (nodeId == 32) SetDefaultStarRating(beatmap, reader, DbGameMode.Taiko);
        else if (nodeId == 35) SetDefaultStarRating(beatmap, reader, DbGameMode.Catch);
        else if (nodeId == 38) SetDefaultStarRating(beatmap, reader, DbGameMode.Mania);
        else if (nodeId == 40) beatmap.DrainTime = TimeSpan.FromSeconds(reader.GetInt32());
        else if (nodeId == 41) beatmap.TotalTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 42) beatmap.AudioPreviewTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 46) beatmap.BeatmapId = reader.GetInt32();
        else if (nodeId == 47) beatmap.BeatmapSetId = reader.GetInt32();
        else if (nodeId == 55) beatmap.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == 56) beatmap.Source = reader.GetString();
        else if (nodeId == 57) beatmap.Tags = reader.GetString();
        else if (nodeId == 63) beatmap.FolderName = reader.GetString();
    }

    private static void SetDefaultStarRating(SimpleBeatmap beatmap, OsuDbReader osuDbReader, DbGameMode index)
    {
        while (osuDbReader.Read())
        {
            if (osuDbReader.NodeType == NodeType.ArrayEnd) break;
            var data = osuDbReader.GetIntDoublePair();
            var mods = (Mods)data.IntValue;
            if (mods != Mods.None) continue;

            if (index == DbGameMode.Circle) beatmap.DefaultStarRatingStd = data.DoubleValue;
            else if (index == DbGameMode.Taiko) beatmap.DefaultStarRatingTaiko = data.DoubleValue;
            else if (index == DbGameMode.Catch) beatmap.DefaultStarRatingCtB = data.DoubleValue;
            else if (index == DbGameMode.Mania) beatmap.DefaultStarRatingMania = data.DoubleValue;
        }
    }
}