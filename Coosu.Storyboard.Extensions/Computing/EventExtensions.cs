using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Computing;

public static class EventExtensions
{
    internal static readonly ReadOnlyDictionary<string, double[]> IneffectiveDictionary = new(
        new Dictionary<string, double[]>
        {
            [EventTypes.Fade.Flag] = new[] { 0d },
            [EventTypes.Scale.Flag] = new[] { 0d },
            [EventTypes.Vector.Flag] = new[] { 0d, 0d },
            [EventTypes.Color.Flag] = new[] { 0d, 0d, 0d },
        });

    internal static readonly ReadOnlyDictionary<string, double[]> DefaultDictionary = new(
        new Dictionary<string, double[]>
        {
            [EventTypes.Fade.Flag] = new[] { 1d },
            [EventTypes.Scale.Flag] = new[] { 1d },
            [EventTypes.Vector.Flag] = new[] { 1d, 1d },
            [EventTypes.Rotate.Flag] = new[] { 0d },
            [EventTypes.Color.Flag] = new[] { 255d, 255d, 255d },
        });

    public static double[] GetIneffectiveValue(this IKeyEvent e)
    {
        return IneffectiveDictionary.ContainsKey(e.EventType.Flag)
            ? IneffectiveDictionary[e.EventType.Flag]
            : EmptyArray<double>.Value;
    }

    public static double[]? GetDefaultValue(this EventType eventType)
    {
        return DefaultDictionary.ContainsKey(eventType.Flag)
            ? DefaultDictionary[eventType.Flag]
            : null;
    }

    public static double[]? GetDefaultValue(this EventType eventType, ICameraUsable? sprite)
    {
        if (sprite == null) return GetDefaultValue(eventType);

        if (eventType == EventTypes.Move) return new[] { sprite.DefaultX, sprite.DefaultY };
        if (eventType == EventTypes.MoveX) return new[] { sprite.DefaultX };
        if (eventType == EventTypes.MoveY) return new[] { sprite.DefaultY };
        return GetDefaultValue(eventType);
    }

    public static List<double> ComputeRelative(this EventType eventType,
        IReadOnlyList<double> source,
        IReadOnlyList<double> relativeVal,
        int? accuracy = null)
    {
        if (eventType.Size < 1)
            throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Only support sized event type.");
        var list = new List<double>(eventType.Size);
        for (int i = 0; i < eventType.Size; i++)
        {
            //if (eventType == EventTypes.Fade ||eventType==EventTypes.Scale||eventType==)
            //{
            //    value[i] = source[i] * relativeVal[i];
            //}
            if (accuracy == null)
                list.Add(source[i] + relativeVal[i]);
            else
                list.Add((double)Math.Round(source[i] + relativeVal[i], accuracy.Value));
        }

        return list;
    }
}