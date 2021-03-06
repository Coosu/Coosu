﻿using Coosu.Storyboard.Events;
using Coosu.Storyboard.Events.Containers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coosu.Storyboard.Utils
{
    public static class StringUtility
    {
        internal static void WriteGroupedEvent(this TextWriter sw, IEnumerable<CommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            var groupedEvents = events.OrderBy(k => k.EventType).GroupBy(k => k.EventType);
            foreach (var grouping in groupedEvents)
                foreach (CommonEvent e in grouping)
                    sw.WriteLine(indent + e);
        }

        internal static void WriteSequentialEvent(this TextWriter sw, IEnumerable<CommonEvent> events, int index)
        {
            var indent = new string(' ', index);
            foreach (CommonEvent e in events)
                sw.WriteLine(indent + e);
        }

        public static void WriteContainerEvents(TextWriter sw, EventContainer container, bool group)
        {
            if (group)
                sw.WriteGroupedEvent(container.EventList, 1);
            else
                sw.WriteSequentialEvent(container.EventList, 1);
        }

        internal static void WriteElementEvents(this TextWriter sw, Element element, bool group)
        {
            if (group)
                sw.WriteGroupedEvent(element.EventList, 1);
            else
                sw.WriteSequentialEvent(element.EventList, 1);

            foreach (var loop in element.LoopList)
                sw.WriteLoop(loop, group);
            foreach (var trigger in element.TriggerList)
                sw.WriteTrigger(trigger, group);
        }

        internal static void WriteTrigger(this TextWriter sw, Trigger trigger, bool group)
        {
            var head = " " + trigger;
            sw.WriteLine(head);

            if (group)
                sw.WriteGroupedEvent(trigger.EventList, 2);
            else
                sw.WriteSequentialEvent(trigger.EventList, 2);
        }

        internal static void WriteLoop(this TextWriter sw, Loop loop, bool group)
        {
            var head = " " + loop;
            sw.WriteLine(head);

            if (group)
                sw.WriteGroupedEvent(loop.EventList, 2);
            else
                sw.WriteSequentialEvent(loop.EventList, 2);
        }
    }
}
