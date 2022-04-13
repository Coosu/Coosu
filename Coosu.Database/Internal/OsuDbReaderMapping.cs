using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;
using Coosu.Database.Utils;
using B = Coosu.Database.DataTypes.Beatmap;

namespace Coosu.Database.Internal;

internal static class OsuDbReaderMapping
{
    public static readonly string[] GeneralSequence =
    {
        "OsuVersion", "FolderCount", "AccountUnlocked", "AccountUnlockDate", "PlayerName", "BeatmapCount",
        "Beatmaps[]", "UserPermission"
    };

    public static readonly DataType[] GeneralSequenceType =
    {
        DataType.Int32, DataType.Int32, DataType.Boolean, DataType.DateTime, DataType.String, DataType.Int32,
        DataType.Array, DataType.Int32
    };

    public static readonly string[] BeatmapSequence =
    {
        nameof(B.Artist), nameof(B.ArtistUnicode), nameof(B.Title), nameof(B.TitleUnicode),
        nameof(B.Creator), nameof(B.Difficulty),
        nameof(B.AudioFileName), nameof(B.MD5Hash), nameof(B.FileName),
        nameof(B.RankedStatus), nameof(B.CirclesCount), nameof(B.SlidersCount), nameof(B.SpinnersCount),
        "LastModified", "ApproachRate", "CircleSize", "HpDrain", "OverallDifficulty", "SliderVelocity",

        "StarRatingStandardCount", "StarRatingStandards[]", "StarRatingTaikoCount", "StarRatingTaikos[]",
        "StarRatingCtbCount", "StarRatingCtbs[]", "StarRatingManiaCount", "StarRatingManias[]",

        "DrainTime", "TotalTime", "AudioPreviewTime",
        "TimingPointCount", "TimingPoints[]",

        "BeatmapId", "BeatmapSetId", "ThreadId",
        "GradeStandard", "GradeTaiko", "GradeCtb", "GradeMania",
        "LocalOffset", "StackLeniency", "GameMode", "Source", "Tags", "OnlineOffset",
        "TitleFont", "IsUnplayed", "LastPlayed", "IsOsz2", "FolderName", "LastTimeChecked",
        "IsSoundIgnored", "IsSkinIgnored", "IsStoryboardDisabled", "IsVideoDisabled", "IsVisualOverride",
        "LastModification?", "ManiaScrollSpeed",
    };

    public static readonly DataType[] BeatmapSequenceType =
    {
        DataType.String, DataType.String, DataType.String, DataType.String,
        DataType.String, DataType.String,
        DataType.String, DataType.String, DataType.String,
        DataType.Byte, DataType.Int16, DataType.Int16, DataType.Int16,
        DataType.DateTime, DataType.Single, DataType.Single, DataType.Single, DataType.Single, DataType.Double,

        DataType.Int32, DataType.Array, DataType.Int32, DataType.Array,
        DataType.Int32, DataType.Array, DataType.Int32, DataType.Array,

        DataType.Int32, DataType.Int32, DataType.Int32,
        DataType.Int32, DataType.Array,

        DataType.Int32, DataType.Int32, DataType.Int32,
        DataType.Byte, DataType.Byte, DataType.Byte, DataType.Byte,
        DataType.Int16, DataType.Single, DataType.Byte, DataType.String, DataType.String, DataType.Int16,
        DataType.String, DataType.Boolean, DataType.DateTime, DataType.Boolean, DataType.String, DataType.DateTime,
        DataType.Boolean, DataType.Boolean, DataType.Boolean, DataType.Boolean, DataType.Boolean,
        DataType.Int32, DataType.Byte
    };
}