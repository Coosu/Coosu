using Coosu.Storyboard.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Coosu.Storyboard.Internal
{
    public static class EventExtension
    {
        public static readonly ReadOnlyDictionary<EventType, float[]> DefaultDictionary =
            new ReadOnlyDictionary<EventType, float[]>(
                new Dictionary<EventType, float[]>
                {
                    [EventType.Fade] = new[] { 1f },
                    [EventType.Scale] = new[] { 1f },
                    [EventType.Vector] = new[] { 1f, 1f },
                    [EventType.Rotate] = new[] { 0f },
                    [EventType.Color] = new[] { 255f, 255f, 255f },
                }
            );

        public static readonly ReadOnlyDictionary<EventType, float[]> UnworthyDictionary =
            new ReadOnlyDictionary<EventType, float[]>(
                new Dictionary<EventType, float[]>
                {
                    [EventType.Fade] = new[] { 0f },
                    [EventType.Scale] = new[] { 0f },
                    [EventType.Vector] = new[] { 0f, 0f },
                    [EventType.Color] = new[] { 0f, 0f, 0f },
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
    }
}
