using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Utils
{
    public static class CommonEventExtensions
    {
        public static readonly ReadOnlyDictionary<EventType, double[]> DefaultDictionary = new(
            new Dictionary<EventType, double[]>
            {
                [EventTypes.Fade] = new[] { 1d },
                [EventTypes.Scale] = new[] { 1d },
                [EventTypes.Vector] = new[] { 1d, 1d },
                [EventTypes.Rotate] = new[] { 0d },
                [EventTypes.Color] = new[] { 255d, 255d, 255d },
            });

        public static double[]? GetDefaultValue(this EventType eventType)
        {
            return DefaultDictionary.ContainsKey(eventType)
                ? DefaultDictionary[eventType]
                : null;
        }

        public static double[]? GetDefaultValue(this ICommonEvent e)
        {
            return GetDefaultValue(e.EventType);
        }

        public static double[] RelativeCompute(this EventType eventType, double[] source, double[] relativeVal)
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

                value[i] = source[i] + relativeVal[i];
            }

            return value;
        }

        public static bool IsDefault(this ICommonEvent e)
        {
            return DefaultDictionary.ContainsKey(e.EventType) &&
                   e.Start.SequenceEqual(DefaultDictionary[e.EventType]);
        }

        public static double[] ComputeFrame(this ICommonEvent e, double currentTime)
        {
            var easing = e.Easing;
            var size = e.EventType.Size;

            var start = e.Start;
            var end = e.End;

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var normalizedTime = (currentTime - startTime) / (endTime - startTime);
            var easedTime = easing.Ease(normalizedTime);

            var value = new double[size];
            for (int i = 0; i < size; i++)
            {
                value[i] = (end[i] - start[i]) * easedTime + start[i];
            }

            return value;
        }
    }
}