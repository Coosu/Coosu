using System.Collections.Generic;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Serialization;

public static class OsuDbReaderExtensions
{
    public static IEnumerable<Beatmap> EnumerateBeatmaps(this OsuDbReader reader)
    {
        Beatmap? beatmap = default;
        int count = 0;
        int itemIndex = -1;
        int arrayCount = 0;

        while (reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;

            if (nodeType == NodeType.ObjectStart)
            {
                beatmap = new Beatmap();
                continue;
            }

            if (nodeType == NodeType.ObjectEnd)
            {
                if (beatmap != null)
                {
                    itemIndex = -1;
                    yield return beatmap;
                    count++;
                    if (count == 2999)
                    {

                    }
                    beatmap = default;
                }
            }

            if (beatmap == default)
            {
                continue;
            }

            if (nodeType is NodeType.ArrayStart or NodeType.KeyValue)
            {
                itemIndex++;
            }
            else
            {
                continue;
            }

            if (itemIndex == 0) beatmap.Artist = reader.GetString();
            else if (itemIndex == 1) beatmap.ArtistUnicode = reader.GetString();
            else if (itemIndex == 2) beatmap.Title = reader.GetString();
            else if (itemIndex == 3) beatmap.TitleUnicode = reader.GetString();
            else if (itemIndex == 4) beatmap.Creator = reader.GetString();
            else if (itemIndex == 5) beatmap.Difficulty = reader.GetString();
            else if (itemIndex == 6) beatmap.AudioFileName = reader.GetString();
            else if (itemIndex == 7) beatmap.MD5Hash = reader.GetString();
            else if (itemIndex == 8) beatmap.FileName = reader.GetString();
            else if (itemIndex == 9) beatmap.RankedStatus = (RankedStatus)reader.GetByte();
            else if (itemIndex == 10) beatmap.CirclesCount = reader.GetInt16();
            else if (itemIndex == 11) beatmap.SlidersCount = reader.GetInt16();
            else if (itemIndex == 12) beatmap.SpinnersCount = reader.GetInt16();
            else if (itemIndex == 13) beatmap.LastModified = reader.GetDateTime();
            else if (itemIndex == 14) beatmap.ApproachRate = reader.GetSingle();
            else if (itemIndex == 15) beatmap.CircleSize = reader.GetSingle();
            else if (itemIndex == 16) beatmap.HpDrain = reader.GetSingle();
            else if (itemIndex == 17) beatmap.OverallDifficulty = reader.GetSingle();
            else if (itemIndex == 18) beatmap.SliderVelocity = reader.GetDouble();
            else if (itemIndex == 20) FillStarRating(beatmap.StarRatingStd = new(), reader);
            else if (itemIndex == 22) FillStarRating(beatmap.StarRatingTaiko = new(), reader);
            else if (itemIndex == 24) FillStarRating(beatmap.StarRatingCtb = new(), reader);
            else if (itemIndex == 26) FillStarRating(beatmap.StarRatingMania = new(), reader);
            else if (itemIndex == 27) beatmap.DrainTime = reader.GetInt32();
            else if (itemIndex == 28) beatmap.TotalTime = reader.GetInt32();
            else if (itemIndex == 29) beatmap.AudioPreviewTime = reader.GetInt32();
            else if (itemIndex == 30) arrayCount = reader.GetInt32();
            else if (itemIndex == 31) FillTimingPoints(beatmap.TimingPoints = new(arrayCount), reader);
            else if (itemIndex == 32) beatmap.BeatmapId = reader.GetInt32();
            else if (itemIndex == 33) beatmap.BeatmapSetId = reader.GetInt32();
            else if (itemIndex == 34) beatmap.ThreadId = reader.GetInt32();
            else if (itemIndex == 35) beatmap.GradeStandard = (Grade)reader.GetByte();
            else if (itemIndex == 36) beatmap.GradeTaiko = (Grade)reader.GetByte();
            else if (itemIndex == 37) beatmap.GradeCtb = (Grade)reader.GetByte();
            else if (itemIndex == 38) beatmap.GradeMania = (Grade)reader.GetByte();
            else if (itemIndex == 39) beatmap.LocalOffset = reader.GetInt16();
            else if (itemIndex == 40) beatmap.StackLeniency = reader.GetSingle();
            else if (itemIndex == 41) beatmap.GameMode = (DbGameMode)reader.GetByte();
            else if (itemIndex == 42) beatmap.Source = reader.GetString();
            else if (itemIndex == 43) beatmap.Tags = reader.GetString();
            else if (itemIndex == 44) beatmap.OnlineOffset = reader.GetInt16();
            else if (itemIndex == 45) beatmap.TitleFont = reader.GetString();
            else if (itemIndex == 46) beatmap.IsUnplayed = reader.GetBoolean();
            else if (itemIndex == 47) beatmap.LastPlayed = reader.GetDateTime();
            else if (itemIndex == 48) beatmap.IsOsz2 = reader.GetBoolean();
            else if (itemIndex == 49) beatmap.FolderName = reader.GetString();
            else if (itemIndex == 50) beatmap.LastTimeChecked = reader.GetDateTime();
            else if (itemIndex == 51) beatmap.IsSoundIgnored = reader.GetBoolean();
            else if (itemIndex == 52) beatmap.IsSkinIgnored = reader.GetBoolean();
            else if (itemIndex == 53) beatmap.IsStoryboardDisabled = reader.GetBoolean();
            else if (itemIndex == 54) beatmap.IsVideoDisabled = reader.GetBoolean();
            else if (itemIndex == 55) beatmap.IsVisualOverride = reader.GetBoolean();
            else if (itemIndex == 57) beatmap.ManiaScrollSpeed = reader.GetByte();
        }
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