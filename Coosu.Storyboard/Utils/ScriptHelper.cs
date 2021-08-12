using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Utils
{
    public static class ScriptHelper
    {
        public static async Task WriteGroupedEventAsync(TextWriter sw, IEnumerable<ICommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            var groupedEvents = events.OrderBy(k => k.EventType).GroupBy(k => k.EventType);
            foreach (var grouping in groupedEvents)
                foreach (CommonEvent e in grouping)
                    await sw.WriteLineAsync(indent + e);
        }

        public static async Task WriteSequentialEventAsync(TextWriter sw, IEnumerable<ICommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            foreach (CommonEvent e in events)
                await sw.WriteLineAsync(indent + e);
        }

        public static async Task WriteHostEventsAsync(TextWriter sw, IEventHost host, bool group)
        {
            if (group)
                await WriteGroupedEventAsync(sw, host.Events, 1);
            else
                await WriteSequentialEventAsync(sw, host.Events, 1);
        }

        public static async Task WriteElementEventsAsync(TextWriter sw, ISceneObject sprite, bool group)
        {
            if (group)
                await WriteGroupedEventAsync(sw, sprite.Events, 1);
            else
                await WriteSequentialEventAsync(sw, sprite.Events, 1);

            foreach (var loop in sprite.LoopList)
                await WriteLoopAsync(sw, loop, @group);
            foreach (var trigger in sprite.TriggerList)
                await WriteTriggerAsync(sw, trigger, @group);
        }

        public static async Task WriteTriggerAsync(TextWriter sw, Trigger trigger, bool group)
        {
            var head = " " + trigger;
            await sw.WriteLineAsync(head);

            if (group)
                await WriteGroupedEventAsync(sw, trigger.Events, 2);
            else
                await WriteSequentialEventAsync(sw, trigger.Events, 2);
        }

        public static async Task WriteLoopAsync(TextWriter sw, Loop loop, bool group)
        {
            var head = " " + loop;
            await sw.WriteLineAsync(head);

            if (group)
                await WriteGroupedEventAsync(sw, loop.Events, 2);
            else
                await WriteSequentialEventAsync(sw, loop.Events, 2);
        }
    }
}
