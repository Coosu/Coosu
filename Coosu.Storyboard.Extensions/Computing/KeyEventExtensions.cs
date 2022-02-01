using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class KeyEventExtensions
    {
        public static float[]? GetDefaultValue(this IKeyEvent e)
        {
            return e.EventType.GetDefaultValue();
        }

        public static bool EqualsMaxTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.EndTime.Equals(host.MaxTime());
        }

        public static bool EqualsMinTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.StartTime.Equals(host.MinTime());
        }

        public static bool IsTimeInRange(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.IsSmallerThanMaxTime(host) && e.IsLargerThanMinTime(host);
        }

        public static bool IsSmallerThanMaxTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.EndTime < host.MaxTime() ||
                   e.EqualsMultiMaxTime(host);
        }

        public static bool IsLargerThanMinTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.StartTime > host.MinTime() ||
                   e.EqualsMultiMinTime(host);
        }

        public static bool EqualsMultiMaxTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.EqualsMaxTime(host) && host.GetMaxTimeCount() > 1;
        }

        public static bool EqualsMultiMinTime(this IKeyEvent e, IDetailedEventHost host)
        {
            return e.EqualsMinTime(host) && host.GetMinTimeCount() > 1;
        }

        public static bool InInvisibleTimingRange(this IKeyEvent e, TimeRange obsoleteList, out RangeValue<double> range)
        {
            return obsoleteList.ContainsTimingPoint(out range, e.StartTime, e.EndTime);
        }
        public static bool SuccessiveTo(this IKeyEvent previous, IKeyEvent next)
        {
#if NET5_0_OR_GREATER
            return previous.GetEndsSpan().SequenceEqual(next.GetStartsSpan());
#else
            return previous.GetEnds().SequenceEqual(next.GetStarts());
#endif
        }

        public static bool EndsWithIneffective(this IKeyEvent e)
        {
            return EventExtensions.IneffectiveDictionary.ContainsKey(e.EventType.Flag) &&
                   EventExtensions.IneffectiveDictionary[e.EventType.Flag].SequenceEqual(e.GetEnds());
        }

        public static bool IsStaticAndDefault(this IKeyEvent e)
        {
            if (!e.IsStartsEqualsEnds()) return false;
            return EventExtensions.DefaultDictionary.TryGetValue(e.EventType.Flag, out var defValue) &&
#if NET5_0_OR_GREATER
                   e.GetStartsSpan().SequenceEqual(defValue.AsSpan())
#else
                   e.GetStarts().SequenceEqual(defValue)
#endif
                ;
        }

        public static bool OnInvisibleTimingRangeBound(this IKeyEvent e, TimeRange obsoleteList)
        {
            return obsoleteList.OnTimingRangeBound(out _, e.StartTime) ||
                   obsoleteList.OnTimingRangeBound(out _, e.EndTime);
        }

        /// <summary>
        /// 0ms based fixed frame determined by interval
        /// </summary>
        /// <param name="e"></param>
        /// <param name="absolute"></param>
        /// <param name="discretizingInterval"></param>
        /// <param name="discretizingAccuracy"></param>
        /// <returns></returns>
        public static List<IKeyEvent> ComputeDiscretizedEvents(this IKeyEvent e,
            bool absolute,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            if (e.EventType.Index < 100)
            {
                return BasicEventExtensions.ComputeDiscretizedEvents((BasicEvent)e,
                    discretizingInterval, discretizingAccuracy);
            }

            // relative events
            // absolute: compute each frame with actual relative value
            // !absolute: compute each frame and preserve the changed relative value compared to the first frame 
            var eventList = new List<IKeyEvent>();
            var targetEventType = e.EventType;

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var thisTime = startTime - (startTime % discretizingInterval);
            var nextTime = startTime - (startTime % discretizingInterval) + discretizingInterval;
            if (nextTime > endTime) nextTime = endTime;
            List<float> reusableValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);

            eventList.Add(new RelativeEvent(targetEventType, LinearEase.Instance,
                startTime, nextTime, reusableValue.ToList()));

            while (nextTime < endTime)
            {
                thisTime += discretizingInterval;
                nextTime += discretizingInterval;
                if (nextTime > endTime) nextTime = endTime;
                List<float> newValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);
                var newValueCopy = newValue.ToList();

                if (absolute)
                {
                    for (int i = 0; i < e.EventType.Size; i++)
                    {
                        newValue[i] = discretizingAccuracy == null
                            ? newValue[i] - reusableValue[i]
                            : (float)Math.Round(newValue[i] - reusableValue[i], discretizingAccuracy.Value);
                        reusableValue[i] = newValueCopy[i];
                    }
                }

                var relativeEvent = new RelativeEvent(targetEventType, LinearEase.Instance,
                    thisTime, nextTime, newValue);
                if (!absolute)
                {
                    relativeEvent.SetStarts(reusableValue);
                    reusableValue = newValueCopy;
                }

                eventList.Add(relativeEvent);
            }

            return eventList;
        }

        public static List<float> ComputeFrame(this IKeyEvent e, float currentTime, int? accuracy)
        {
            if (e.EventType.Index < 100)
            {
                return BasicEventExtensions.ComputeFrame((BasicEvent)e, currentTime, accuracy);
            }

            var easing = e.Easing;
            var size = e.EventType.Size;

            var start = new float[size];
            var end = e.GetEnds().ToArray();

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var normalizedTime = (currentTime - startTime) / (endTime - startTime);
            var easedTime = (float)easing.Ease(normalizedTime);

            var list = new List<float>(size);
            for (int i = 0; i < size; i++)
            {
                var val = (end[i] - start[i]) * easedTime + start[i];
                if (accuracy == null) list.Add(val);
                else list.Add((float)Math.Round(val, accuracy.Value));
            }

            return list;
        }
    }
}