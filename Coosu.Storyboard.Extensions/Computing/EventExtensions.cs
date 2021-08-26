using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class EventExtensions
    {
        internal static readonly ReadOnlyDictionary<EventType, double[]> IneffectiveDictionary = new(
            new Dictionary<EventType, double[]>
            {
                [EventTypes.Fade] = new[] { 0d },
                [EventTypes.Scale] = new[] { 0d },
                [EventTypes.Vector] = new[] { 0d, 0d },
                [EventTypes.Color] = new[] { 0d, 0d, 0d },
            });

        internal static readonly ReadOnlyDictionary<EventType, double[]> DefaultDictionary = new(
            new Dictionary<EventType, double[]>
            {
                [EventTypes.Fade] = new[] { 1d },
                [EventTypes.Scale] = new[] { 1d },
                [EventTypes.Vector] = new[] { 1d, 1d },
                [EventTypes.Rotate] = new[] { 0d },
                [EventTypes.Color] = new[] { 255d, 255d, 255d },
            });

        public static double[] GetIneffectiveValue(this IKeyEvent e)
        {
            return IneffectiveDictionary.ContainsKey(e.EventType)
                ? IneffectiveDictionary[e.EventType]
                : EmptyArray<double>.Value;
        }

        public static double[]? GetDefaultValue(this EventType eventType)
        {
            return DefaultDictionary.ContainsKey(eventType)
                ? DefaultDictionary[eventType]
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

        public static double[] ComputeRelative(this EventType eventType, double[] source, double[] relativeVal,
            int? accuracy = null)
        {
            if (eventType.Size < 1)
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Only support sized event type.");
            var value = new double[eventType.Size];
            for (int i = 0; i < eventType.Size; i++)
            {
                //if (eventType == EventTypes.Fade ||eventType==EventTypes.Scale||eventType==)
                //{
                //    value[i] = source[i] * relativeVal[i];
                //}
                if (accuracy == null)
                    value[i] = source[i] + relativeVal[i];
                else
                    value[i] = Math.Round(source[i] + relativeVal[i], accuracy.Value);
            }

            return value;
        }
    }
}
