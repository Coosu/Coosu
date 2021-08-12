using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX
{
    public static class EventHostExtension
    {
        public static void ApplyAction(this IEventHost host, CommonEvent commonEvent)
        {
            host.Events.Add(commonEvent);
        }

        public static void ApplyAction<T>(this IEventHost host, T commonEvent) where T : CommonEvent
        {
            host.Events.Add(commonEvent);
        }
    }
}