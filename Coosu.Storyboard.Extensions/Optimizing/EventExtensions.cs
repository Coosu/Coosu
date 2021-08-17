using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

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
                : Array.Empty<double>();
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
    }
}
