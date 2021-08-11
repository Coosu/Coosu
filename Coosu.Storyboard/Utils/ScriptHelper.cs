using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Events.EventHosts;

namespace Coosu.Storyboard.Utils
{
    public static class ScriptHelper
    {
        public static async Task WriteGroupedEventAsync(this TextWriter sw, IEnumerable<CommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            var groupedEvents = events.OrderBy(k => k.EventType).GroupBy(k => k.EventType);
            foreach (var grouping in groupedEvents)
                foreach (CommonEvent e in grouping)
                    await sw.WriteLineAsync(indent + e);
        }

        public static async Task WriteSequentialEventAsync(this TextWriter sw, IEnumerable<CommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            foreach (CommonEvent e in events)
                await sw.WriteLineAsync(indent + e);
        }

        public static async Task WriteHostEventsAsync(TextWriter sw, EventHost host, bool group)
        {
            if (group)
                await sw.WriteGroupedEventAsync(host.Events, 1);
            else
                await sw.WriteSequentialEventAsync(host.Events, 1);
        }

        public static async Task WriteElementEventsAsync(this TextWriter sw, Sprite sprite, bool group)
        {
            if (group)
                await sw.WriteGroupedEventAsync(sprite.Events, 1);
            else
                await sw.WriteSequentialEventAsync(sprite.Events, 1);

            foreach (var loop in sprite.LoopList)
                await sw.WriteLoopAsync(loop, group);
            foreach (var trigger in sprite.TriggerList)
                await sw.WriteTriggerAsync(trigger, group);
        }

        public static async Task WriteTriggerAsync(this TextWriter sw, Trigger trigger, bool group)
        {
            var head = " " + trigger;
            await sw.WriteLineAsync(head);

            if (group)
                await sw.WriteGroupedEventAsync(trigger.Events, 2);
            else
                await sw.WriteSequentialEventAsync(trigger.Events, 2);
        }

        public static async Task WriteLoopAsync(this TextWriter sw, Loop loop, bool group)
        {
            var head = " " + loop;
            await sw.WriteLineAsync(head);

            if (group)
                await sw.WriteGroupedEventAsync(loop.Events, 2);
            else
                await sw.WriteSequentialEventAsync(loop.Events, 2);
        }
    }
}
