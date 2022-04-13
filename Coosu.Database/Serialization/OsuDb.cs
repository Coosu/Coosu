using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;
using Coosu.Database.Mapping;
using Coosu.Database.Mapping.Converting;

namespace Coosu.Database.Serialization;

public class OsuDb
{
    public int OsuVersion { get; set; }
    public int FolderCount { get; set; }
    public bool AccountUnlocked { get; set; }
    public DateTime UnlockDate { get; set; }
    public string PlayerName { get; set; }
    internal int BeatmapCount => Beatmaps?.Count ?? 0;
    [OsuDbArray(typeof(Beatmap),
        SubDataType = DataType.Object, LengthDeclaration = nameof(BeatmapCount),
        Converter = typeof(IntDoublePair2ModKeyValueConverter))]
    public List<Beatmap> Beatmaps { get; set; } = new();
    public Permissions Permissions { get; set; }
    public static OsuDb ReadFromFile(string path, MappingHelper? mappingHelper = null)
    {
        return ReadFromStream(File.OpenRead(path), mappingHelper);
    }

    public static OsuDb ReadFromStream(Stream stream, MappingHelper? mappingHelper = null)
    {
        var osuDb = new OsuDb();
        using var reader = new OsuDbReader(stream, mappingHelper);

        int itemIndex = -1;
        int beatmapCount = 0;
        while (reader.Read())
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

            if (itemIndex == 0) osuDb.OsuVersion = reader.GetInt32();
            else if (itemIndex == 1) osuDb.FolderCount = reader.GetInt32();
            else if (itemIndex == 2) osuDb.AccountUnlocked = reader.GetBoolean();
            else if (itemIndex == 3) osuDb.UnlockDate = reader.GetDateTime();
            else if (itemIndex == 4) osuDb.PlayerName = reader.GetString();
            else if (itemIndex == 5) beatmapCount = reader.GetInt32();
            else if (itemIndex == 6)
            {
                osuDb.Beatmaps.Capacity = beatmapCount;
                osuDb.Beatmaps.AddRange(reader.EnumerateBeatmaps());
            }
            else if (itemIndex == 7) osuDb.Permissions = (Permissions)reader.GetInt32();
        }

        return osuDb;
    }

}