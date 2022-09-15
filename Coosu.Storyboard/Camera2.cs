using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard;

public class Camera2 : IEventHost, ICameraUsable, ICloneable
{
    private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);

    public IReadOnlyCollection<IKeyEvent> Events
    {
        get => (IReadOnlyCollection<IKeyEvent>)_events;
        internal set => _events = value as ICollection<IKeyEvent> ?? throw new Exception(
            $"The collection should be {nameof(ICollection<IKeyEvent>)}");
    }

    public OriginType OriginType { get; set; } = OriginType.Centre;

    /// <inheritdoc />
    public string CameraIdentifier { get; set; } = Guid.NewGuid().ToString();

    /// <inheritdoc />
    public double DefaultX { get; set; } = 320;

    /// <inheritdoc />
    public double DefaultY { get; set; } = 240;

    /// <inheritdoc />
    public double DefaultZ { get; set; } = 1;

    public object Clone()
    {
        return new Camera2
        {
            CameraIdentifier = CameraIdentifier,
            DefaultX = DefaultX,
            DefaultY = DefaultY,
            DefaultZ = DefaultZ,
            OriginType = OriginType,
            Events = Events.Select(k => k.Clone()).Cast<IKeyEvent>().ToList()
        };
    }

    IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;

    public void AddEvent(IKeyEvent @event)
    {
        throw new NotImplementedException("The camera transform is currently not implemented.");
        _events.Add(@event);
    }

    public bool RemoveEvent(IKeyEvent @event)
    {
        throw new NotImplementedException("The camera transform is currently not implemented.");
        return _events.Remove(@event);
    }

    public void ResetEventCollection(IComparer<IKeyEvent>? comparer)
    {
        throw new NotImplementedException("The camera transform is currently not implemented.");
        _events.Clear();
        if (comparer == null)
            _events = new HashSet<IKeyEvent>();
        else
            _events = new SortedSet<IKeyEvent>(comparer);
    }

    public async Task WriteHeaderAsync(TextWriter writer)
    {
    }

    public async Task WriteScriptAsync(TextWriter writer)
    {
    }

    public State GetCameraState(double time)
    {
        return new State();
    }

    public State GetObjectState(double time, State oldState)
    {
        return oldState;
    }
}