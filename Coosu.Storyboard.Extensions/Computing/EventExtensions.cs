using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class EventExtensions
    {
        internal static readonly ReadOnlyDictionary<EventType, float[]> IneffectiveDictionary = new(
            new Dictionary<EventType, float[]>
            {
                [EventTypes.Fade] = new[] { 0f },
                [EventTypes.Scale] = new[] { 0f },
                [EventTypes.Vector] = new[] { 0f, 0f },
                [EventTypes.Color] = new[] { 0f, 0f, 0f },
            });

        internal static readonly ReadOnlyDictionary<EventType, float[]> DefaultDictionary = new(
            new Dictionary<EventType, float[]>
            {
                [EventTypes.Fade] = new[] { 1f },
                [EventTypes.Scale] = new[] { 1f },
                [EventTypes.Vector] = new[] { 1f, 1f },
                [EventTypes.Rotate] = new[] { 0f },
                [EventTypes.Color] = new[] { 255f, 255f, 255f },
            });

        public static float[] GetIneffectiveValue(this IKeyEvent e)
        {
            return IneffectiveDictionary.ContainsKey(e.EventType)
                ? IneffectiveDictionary[e.EventType]
                : EmptyArray<float>.Value;
        }

        public static float[]? GetDefaultValue(this EventType eventType)
        {
            return DefaultDictionary.ContainsKey(eventType)
                ? DefaultDictionary[eventType]
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

        public static float[] ComputeRelative(this EventType eventType, float[] source, float[] relativeVal,
            int? accuracy = null)
        {
            if (eventType.Size < 1)
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Only support sized event type.");
            var value = new float[eventType.Size];
            for (int i = 0; i < eventType.Size; i++)
            {
                //if (eventType == EventTypes.Fade ||eventType==EventTypes.Scale||eventType==)
                //{
                //    value[i] = source[i] * relativeVal[i];
                //}
                if (accuracy == null)
                    value[i] = source[i] + relativeVal[i];
                else
                    value[i] = (float)Math.Round(source[i] + relativeVal[i], accuracy.Value);
            }

            return value;
        }
    }
}
