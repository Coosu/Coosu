using System;
using System.Collections.Generic;
using Coosu.Database.DataTypes;
using Coosu.Database.Generated;
using Coosu.Database.Internal;

namespace Coosu.Database.Serialization;

public static class OsuDbReaderOsuDbExtensions
{
    private static readonly ObjectStructure BeatmapStructure;

    static OsuDbReaderOsuDbExtensions()
    {
        var mappingHelper = StructureHelperPool.GetHelperByType(StructureHelperPool.TypeOsuDb);
        var rootStructure = (ObjectStructure)mappingHelper.RootStructure;
        var arrayStructure = (ArrayStructure)rootStructure.Structures[6];
        BeatmapStructure = arrayStructure.ObjectStructure!;
    }

    public static IEnumerable<Beatmap> EnumerateBeatmaps(this OsuDbReader reader)
    {
        Beatmap? beatmap = default;
        //int index = 0;
        int arrayCount = 0;

        while (!reader.IsEndOfStream && reader.Read())
        {
            //Console.WriteLine("Name = " + GetString(reader.Name) + "; " +
            //                  "Path = " + GetString(reader.Path) + "; " +
            //                  "Value = " + GetString(reader.Value) + "; " +
            //                  "NodeType = " + GetString(reader.NodeType) + "; " +
            //                  "DataType = " + GetString(reader.DataType) + "; " +
            //                  "TargetType = " + GetString(reader.TargetType));

            if (reader.NodeType == NodeType.ObjectStart)
            {
                beatmap = new Beatmap();
                //Console.WriteLine("Beatmap index: " + index);
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && beatmap != null)
            {
                yield return beatmap;
                //index++;
                beatmap = default;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == (int)NodeId.OsuDb_BeatmapArray)
            {
                yield break;
            }

            if (beatmap == default)
            {
                continue;
            }

            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }

            FillProperty(reader, beatmap, ref arrayCount);
        }
    }

    private static void FillProperty(OsuDbReader reader, Beatmap beatmap, ref int arrayCount)
    {
        var nodeId = (NodeId)reader.NodeId;
        if (nodeId == NodeId.Artist) beatmap.Artist = reader.GetString();
        else if (nodeId == NodeId.ArtistUnicode) beatmap.ArtistUnicode = reader.GetString();
        else if (nodeId == NodeId.Title) beatmap.Title = reader.GetString();
        else if (nodeId == NodeId.TitleUnicode) beatmap.TitleUnicode = reader.GetString();
        else if (nodeId == NodeId.Creator) beatmap.Creator = reader.GetString();
        else if (nodeId == NodeId.Difficulty) beatmap.Difficulty = reader.GetString();
        else if (nodeId == NodeId.AudioFileName) beatmap.AudioFileName = reader.GetString();
        else if (nodeId == NodeId.Md5Hash) beatmap.Md5Hash = reader.GetString();
        else if (nodeId == NodeId.FileName) beatmap.FileName = reader.GetString();
        else if (nodeId == NodeId.RankedStatus) beatmap.RankedStatus = (RankedStatus)reader.GetByte();
        else if (nodeId == NodeId.CirclesCount) beatmap.CirclesCount = reader.GetInt16();
        else if (nodeId == NodeId.SlidersCount) beatmap.SlidersCount = reader.GetInt16();
        else if (nodeId == NodeId.SpinnersCount) beatmap.SpinnersCount = reader.GetInt16();
        else if (nodeId == NodeId.LastModified) beatmap.LastModified = reader.GetDateTime();
        else if (nodeId == NodeId.ApproachRate) beatmap.ApproachRate = reader.GetSingle();
        else if (nodeId == NodeId.CircleSize) beatmap.CircleSize = reader.GetSingle();
        else if (nodeId == NodeId.HpDrain) beatmap.HpDrain = reader.GetSingle();
        else if (nodeId == NodeId.OverallDifficulty) beatmap.OverallDifficulty = reader.GetSingle();
        else if (nodeId == NodeId.SliderVelocity) beatmap.SliderVelocity = reader.GetDouble();
        else if (nodeId == NodeId.StarRatingStdArray) FillStarRating(beatmap.StarRatingStd = new(), reader);
        else if (nodeId == NodeId.StarRatingTaikoArray) FillStarRating(beatmap.StarRatingTaiko = new(), reader);
        else if (nodeId == NodeId.StarRatingCtbArray) FillStarRating(beatmap.StarRatingCtb = new(), reader);
        else if (nodeId == NodeId.StarRatingManiaArray) FillStarRating(beatmap.StarRatingMania = new(), reader);
        else if (nodeId == NodeId.DrainTime) beatmap.DrainTime = TimeSpan.FromSeconds(reader.GetInt32());
        else if (nodeId == NodeId.TotalTime) beatmap.TotalTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == NodeId.AudioPreviewTime) beatmap.AudioPreviewTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == NodeId.TimingPointCount) arrayCount = reader.GetInt32();
        else if (nodeId == NodeId.TimingPointArray)
        {
            if (arrayCount > 0) FillTimingPoints(beatmap.TimingPoints = new(arrayCount), reader);
        }
        else if (nodeId == NodeId.BeatmapId) beatmap.BeatmapId = reader.GetInt32();
        else if (nodeId == NodeId.BeatmapSetId) beatmap.BeatmapSetId = reader.GetInt32();
        else if (nodeId == NodeId.ThreadId) beatmap.ThreadId = reader.GetInt32();
        else if (nodeId == NodeId.GradeStandard) beatmap.GradeStandard = (Grade)reader.GetByte();
        else if (nodeId == NodeId.GradeTaiko) beatmap.GradeTaiko = (Grade)reader.GetByte();
        else if (nodeId == NodeId.GradeCtb) beatmap.GradeCtb = (Grade)reader.GetByte();
        else if (nodeId == NodeId.GradeMania) beatmap.GradeMania = (Grade)reader.GetByte();
        else if (nodeId == NodeId.LocalOffset) beatmap.LocalOffset = reader.GetInt16();
        else if (nodeId == NodeId.StackLeniency) beatmap.StackLeniency = reader.GetSingle();
        else if (nodeId == NodeId.GameMode) beatmap.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == NodeId.Source) beatmap.Source = reader.GetString();
        else if (nodeId == NodeId.Tags) beatmap.Tags = reader.GetString();
        else if (nodeId == NodeId.OnlineOffset) beatmap.OnlineOffset = reader.GetInt16();
        else if (nodeId == NodeId.TitleFont) beatmap.TitleFont = reader.GetString();
        else if (nodeId == NodeId.IsUnplayed) beatmap.IsUnplayed = reader.GetBoolean();
        else if (nodeId == NodeId.LastPlayed) beatmap.LastPlayed = reader.GetDateTime();
        else if (nodeId == NodeId.IsOsz2) beatmap.IsOsz2 = reader.GetBoolean();
        else if (nodeId == NodeId.FolderName) beatmap.FolderName = reader.GetString();
        else if (nodeId == NodeId.LastTimeChecked) beatmap.LastTimeChecked = reader.GetDateTime();
        else if (nodeId == NodeId.IsSoundIgnored) beatmap.IsSoundIgnored = reader.GetBoolean();
        else if (nodeId == NodeId.IsSkinIgnored) beatmap.IsSkinIgnored = reader.GetBoolean();
        else if (nodeId == NodeId.IsStoryboardDisabled) beatmap.IsStoryboardDisabled = reader.GetBoolean();
        else if (nodeId == NodeId.IsVideoDisabled) beatmap.IsVideoDisabled = reader.GetBoolean();
        else if (nodeId == NodeId.IsVisualOverride) beatmap.IsVisualOverride = reader.GetBoolean();
        else if (nodeId == NodeId.ManiaScrollSpeed) beatmap.ManiaScrollSpeed = reader.GetByte();
    }

    private static void FillTimingPoints(List<TimingPoint> timingPoints, OsuDbReader reader)
    {
        while (!reader.IsEndOfStream && reader.Read())
        {
            if (reader.NodeType == NodeType.ArrayEnd) break;
            var timingPoint = reader.GetTimingPoint();
            timingPoints.Add(timingPoint);
        }
    }

    private static void FillStarRating(Dictionary<Mods, float> dictionary, OsuDbReader reader)
    {
        while (!reader.IsEndOfStream && reader.Read())
        {
            if (reader.NodeType == NodeType.ArrayEnd) break;
            var data = reader.GetIntSinglePair();
            var mods = (Mods)data.IntValue;
            dictionary.Add(mods, data.SingleValue);
        }
    }

    //private static string GetString(object? obj)
    //{
    //    if (obj == null) return "NULL";
    //    return obj.ToString();
    //}
}