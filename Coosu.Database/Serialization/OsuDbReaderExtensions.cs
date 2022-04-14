using System;
using System.Collections.Generic;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Serialization;

public static class OsuDbReaderExtensions
{
    private static readonly ObjectStructure _beatmapStructure;

    static OsuDbReaderExtensions()
    {
        var mappingHelper = OsuDbReader.StructureHelper;
        var rootStructure = (ObjectStructure)mappingHelper.RootStructure;
        var arrayStructure = (ArrayStructure)rootStructure.Structures[6];
        _beatmapStructure = arrayStructure.ObjectStructure!;
    }

    public static IEnumerable<Beatmap> EnumerateBeatmaps(this OsuDbReader reader)
    {
        Beatmap? beatmap = default;
        int count = 0;
        int arrayCount = 0;

        while (reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;
            var nodeId = reader.NodeId;

            if (nodeType == NodeType.ObjectStart)
            {
                beatmap = new Beatmap();
                continue;
            }

            if (nodeType == NodeType.ObjectEnd)
            {
                if (beatmap != null)
                {
                    yield return beatmap;
                    count++;
                    beatmap = default;
                }
            }

            if (nodeType == NodeType.ArrayEnd && nodeId == 7)
            {
                yield break;
            }

            if (beatmap == default)
            {
                continue;
            }

            if (nodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }

            FillProperty(reader, nodeId, beatmap, ref arrayCount);
        }
    }

    private static void FillProperty(OsuDbReader reader, int nodeId, Beatmap beatmap, ref int arrayCount)
    {
        if (nodeId == 9) beatmap.Artist = reader.GetString();
        else if (nodeId == 10) beatmap.ArtistUnicode = reader.GetString();
        else if (nodeId == 11) beatmap.Title = reader.GetString();
        else if (nodeId == 12) beatmap.TitleUnicode = reader.GetString();
        else if (nodeId == 13) beatmap.Creator = reader.GetString();
        else if (nodeId == 14) beatmap.Difficulty = reader.GetString();
        else if (nodeId == 15) beatmap.AudioFileName = reader.GetString();
        else if (nodeId == 16) beatmap.Md5Hash = reader.GetString();
        else if (nodeId == 17) beatmap.FileName = reader.GetString();
        else if (nodeId == 18) beatmap.RankedStatus = (RankedStatus)reader.GetByte();
        else if (nodeId == 19) beatmap.CirclesCount = reader.GetInt16();
        else if (nodeId == 20) beatmap.SlidersCount = reader.GetInt16();
        else if (nodeId == 21) beatmap.SpinnersCount = reader.GetInt16();
        else if (nodeId == 22) beatmap.LastModified = reader.GetDateTime();
        else if (nodeId == 23) beatmap.ApproachRate = reader.GetSingle();
        else if (nodeId == 24) beatmap.CircleSize = reader.GetSingle();
        else if (nodeId == 25) beatmap.HpDrain = reader.GetSingle();
        else if (nodeId == 26) beatmap.OverallDifficulty = reader.GetSingle();
        else if (nodeId == 27) beatmap.SliderVelocity = reader.GetDouble();
        else if (nodeId == 29) FillStarRating(beatmap.StarRatingStd = new(), reader);
        else if (nodeId == 32) FillStarRating(beatmap.StarRatingTaiko = new(), reader);
        else if (nodeId == 35) FillStarRating(beatmap.StarRatingCtb = new(), reader);
        else if (nodeId == 38) FillStarRating(beatmap.StarRatingMania = new(), reader);
        else if (nodeId == 40) beatmap.DrainTime = TimeSpan.FromSeconds(reader.GetInt32());
        else if (nodeId == 41) beatmap.TotalTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 42) beatmap.AudioPreviewTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 43) arrayCount = reader.GetInt32();
        else if (nodeId == 44)
        {
            if (arrayCount > 0) FillTimingPoints(beatmap.TimingPoints = new(arrayCount), reader);
        }
        else if (nodeId == 46) beatmap.BeatmapId = reader.GetInt32();
        else if (nodeId == 47) beatmap.BeatmapSetId = reader.GetInt32();
        else if (nodeId == 48) beatmap.ThreadId = reader.GetInt32();
        else if (nodeId == 49) beatmap.GradeStandard = (Grade)reader.GetByte();
        else if (nodeId == 50) beatmap.GradeTaiko = (Grade)reader.GetByte();
        else if (nodeId == 51) beatmap.GradeCtb = (Grade)reader.GetByte();
        else if (nodeId == 52) beatmap.GradeMania = (Grade)reader.GetByte();
        else if (nodeId == 53) beatmap.LocalOffset = reader.GetInt16();
        else if (nodeId == 54) beatmap.StackLeniency = reader.GetSingle();
        else if (nodeId == 55) beatmap.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == 56) beatmap.Source = reader.GetString();
        else if (nodeId == 57) beatmap.Tags = reader.GetString();
        else if (nodeId == 58) beatmap.OnlineOffset = reader.GetInt16();
        else if (nodeId == 59) beatmap.TitleFont = reader.GetString();
        else if (nodeId == 60) beatmap.IsUnplayed = reader.GetBoolean();
        else if (nodeId == 61) beatmap.LastPlayed = reader.GetDateTime();
        else if (nodeId == 62) beatmap.IsOsz2 = reader.GetBoolean();
        else if (nodeId == 63) beatmap.FolderName = reader.GetString();
        else if (nodeId == 64) beatmap.LastTimeChecked = reader.GetDateTime();
        else if (nodeId == 65) beatmap.IsSoundIgnored = reader.GetBoolean();
        else if (nodeId == 66) beatmap.IsSkinIgnored = reader.GetBoolean();
        else if (nodeId == 67) beatmap.IsStoryboardDisabled = reader.GetBoolean();
        else if (nodeId == 68) beatmap.IsVideoDisabled = reader.GetBoolean();
        else if (nodeId == 69) beatmap.IsVisualOverride = reader.GetBoolean();
        else if (nodeId == 71) beatmap.ManiaScrollSpeed = reader.GetByte();
    }

    private static void FillTimingPoints(List<TimingPoint> timingPoints, OsuDbReader osuDbReader)
    {
        while (osuDbReader.Read())
        {
            if (osuDbReader.NodeType == NodeType.ArrayEnd) break;
            var timingPoint = osuDbReader.GetTimingPoint();
            timingPoints.Add(timingPoint);
        }
    }

    private static void FillStarRating(IDictionary<Mods, double> dictionary, OsuDbReader osuDbReader)
    {
        while (osuDbReader.Read())
        {
            if (osuDbReader.NodeType == NodeType.ArrayEnd) break;
            var data = osuDbReader.GetIntDoublePair();
            var mods = (Mods)data.IntValue;
            dictionary.Add(mods, data.DoubleValue);
        }
    }
}