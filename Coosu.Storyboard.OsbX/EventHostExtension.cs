using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX
{
    public static class EventHostExtension
    {
        public static void ApplyAction(this EventHost host, CommonEvent commonEvent)
        {
            host.Events.Add(commonEvent);
        }

        public static void ApplyAction<T>(this EventHost host, T commonEvent) where T : CommonEvent
        {
            host.Events.Add(commonEvent);
        }
    }
}