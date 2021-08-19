using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public static class EventExtensions
    {
        public static readonly ReadOnlyDictionary<EventType, double[]> UnworthyDictionary = new(
            new Dictionary<EventType, double[]>
            {
                [EventTypes.Fade] = new[] { 0d },
                [EventTypes.Scale] = new[] { 0d },
                [EventTypes.Vector] = new[] { 0d, 0d },
                [EventTypes.Color] = new[] { 0d, 0d, 0d },
            });

        public static double[] GetUnworthyValue(this ICommonEvent e)
        {
            return UnworthyDictionary.ContainsKey(e.EventType)
                ? UnworthyDictionary[e.EventType]
                : EmptyArray<double>.Value;
        }

        public static IEnumerable<Fade> GetFadeList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Fade).Select(k => (Fade)k);
        public static IEnumerable<Color> GetColorList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Color).Select(k => (Color)k);
        public static IEnumerable<Move> GetMoveList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Move).Select(k => (Move)k);
        public static IEnumerable<MoveX> GetMoveXList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.MoveX).Select(k => (MoveX)k);
        public static IEnumerable<MoveY> GetMoveYList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.MoveY).Select(k => (MoveY)k);
        public static IEnumerable<Parameter> GetParameterList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Parameter).Select(k => (Parameter)k);
        public static IEnumerable<Rotate> GetRotateList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Rotate).Select(k => (Rotate)k);
        public static IEnumerable<Scale> GetScaleList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Scale).Select(k => (Scale)k);
        public static IEnumerable<Vector> GetVectorList(this IEventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Vector).Select(k => (Vector)k);

        /// <summary>
        /// 0ms based fixed frame determined by interval
        /// </summary>
        /// <param name="e"></param>
        /// <param name="absolute"></param>
        /// <param name="discretizingInterval"></param>
        /// <param name="discretizingAccuracy"></param>
        /// <returns></returns>
        public static List<ICommonEvent> ComputeDiscretizedEvents(this ICommonEvent e,
            bool absolute,
            int discretizingInterval = TempGlobalConstant.DiscretizingInterval,
            int? discretizingAccuracy = 3)
        {
            if (e.EventType.Index < 100)
            {
                return CommonEventExtensions.ComputeDiscretizedEvents((CommonEvent)e,
                    discretizingInterval, discretizingAccuracy);
            }

            // relative events
            // absolute: compute each frame with actual relative value
            // !absolute: compute each frame and preserve the changed relative value compared to the first frame 
            var eventList = new List<ICommonEvent>();
            var targetEventType = e.EventType;

            var startTime = (int)e.StartTime;
            var endTime = (int)e.EndTime;

            var thisTime = startTime - (startTime % discretizingInterval);
            var nextTime = startTime - (startTime % discretizingInterval) + discretizingInterval;
            if (nextTime > endTime) nextTime = endTime;
            double[] reusableValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);

            eventList.Add(new RelativeEvent(targetEventType, LinearEase.Instance,
                startTime, nextTime, reusableValue.ToArray()));

            while (nextTime < endTime)
            {
                thisTime += discretizingInterval;
                nextTime += discretizingInterval;
                if (nextTime > endTime) nextTime = endTime;
                double[] newValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);
                var copy = newValue.ToArray();

                if (absolute)
                {
                    for (int i = 0; i < e.EventType.Size; i++)
                    {
                        newValue[i] = discretizingAccuracy == null
                             ? newValue[i] - reusableValue[i]
                             : Math.Round(newValue[i] - reusableValue[i], discretizingAccuracy.Value);
                        reusableValue[i] = copy[i];
                    }
                }

                var relativeEvent = new RelativeEvent(targetEventType, LinearEase.Instance,
                    thisTime, nextTime, newValue);
                if (!absolute)
                {
                    relativeEvent.Start = reusableValue;
                    reusableValue = copy;
                }

                eventList.Add(relativeEvent);
            }

            return eventList;
        }

        public static double[] ComputeFrame(this ICommonEvent e, double currentTime, int? accuracy)
        {
            if (e.EventType.Index < 100)
            {
                return CommonEventExtensions.ComputeFrame((CommonEvent)e, currentTime, accuracy);
            }

            var easing = e.Easing;
            var size = e.EventType.Size;

            var start = new double[size];
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
    }
}
