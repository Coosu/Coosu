using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class BasicEventExtensions
    {
        public static bool EqualsInitialPosition(this Move move, Sprite sprite)
        {
            return move.StartX.Equals(sprite.DefaultX) &&
                   move.StartY.Equals(sprite.DefaultY);
        }

        public static double[] ComputeFrame(this BasicEvent e, double currentTime, int? accuracy)
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

        public static double[] ComputeFrame(IEnumerable<BasicEvent> events, EventType eventType, double time, int? accuracy)
        {
            var basicEvents = events
                .OrderBy(k => k.StartTime)
                .ToList();
            if (basicEvents.Count == 0)
                return eventType.GetDefaultValue() ?? throw new NotSupportedException(eventType.Flag + " doesn't have any default value.");

            if (time < basicEvents[0].StartTime)
                return basicEvents[0].Start.ToArray();

            var e = basicEvents.FirstOrDefault(k => k.StartTime <= time && k.EndTime > time);
            if (e != null) return KeyEventExtensions.ComputeFrame(e, time, accuracy);

            var lastE = basicEvents.Last(k => k.EndTime <= time);
            return lastE.End.ToArray();
        }

        public static List<IKeyEvent> ComputeDiscretizedEvents(this BasicEvent e,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            var eventList = new List<IKeyEvent>();
            var targetEventType = e.EventType;

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var thisTime = startTime - (startTime % discretizingInterval);
            var nextTime = startTime - (startTime % discretizingInterval) + discretizingInterval;
            if (nextTime > endTime) nextTime = endTime;
            double[] reusableValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);

            eventList.Add(BasicEvent.Create(targetEventType, LinearEase.Instance,
                startTime, nextTime, e.Start.ToArray(), reusableValue));

            while (nextTime < endTime)
            {
                thisTime += discretizingInterval;
                nextTime += discretizingInterval;
                if (nextTime > endTime) nextTime = endTime;
                double[] newValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);
                eventList.Add(BasicEvent.Create(targetEventType, LinearEase.Instance, thisTime, nextTime,
                    reusableValue.ToArray(), newValue));
                reusableValue = newValue;
            }

            return eventList;
        }
    }
}