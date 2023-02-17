using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.OsbX;

[DebuggerDisplay("Header = {DebuggerDisplay}")]
public class Camera25Object : ISceneObject, IDefinedObject
{
    private string DebuggerDisplay => this.GetHeaderString();
    private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);
    public ObjectType ObjectType { get; } = 99;
    public int? RowInSource { get; set; }
    public object? Tag { get; set; }
    public double DefaultY { get; set; } = 0;
    public double DefaultX { get; set; } = 0;
    public double DefaultZ { get; set; } = 1;
    public string CameraIdentifier { get; set; }
    public List<Loop> LoopList { get; } = new();
    public List<Trigger> TriggerList { get; } = new();
    public double MaxTime() => Events.Count > 0 ? Events.Max(k => k.EndTime) : 0;
    public double MinTime() => Events.Count > 0 ? Events.Min(k => k.StartTime) : 0;
    public double MaxStartTime() => Events.Count > 0 ? Events.Max(k => k.StartTime) : 0;
    public double MinEndTime() => Events.Count > 0 ? Events.Min(k => k.EndTime) : 0;
    public bool EnableGroupedSerialization { get; set; }

    // EventHosts

    public IReadOnlyCollection<IKeyEvent> Events
    {
        get => (IReadOnlyCollection<IKeyEvent>)_events;
        internal set => _events = value as ICollection<IKeyEvent> ??
                                  throw new Exception($"The collection should be {nameof(ICollection<IKeyEvent>)}");
    }

    IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;

    public void AddEvent(IKeyEvent @event)
    {
        _events.Add(@event);
    }

    public bool RemoveEvent(IKeyEvent @event)
    {
        return _events.Remove(@event);
    }

    public void ResetEventCollection(IComparer<IKeyEvent>? comparer)
    {
        _events.Clear();
        if (comparer == null)
            _events = new HashSet<IKeyEvent>();
        else
            _events = new SortedSet<IKeyEvent>(comparer);
    }

    public async Task WriteHeaderAsync(TextWriter writer)
    {
        await writer.WriteAsync($"{ObjectType.GetString(ObjectType)},{CameraIdentifier}");
    }

    public async Task WriteScriptAsync(TextWriter writer)
    {
        await WriteHeaderAsync(writer);
        await writer.WriteLineAsync();
        await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    static Camera25Object()
    {
        ObjectType.SignType(99, "Camera25");
    }
}