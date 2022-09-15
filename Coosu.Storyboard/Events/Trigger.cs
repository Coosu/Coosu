using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events;

public sealed class Trigger : ISubEventHost, IEvent
{
    public event Action? TimingChanged;

    internal ISceneObject? _baseObject;
    private const string HitSound = "HitSound";
    private double _startTime;
    private double _endTime;
    private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);

    private double? _cachedMaxTime;
    private double? _cachedMaxStartTime;

    public IReadOnlyCollection<IKeyEvent> Events
    {
        get => (IReadOnlyCollection<IKeyEvent>)_events;
        internal set => _events = value as ICollection<IKeyEvent> ?? throw new Exception(
            $"The collection should be {nameof(ICollection<IKeyEvent>)}");
    }

    public EventType EventType { get; } = EventTypes.Trigger;

    public bool EnableGroupedSerialization { get; set; } /*= true;*/

    public double StartTime
    {
        get => _startTime;
        set
        {
            if (Precision.AlmostEquals(_startTime, value)) return;
            _startTime = value;
            TimingChanged?.Invoke();
        }
    }

    public double EndTime
    {
        get => _endTime;
        set
        {
            if (Precision.AlmostEquals(_endTime, value)) return;
            _endTime = value;
            TimingChanged?.Invoke();
        }
    }

    public string TriggerName { get; set; }

    public double MaxTime()
    {
        if (_cachedMaxTime != null) return _cachedMaxTime.Value;
        return (double)(_cachedMaxTime =
            EndTime + (Events.Count > 0 ? Events.Max(k => k.EndTime) : 0));
    }

    public double MinTime() => StartTime;

    public double MaxStartTime()
    {
        if (_cachedMaxStartTime != null) return _cachedMaxStartTime.Value;
        return (double)(_cachedMaxStartTime =
            EndTime + (Events.Count > 0 ? Events.Max(k => k.StartTime) : 0));   //if hitsound played at end time

    }

    public double MinEndTime() => StartTime; // if no hitsound here

    public Trigger(double startTime, double endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
    {
        StartTime = startTime;
        EndTime = endTime;

        TriggerName = GetTriggerString(triggerType, listenSample, customSampleSet);
    }

    public Trigger(double startTime, double endTime, string triggerName)
    {
        StartTime = startTime;
        EndTime = endTime;
        TriggerName = triggerName;
    }

    public async Task WriteHeaderAsync(TextWriter writer)
    {
        await writer.WriteAsync(EventType.Flag);
        await writer.WriteAsync(',');
        await writer.WriteAsync(TriggerName);
        await writer.WriteAsync(',');
        await writer.WriteStandardizedNumberAsync(Math.Round(StartTime));
        await writer.WriteAsync(',');
        await writer.WriteStandardizedNumberAsync(Math.Round(EndTime));
    }

    public async Task WriteScriptAsync(TextWriter writer)
    {
        await ScriptHelper.WriteSubEventHostAsync(writer, this, EnableGroupedSerialization);
    }

    private static string GetTriggerString(TriggerType triggerType, bool listenSample, uint? customSampleSet)
    {
        var sb = new StringBuilder(HitSound, 23);
        if (triggerType.HasFlag(TriggerType.HitSoundNormal) ||
            triggerType.HasFlag(TriggerType.HitSoundSoft) ||
            triggerType.HasFlag(TriggerType.HitSoundDrum))
        {
            if (listenSample)
                sb.Append("All");

            if (triggerType.HasFlag(TriggerType.HitSoundNormal))
                sb.Append("Normal");
            else if (triggerType.HasFlag(TriggerType.HitSoundSoft))
                sb.Append("Soft");
            else if (triggerType.HasFlag(TriggerType.HitSoundDrum))
                sb.Append("Drum");

            if (listenSample)
            {
                var str = sb.ToString();
                return str.EndsWith("All") ? HitSound : str;
            }
        }

        if (triggerType.HasFlag(TriggerType.HitSoundWhistle) ||
            triggerType.HasFlag(TriggerType.HitSoundFinish) ||
            triggerType.HasFlag(TriggerType.HitSoundClap))
        {
            if (triggerType.HasFlag(TriggerType.HitSoundWhistle))
                sb.Append("Whistle");
            else if (triggerType.HasFlag(TriggerType.HitSoundFinish))
                sb.Append("Finish");
            else if (triggerType.HasFlag(TriggerType.HitSoundClap))
                sb.Append("Clap");
        }

        if (customSampleSet != null) sb.Append(customSampleSet.ToString());
        return sb.ToString();
    }

    public void AdjustTiming(double offset)
    {
        StartTime += offset;
    }

    IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;

    public void AddEvent(IKeyEvent @event)
    {
        _events.Add(@event);
        @event.TimingChanged += ResetCacheAndRaiseTimingChanged;
        ResetCacheAndRaiseTimingChanged();
    }

    public bool RemoveEvent(IKeyEvent @event)
    {
        @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        var removeEvent = _events.Remove(@event);
        if (removeEvent)
            ResetCacheAndRaiseTimingChanged();
        return removeEvent;
    }

    public void ResetEventCollection(IComparer<IKeyEvent>? comparer = null)
    {
        var valid = _events.Count > 0;
        foreach (var @event in _events)
            @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        _events.Clear();
        if (comparer == null)
            _events = new HashSet<IKeyEvent>();
        else
            _events = new SortedSet<IKeyEvent>(comparer);
        if (valid) ResetCacheAndRaiseTimingChanged();
    }

    ISceneObject? ISubEventHost.BaseObject
    {
        get => _baseObject;
        set => _baseObject = value;
    }

    public object Clone()
    {
        var trigger = new Trigger(StartTime, EndTime, TriggerName)
        {
            EnableGroupedSerialization = EnableGroupedSerialization
        };

        foreach (var keyEvent in _events)
            trigger.AddEvent((IKeyEvent)keyEvent.Clone());

        return trigger;
    }

    private void ResetCacheAndRaiseTimingChanged()
    {
        _cachedMaxTime = null;
        _cachedMaxStartTime = null;
        TimingChanged?.Invoke();
    }
}