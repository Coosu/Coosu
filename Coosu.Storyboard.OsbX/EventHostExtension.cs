using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX
{
    public static class EventHostExtension
    {
        public static void ApplyAction(this IDetailedEventHost host, BasicEvent basicEvent)
        {
            host.Events.Add(basicEvent);
        }

        public static void ApplyAction<T>(this IDetailedEventHost host, T basicEvent) where T : BasicEvent
        {
            host.Events.Add(basicEvent);
        }
    }
}