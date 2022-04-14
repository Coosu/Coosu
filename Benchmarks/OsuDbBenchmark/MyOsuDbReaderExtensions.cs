using Coosu.Database;
using Coosu.Database.DataTypes;

namespace OsuDbBenchmark;

public class SimpleBeatmapInfo
{
    public string Artist { get; set; } = null!;
    public string ArtistUnicode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string TitleUnicode { get; set; } = null!;
    public string Creator { get; set; } = null!; //mapper
    public string Version { get; set; } = null!;//difficulty name
    public string BeatmapFileName { get; set; } = null!;
    public DateTime LastModified { get; set; }
    public double DefaultStarRatingStd { get; set; }
    public double DefaultStarRatingTaiko { get; set; }
    public double DefaultStarRatingCtB { get; set; }
    public double DefaultStarRatingMania { get; set; }
    public TimeSpan DrainTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan AudioPreviewTime { get; set; }
    public int BeatmapId { get; set; }
    public int BeatmapSetId { get; set; }
    public DbGameMode GameMode { get; set; }
    public string Source { get; set; } = null!;
    public string Tags { get; set; } = null!;
    public string FolderName { get; set; } = null!;
    public string AudioFileName { get; set; } = null!;
}

public static class MyOsuDbReaderExtensions
{
    public static IEnumerable<SimpleBeatmapInfo> EnumerateMyBeatmaps(this OsuDbReader reader)
    {
        SimpleBeatmapInfo? beatmap = null;

        while (reader.Read())
        {
            if (reader.NodeType == NodeType.ObjectStart)
            {
                beatmap = new SimpleBeatmapInfo();
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
            FillProperty(reader, reader.NodeId, beatmap);
        }
    }

    private static void FillProperty(OsuDbReader reader, int nodeId, SimpleBeatmapInfo beatmapInfo)
    {
        if (nodeId == 9) beatmapInfo.Artist = reader.GetString();
        else if (nodeId == 10) beatmapInfo.ArtistUnicode = reader.GetString();
        else if (nodeId == 11) beatmapInfo.Title = reader.GetString();
        else if (nodeId == 12) beatmapInfo.TitleUnicode = reader.GetString();
        else if (nodeId == 13) beatmapInfo.Creator = reader.GetString();
        else if (nodeId == 14) beatmapInfo.Version = reader.GetString();
        else if (nodeId == 15) beatmapInfo.AudioFileName = reader.GetString();
        //else if (nodeId == 16) beatmap.Md5Hash = reader.GetString();
        else if (nodeId == 17) beatmapInfo.BeatmapFileName = reader.GetString();
        //else if (nodeId == 18) beatmap.RankedStatus = (RankedStatus)reader.GetByte();
        //else if (nodeId == 19) beatmap.CirclesCount = reader.GetInt16();
        //else if (nodeId == 20) beatmap.SlidersCount = reader.GetInt16();
        //else if (nodeId == 21) beatmap.SpinnersCount = reader.GetInt16();
        else if (nodeId == 22) beatmapInfo.LastModified = reader.GetDateTime();
        //else if (nodeId == 23) beatmap.ApproachRate = reader.GetSingle();
        //else if (nodeId == 24) beatmap.CircleSize = reader.GetSingle();
        //else if (nodeId == 25) beatmap.HpDrain = reader.GetSingle();
        //else if (nodeId == 26) beatmap.OverallDifficulty = reader.GetSingle();
        //else if (nodeId == 27) beatmap.SliderVelocity = reader.GetDouble();
        else if (nodeId == 29) FillStarRating(ref beatmapInfo, reader, DbGameMode.Circle);
        else if (nodeId == 32) FillStarRating(ref beatmapInfo, reader, DbGameMode.Taiko);
        else if (nodeId == 35) FillStarRating(ref beatmapInfo, reader, DbGameMode.Catch);
        else if (nodeId == 38) FillStarRating(ref beatmapInfo, reader, DbGameMode.Mania);
        else if (nodeId == 40) beatmapInfo.DrainTime = TimeSpan.FromSeconds(reader.GetInt32());
        else if (nodeId == 41) beatmapInfo.TotalTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 42) beatmapInfo.AudioPreviewTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        //else if (nodeId == 43) arrayCount = reader.GetInt32();
        //else if (nodeId == 44)
        //{
        //    if (arrayCount > 0) FillTimingPoints(beatmap.TimingPoints = new(arrayCount), reader);
        //}
        else if (nodeId == 46) beatmapInfo.BeatmapId = reader.GetInt32();
        else if (nodeId == 47) beatmapInfo.BeatmapSetId = reader.GetInt32();
        //else if (nodeId == 48) beatmap.ThreadId = reader.GetInt32();
        //else if (nodeId == 49) beatmap.GradeStandard = (Grade)reader.GetByte();
        //else if (nodeId == 50) beatmap.GradeTaiko = (Grade)reader.GetByte();
        //else if (nodeId == 51) beatmap.GradeCtb = (Grade)reader.GetByte();
        //else if (nodeId == 52) beatmap.GradeMania = (Grade)reader.GetByte();
        //else if (nodeId == 53) beatmap.LocalOffset = reader.GetInt16();
        //else if (nodeId == 54) beatmap.StackLeniency = reader.GetSingle();
        else if (nodeId == 55) beatmapInfo.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == 56) beatmapInfo.Source = reader.GetString();
        else if (nodeId == 57) beatmapInfo.Tags = reader.GetString();
        //else if (nodeId == 58) beatmap.OnlineOffset = reader.GetInt16();
        //else if (nodeId == 59) beatmap.TitleFont = reader.GetString();
        //else if (nodeId == 60) beatmap.IsUnplayed = reader.GetBoolean();
        //else if (nodeId == 61) beatmap.LastPlayed = reader.GetDateTime();
        //else if (nodeId == 62) beatmap.IsOsz2 = reader.GetBoolean();
        else if (nodeId == 63) beatmapInfo.FolderName = reader.GetString();
        //else if (nodeId == 64) beatmap.LastTimeChecked = reader.GetDateTime();
        //else if (nodeId == 65) beatmap.IsSoundIgnored = reader.GetBoolean();
        //else if (nodeId == 66) beatmap.IsSkinIgnored = reader.GetBoolean();
        //else if (nodeId == 67) beatmap.IsStoryboardDisabled = reader.GetBoolean();
        //else if (nodeId == 68) beatmap.IsVideoDisabled = reader.GetBoolean();
        //else if (nodeId == 69) beatmap.IsVisualOverride = reader.GetBoolean();
        //else if (nodeId == 71) beatmap.ManiaScrollSpeed = reader.GetByte();
    }

    private static void FillStarRating(ref SimpleBeatmapInfo beatmapInfo, OsuDbReader osuDbReader, DbGameMode index)
    {
        while (osuDbReader.Read())
        {
            if (osuDbReader.NodeType == NodeType.ArrayEnd) break;
            var data = osuDbReader.GetIntDoublePair();
            var mods = (Mods)data.IntValue;
            if (mods != Mods.None) continue;

            if (index == DbGameMode.Circle) beatmapInfo.DefaultStarRatingStd = data.DoubleValue;
            else if (index == DbGameMode.Taiko) beatmapInfo.DefaultStarRatingTaiko = data.DoubleValue;
            else if (index == DbGameMode.Catch) beatmapInfo.DefaultStarRatingCtB = data.DoubleValue;
            else if (index == DbGameMode.Mania) beatmapInfo.DefaultStarRatingMania = data.DoubleValue;
        }
    }
}