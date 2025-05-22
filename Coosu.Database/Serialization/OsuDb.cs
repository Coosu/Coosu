using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.Annotations;
using Coosu.Database.DataTypes;
using Coosu.Database.Generated;

namespace Coosu.Database.Serialization;

public class OsuDb
{
    public int OsuVersion { get; set; }
    public int FolderCount { get; set; }
    public bool AccountUnlocked { get; set; }
    public DateTime UnlockDate { get; set; }
    public string PlayerName { get; set; } = null!;
    internal int BeatmapCount => Beatmaps?.Count ?? 0;

    [StructureArray(typeof(Beatmap), nameof(BeatmapCount),
        SubDataType = DataType.Object)]
    public List<Beatmap> Beatmaps { get; set; } = new();

    public Permissions Permissions { get; set; }

    public static OsuDb ReadFromFile(string path)
    {
        return ReadFromStream(File.OpenRead(path));
    }

    public static OsuDb ReadFromStream(Stream stream)
    {
        var osuDb = new OsuDb();
        using var reader = new OsuDbReader(stream);

        int beatmapCount = 0;
        while (!reader.IsEndOfStream && reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;
            var nodeId = (NodeId)reader.NodeId;

            if (nodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }

            if (nodeId == NodeId.OsuDb_OsuVersion) osuDb.OsuVersion = reader.GetInt32();
            else if (nodeId == NodeId.OsuDb_FolderCount) osuDb.FolderCount = reader.GetInt32();
            else if (nodeId == NodeId.OsuDb_AccountUnlocked) osuDb.AccountUnlocked = reader.GetBoolean();
            else if (nodeId == NodeId.OsuDb_UnlockDate) osuDb.UnlockDate = reader.GetDateTime();
            else if (nodeId == NodeId.OsuDb_PlayerName) osuDb.PlayerName = reader.GetString();
            else if (nodeId == NodeId.OsuDb_BeatmapCount) beatmapCount = reader.GetInt32();
            else if (nodeId == NodeId.OsuDb_BeatmapArray)
            {
                osuDb.Beatmaps.Capacity = beatmapCount;
                osuDb.Beatmaps.AddRange(reader.EnumerateBeatmaps());
            }
            else if (nodeId == NodeId.OsuDb_Permissions) osuDb.Permissions = (Permissions)reader.GetInt32();
        }

        return osuDb;
    }
}