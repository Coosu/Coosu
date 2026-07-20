using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Utils;

public static class ScriptHelper
{
    private sealed class ElementEventsWriterHolder
    {
        public ElementEventsWriterHolder(Func<TextWriter, ISceneObject, bool, Task> writer)
        {
            Writer = writer;
        }

        public Func<TextWriter, ISceneObject, bool, Task> Writer { get; set; }
    }

    private static readonly ConditionalWeakTable<ISceneObject, ElementEventsWriterHolder> ElementEventsWriters = new();

    /// <summary>
    /// Registers a serialization override without adding formatter state to the scene object itself.
    /// </summary>
    public static void SetElementEventsWriter(ISceneObject sceneObject,
        Func<TextWriter, ISceneObject, bool, Task> writer)
    {
        if (sceneObject == null) throw new ArgumentNullException(nameof(sceneObject));
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        var holder = ElementEventsWriters.GetValue(sceneObject, _ => new ElementEventsWriterHolder(writer));
        holder.Writer = writer;
    }

    public static bool RemoveElementEventsWriter(ISceneObject sceneObject) =>
        ElementEventsWriters.Remove(sceneObject);

    public static async Task WriteGroupedEventAsync(TextWriter writer, IEnumerable<IKeyEvent> events, int index)
    {
        var indent = new string(' ', index);
        var groupedEvents = events
            .OrderBy(k => k.EventType)
            .ThenBy(k => k.EndTime)
            .ThenBy(k => k.EventType.Index)
            .GroupBy(k => k.EventType);
        foreach (var grouping in groupedEvents)
        {
            foreach (IKeyEvent e in grouping)
            {
                await writer.WriteAsync(indent);
                await e.WriteScriptAsync(writer);
                await writer.WriteLineAsync();
            }
        }
    }

    public static async Task WriteSequentialEventAsync(TextWriter writer, IEnumerable<IKeyEvent> events, int index)
    {
        var indent = new string(' ', index);
        if (events is SortedSet<IKeyEvent> sortedSet)
        {
            foreach (var e in sortedSet)
            {
                await writer.WriteAsync(indent);
                await e.WriteScriptAsync(writer);
                await writer.WriteLineAsync();
            }
        }
        else
        {
            foreach (IKeyEvent e in events.OrderBy(k => k, EventSequenceComparer.Instance))
            {
                await writer.WriteAsync(indent);
                await e.WriteScriptAsync(writer);
                await writer.WriteLineAsync();
            }
        }
    }

    public static async Task WriteHostEventsAsync(TextWriter writer, IDetailedEventHost host, bool group)
    {
        if (group)
            await WriteGroupedEventAsync(writer, host.Events, 1);
        else
            await WriteSequentialEventAsync(writer, host.Events, 1);
    }

    public static async Task WriteElementEventsAsync(TextWriter writer, ISceneObject sceneObject, bool group)
    {
        if (ElementEventsWriters.TryGetValue(sceneObject, out var customWriter))
        {
            await customWriter.Writer(writer, sceneObject, group);
            return;
        }

        if (sceneObject is ILoopHost loopHost)
        {
            foreach (var loop in loopHost.LoopList)
                await WriteSubEventHostAsync(writer, loop, @group);
        }

        if (group)
            await WriteGroupedEventAsync(writer, sceneObject.Events, 1);
        else
            await WriteSequentialEventAsync(writer, sceneObject.Events, 1);

        if (sceneObject is ITriggerHost triggerHost)
        {
            foreach (var trigger in triggerHost.TriggerList)
                await WriteSubEventHostAsync(writer, trigger, @group);
        }
    }

    public static async Task WriteSubEventHostAsync(TextWriter writer, ISubEventHost subEventHost, bool group)
    {
        await writer.WriteAsync(' ');
        await subEventHost.WriteHeaderAsync(writer);
        await writer.WriteLineAsync();

        if (group)
            await WriteGroupedEventAsync(writer, subEventHost.Events, 2);
        else
            await WriteSequentialEventAsync(writer, subEventHost.Events, 2);
    }
}
