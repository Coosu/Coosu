using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Utils
{
    public static class EventExtension
    {
        public static readonly ReadOnlyDictionary<EventType, float[]> DefaultDictionary =
            new ReadOnlyDictionary<EventType, float[]>(
                new Dictionary<EventType, float[]>
                {
                    [EventTypes.Fade] = new[] { 1f },
                    [EventTypes.Scale] = new[] { 1f },
                    [EventTypes.Vector] = new[] { 1f, 1f },
                    [EventTypes.Rotate] = new[] { 0f },
                    [EventTypes.Color] = new[] { 255f, 255f, 255f },
                }
            );

        public static readonly ReadOnlyDictionary<EventType, float[]> UnworthyDictionary =
            new ReadOnlyDictionary<EventType, float[]>(
                new Dictionary<EventType, float[]>
                {
                    [EventTypes.Fade] = new[] { 0f },
                    [EventTypes.Scale] = new[] { 0f },
                    [EventTypes.Vector] = new[] { 0f, 0f },
                    [EventTypes.Color] = new[] { 0f, 0f, 0f },
                }
            );

        public static float[] GetDefaultValue(this CommonEvent e)
        {
            return DefaultDictionary.ContainsKey(e.EventType)
                ? DefaultDictionary[e.EventType]
                : null;
        }

        public static float[] GetUnworthyValue(this CommonEvent e)
        {
            return UnworthyDictionary.ContainsKey(e.EventType)
                ? UnworthyDictionary[e.EventType]
                : null;
        }

        public static IEnumerable<Fade> GetFadeList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Fade).Select(k => (Fade)k);
        public static IEnumerable<Color> GetColorList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Color).Select(k => (Color)k);
        public static IEnumerable<Move> GetMoveList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Move).Select(k => (Move)k);
        public static IEnumerable<MoveX> GetMoveXList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.MoveX).Select(k => (MoveX)k);
        public static IEnumerable<MoveY> GetMoveYList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.MoveY).Select(k => (MoveY)k);
        public static IEnumerable<Parameter> GetParameterList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Parameter).Select(k => (Parameter)k);
        public static IEnumerable<Rotate> GetRotateList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Rotate).Select(k => (Rotate)k);
        public static IEnumerable<Scale> GetScaleList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Scale).Select(k => (Scale)k);
        public static IEnumerable<Vector> GetVectorList(this EventHost ec) =>
            ec.Events.Where(k => k.EventType == EventTypes.Vector).Select(k => (Vector)k);
    }
}
