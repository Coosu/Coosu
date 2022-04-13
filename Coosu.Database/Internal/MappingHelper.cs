using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Database.Mapping;
using Coosu.Database.Mapping.Converting;
using B = Coosu.Database.DataTypes.Beatmap;

namespace Coosu.Database.Internal;

internal sealed class MappingHelper
{
    private readonly Dictionary<Type, IValueHandler> _sharedHandlers = new();
    private readonly Dictionary<string, int> _arrayCountStorage = new();

    public MappingHelper(Type type)
    {
        Mapping = GetClassMapping(type);
    }

    public ClassMapping Mapping { get; }

    private ClassMapping GetClassMapping(Type type)
    {
        var propertyMapping = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(k => !k.Name.Equals("EqualityContract", StringComparison.Ordinal))
            .ToDictionary(k => k.Name, k => k);
        var classMapping = new ClassMapping();

        foreach (var kvp in propertyMapping)
        {
            var propertyInfo = kvp.Value;

            var attribute = propertyInfo.GetCustomAttribute<OsuDbIgnoreAttribute>();
            if (attribute != null) continue;
            var arrAttr = propertyInfo.GetCustomAttribute<OsuDbArrayAttribute>();
            if (arrAttr == null)
            {
                classMapping.Mapping.Add(propertyInfo.Name, new PropertyMapping
                {
                    BaseClass = classMapping,
                    TargetType = propertyInfo.PropertyType,
                    ValueHandler = DefaultValueHandler.Instance
                });
            }
            else
            {
                var arrayMapping = new ArrayMapping
                {
                    SubItemType = arrAttr.SubItemType,
                    BaseClass = classMapping,
                    LengthDeclarationMember = type.FullName + "." + propertyMapping[arrAttr.LengthDeclaration].Name,
                    IsObjectArray = arrAttr.IsObject,
                };

                if (arrayMapping.IsObjectArray)
                {
                    arrayMapping.ClassMapping = GetClassMapping(arrAttr.SubItemType);
                }
                else
                {
                    arrayMapping.PropertyMapping = new PropertyMapping
                    {
                        BaseClass = classMapping,
                        TargetType = propertyInfo.PropertyType,
                        ValueHandler = GetSharedValueHandler(arrAttr.ValueHandler) ?? DefaultValueHandler.Instance
                    };
                }

                classMapping.Mapping.Add(propertyInfo.Name, arrayMapping);
            }
        }

        return classMapping;
    }

    private IValueHandler? GetSharedValueHandler(Type? type)
    {
        if (type == null) return null;
        if (_sharedHandlers.TryGetValue(type, out var value))
        {
            return value;
        }

        value = (IValueHandler)Activator.CreateInstance(type);
        _sharedHandlers.Add(type, value);
        return value;
    }

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