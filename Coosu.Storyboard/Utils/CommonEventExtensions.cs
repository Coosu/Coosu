using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

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

        public static double[]? GetDefaultValue(this EventType eventType, ICameraUsable sprite)
        {
            if (eventType == EventTypes.Move) return new[] { sprite.DefaultX, sprite.DefaultY };
            if (eventType == EventTypes.MoveX) return new[] { sprite.DefaultX };
            if (eventType == EventTypes.MoveY) return new[] { sprite.DefaultY };
            return GetDefaultValue(eventType);
        }

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

        public static bool IsDefault(this ICommonEvent e)
        {
            return DefaultDictionary.ContainsKey(e.EventType) &&
                   e.Start.SequenceEqual(DefaultDictionary[e.EventType]);
        }

        public static double[] ComputeFrame(this CommonEvent e, double currentTime, int? accuracy)
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
                var val = (end[i] - start[i]) * easedTime + start[i];
                if (accuracy == null) value[i] = val;
                else value[i] = Math.Round(val, accuracy.Value);
            }

            return value;
        }
        public static double[] ComputeRelative(this EventType eventType, double[] source, double[] relativeVal, int? accuracy = null)
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

        public static List<ICommonEvent> ComputeDiscretizedEvents(this CommonEvent e,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            var eventList = new List<ICommonEvent>();
            var targetEventType = e.EventType;

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var thisTime = startTime - (startTime % discretizingInterval);
            var nextTime = startTime - (startTime % discretizingInterval) + discretizingInterval;
            if (nextTime > endTime) nextTime = endTime;
            double[] reusableValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);

            eventList.Add(CommonEvent.Create(targetEventType, LinearEase.Instance,
                startTime, nextTime, e.Start.ToArray(), reusableValue));

            while (nextTime < endTime)
            {
                thisTime += discretizingInterval;
                nextTime += discretizingInterval;
                if (nextTime > endTime) nextTime = endTime;
                double[] newValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);
                eventList.Add(CommonEvent.Create(targetEventType, LinearEase.Instance, thisTime, nextTime,
                    reusableValue.ToArray(), newValue));
                reusableValue = newValue;
            }

            return eventList;
        }
    }
}