using System;
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
                    beatmap = default;
                }
            }

            if (nodeType == NodeType.ArrayEnd &&
                reader.Path?.EndsWith("Beatmaps[]", StringComparison.Ordinal) == true)
            {
                yield break;
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

            switch (itemIndex)
            {
                case 0:
                    beatmap.Artist = reader.GetString();
                    break;
                case 1:
                    beatmap.ArtistUnicode = reader.GetString();
                    break;
                case 2:
                    beatmap.Title = reader.GetString();
                    break;
                case 3:
                    beatmap.TitleUnicode = reader.GetString();
                    break;
                case 4:
                    beatmap.Creator = reader.GetString();
                    break;
                case 5:
                    beatmap.Difficulty = reader.GetString();
                    break;
                case 6:
                    beatmap.AudioFileName = reader.GetString();
                    break;
                case 7:
                    beatmap.MD5Hash = reader.GetString();
                    break;
                case 8:
                    beatmap.FileName = reader.GetString();
                    break;
                case 9:
                    beatmap.RankedStatus = (RankedStatus)reader.GetByte();
                    break;
                case 10:
                    beatmap.CirclesCount = reader.GetInt16();
                    break;
                case 11:
                    beatmap.SlidersCount = reader.GetInt16();
                    break;
                case 12:
                    beatmap.SpinnersCount = reader.GetInt16();
                    break;
                case 13:
                    beatmap.LastModified = reader.GetDateTime();
                    break;
                case 14:
                    beatmap.ApproachRate = reader.GetSingle();
                    break;
                case 15:
                    beatmap.CircleSize = reader.GetSingle();
                    break;
                case 16:
                    beatmap.HpDrain = reader.GetSingle();
                    break;
                case 17:
                    beatmap.OverallDifficulty = reader.GetSingle();
                    break;
                case 18:
                    beatmap.SliderVelocity = reader.GetDouble();
                    break;
                case 20:
                    FillStarRating(beatmap.StarRatingStd = new(), reader);
                    break;
                case 22:
                    FillStarRating(beatmap.StarRatingTaiko = new(), reader);
                    break;
                case 24:
                    FillStarRating(beatmap.StarRatingCtb = new(), reader);
                    break;
                case 26:
                    FillStarRating(beatmap.StarRatingMania = new(), reader);
                    break;
                case 27:
                    beatmap.DrainTime = reader.GetInt32();
                    break;
                case 28:
                    beatmap.TotalTime = reader.GetInt32();
                    break;
                case 29:
                    beatmap.AudioPreviewTime = reader.GetInt32();
                    break;
                case 30:
                    arrayCount = reader.GetInt32();
                    break;
                case 31:
                    {
                        if (arrayCount > 0)
                        {
                            FillTimingPoints(beatmap.TimingPoints = new(arrayCount), reader);
                        }

                        break;
                    }
                case 32:
                    beatmap.BeatmapId = reader.GetInt32();
                    break;
                case 33:
                    beatmap.BeatmapSetId = reader.GetInt32();
                    break;
                case 34:
                    beatmap.ThreadId = reader.GetInt32();
                    break;
                case 35:
                    beatmap.GradeStandard = (Grade)reader.GetByte();
                    break;
                case 36:
                    beatmap.GradeTaiko = (Grade)reader.GetByte();
                    break;
                case 37:
                    beatmap.GradeCtb = (Grade)reader.GetByte();
                    break;
                case 38:
                    beatmap.GradeMania = (Grade)reader.GetByte();
                    break;
                case 39:
                    beatmap.LocalOffset = reader.GetInt16();
                    break;
                case 40:
                    beatmap.StackLeniency = reader.GetSingle();
                    break;
                case 41:
                    beatmap.GameMode = (DbGameMode)reader.GetByte();
                    break;
                case 42:
                    beatmap.Source = reader.GetString();
                    break;
                case 43:
                    beatmap.Tags = reader.GetString();
                    break;
                case 44:
                    beatmap.OnlineOffset = reader.GetInt16();
                    break;
                case 45:
                    beatmap.TitleFont = reader.GetString();
                    break;
                case 46:
                    beatmap.IsUnplayed = reader.GetBoolean();
                    break;
                case 47:
                    beatmap.LastPlayed = reader.GetDateTime();
                    break;
                case 48:
                    beatmap.IsOsz2 = reader.GetBoolean();
                    break;
                case 49:
                    beatmap.FolderName = reader.GetString();
                    break;
                case 50:
                    beatmap.LastTimeChecked = reader.GetDateTime();
                    break;
                case 51:
                    beatmap.IsSoundIgnored = reader.GetBoolean();
                    break;
                case 52:
                    beatmap.IsSkinIgnored = reader.GetBoolean();
                    break;
                case 53:
                    beatmap.IsStoryboardDisabled = reader.GetBoolean();
                    break;
                case 54:
                    beatmap.IsVideoDisabled = reader.GetBoolean();
                    break;
                case 55:
                    beatmap.IsVisualOverride = reader.GetBoolean();
                    break;
                case 57:
                    beatmap.ManiaScrollSpeed = reader.GetByte();
                    break;
            }
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