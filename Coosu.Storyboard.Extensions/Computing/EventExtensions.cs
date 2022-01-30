using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class EventExtensions
    {
        internal static readonly ReadOnlyDictionary<string, float[]> IneffectiveDictionary = new(
            new Dictionary<string, float[]>
            {
                [EventTypes.Fade.Flag] = new[] { 0f },
                [EventTypes.Scale.Flag] = new[] { 0f },
                [EventTypes.Vector.Flag] = new[] { 0f, 0f },
                [EventTypes.Color.Flag] = new[] { 0f, 0f, 0f },
            });

        internal static readonly ReadOnlyDictionary<string, float[]> DefaultDictionary = new(
            new Dictionary<string, float[]>
            {
                [EventTypes.Fade.Flag] = new[] { 1f },
                [EventTypes.Scale.Flag] = new[] { 1f },
                [EventTypes.Vector.Flag] = new[] { 1f, 1f },
                [EventTypes.Rotate.Flag] = new[] { 0f },
                [EventTypes.Color.Flag] = new[] { 255f, 255f, 255f },
            });

        public static float[] GetIneffectiveValue(this IKeyEvent e)
        {
            return IneffectiveDictionary.ContainsKey(e.EventType.Flag)
                ? IneffectiveDictionary[e.EventType.Flag]
                : EmptyArray<float>.Value;
        }

        public static float[]? GetDefaultValue(this EventType eventType)
        {
            return DefaultDictionary.ContainsKey(eventType.Flag)
                ? DefaultDictionary[eventType.Flag]
                : null;
        }

        public static float[]? GetDefaultValue(this EventType eventType, ICameraUsable? sprite)
        {
            if (sprite == null) return GetDefaultValue(eventType);

            if (eventType == EventTypes.Move) return new[] { sprite.DefaultX, sprite.DefaultY };
            if (eventType == EventTypes.MoveX) return new[] { sprite.DefaultX };
            if (eventType == EventTypes.MoveY) return new[] { sprite.DefaultY };
            return GetDefaultValue(eventType);
        }

        public static List<float> ComputeRelative(this EventType eventType,
            IReadOnlyList<float> source,
            IReadOnlyList<float> relativeVal,
            int? accuracy = null)
        {
            if (eventType.Size < 1)
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Only support sized event type.");
            var list = new List<float>(eventType.Size);
            for (int i = 0; i < eventType.Size; i++)
            {
                //if (eventType == EventTypes.Fade ||eventType==EventTypes.Scale||eventType==)
                //{
                //    value[i] = source[i] * relativeVal[i];
                //}
                if (accuracy == null)
                    list.Add(source[i] + relativeVal[i]);
                else
                    list.Add((float)Math.Round(source[i] + relativeVal[i], accuracy.Value));
            }

            return list;
        }
    }
}
