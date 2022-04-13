using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Database.Annotations;
using Coosu.Database.DataTypes;
using Coosu.Database.Handlers;
using Coosu.Database.Serialization;
using Coosu.Database.Utils;
using B = Coosu.Database.DataTypes.Beatmap;

namespace Coosu.Database.Internal;


public interface IMapping
{
}
public class PropertyMapping : IMapping
{
    public ClassMapping BaseClass { get; set; }
    public Type TargetType { get; set; }
    public IValueHandler? ValueHandler { get; set; }
}

public class ArrayMapping : IMapping
{
    public ClassMapping BaseClass { get; set; }
    public string LengthDeclarationMember { get; set; }
    public Type SubItemType { get; set; }
    public ClassMapping? ClassMapping { get; set; }
    public PropertyMapping? PropertyMapping { get; set; }
    public bool IsObjectArray { get; set; }
    public Func<object>? ArrayCreation { get; set; }
}
public class ClassMapping : IMapping
{
    public Dictionary<string, IMapping> Mapping { get; set; } = new();
}

internal static class OsuDbReaderMapping
{
    private static ClassMapping _classMapping;
    private static readonly Dictionary<Type, IValueHandler> SharedHandlers = new();
    private static readonly Dictionary<string, int> ArrayCountStorage = new();

    static OsuDbReaderMapping()
    {
        var type = typeof(OsuDb);
        var mapping = _classMapping = GetClassMapping(type);
    }

    private static ClassMapping GetClassMapping(Type type)
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

    private static IValueHandler? GetSharedValueHandler(Type? type)
    {
        if (type == null) return null;
        if (SharedHandlers.TryGetValue(type, out var value))
        {
            return value;
        }

        value = (IValueHandler)Activator.CreateInstance(type);
        SharedHandlers.Add(type, value);
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