using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Internal;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard;

/// <summary>
/// Represents a storyboard sprite. This class cannot be inherited.
/// </summary>
[DebuggerDisplay("Header = {DebuggerDisplay}")]
public class Sprite : ISceneObject
{
    private string DebuggerDisplay => this.GetHeaderString();
    private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);

    private double? _cachedMaxTime;
    private double? _cachedMinTime;
    private double? _cachedMaxStartTime;
    private double? _cachedMinEndTime;

    public virtual ObjectType ObjectType { get; } = ObjectTypes.Sprite;

    public int? RowInSource { get; set; }
    public object? Tag { get; set; }

    public LayerType LayerType { get; set; }
    public OriginType OriginType { get; set; }
    public string ImagePath { get; set; }
    /// <inheritdoc />
    public double DefaultX { get; set; }
    /// <inheritdoc />
    public double DefaultY { get; set; }

    /// <inheritdoc />
    public double DefaultZ { get; set; }
    /// <inheritdoc />
    public string CameraIdentifier { get; set; } = "00000000-0000-0000-0000-000000000000";

    // EventHosts

    public IReadOnlyCollection<IKeyEvent> Events
    {
        get => (IReadOnlyCollection<IKeyEvent>)_events;
        set => _events = value as ICollection<IKeyEvent> ?? throw new Exception(
            $"The collection should be {nameof(ICollection<IKeyEvent>)}");
    }

    // ISceneObject

    public IReadOnlyList<Loop> LoopList
    {
        get => _loopList;
        private set => _loopList = (List<Loop>)value;
    }

    public IReadOnlyList<Trigger> TriggerList
    {
        get => _triggerList;
        private set => _triggerList = (List<Trigger>)value;
    }

    public double MaxTime()
    {
        if (_cachedMaxTime != null) return _cachedMaxTime.Value;
        if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
            return double.NaN;

        var max = Events.Count == 0 ? double.MinValue : Events.Max(k => k.EndTime);
        var loopMax = LoopList.Count == 0 ? double.MinValue : LoopList.Max(k => k.OuterMaxTime());
        max = max >= loopMax ? max : loopMax;

        var triggerMax = TriggerList.Count == 0 ? double.MinValue : TriggerList.Max(k => k.MaxTime());
        max = max >= triggerMax ? max : triggerMax;
        return (double)(_cachedMaxTime = max);
    }

    public double MinTime()
    {
        if (_cachedMinTime != null) return _cachedMinTime.Value;
        if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
            return double.NaN;

        var min = Events.Count == 0 ? double.MaxValue : Events.Min(k => k.StartTime);
        var loopMin = LoopList.Count == 0 ? double.MaxValue : LoopList.Min(k => k.OuterMinTime());
        min = min <= loopMin ? min : loopMin;

        var triggerMin = TriggerList.Count == 0 ? double.MaxValue : TriggerList.Min(k => k.MinTime());
        min = min <= triggerMin ? min : triggerMin;
        return (double)(_cachedMinTime = min);
    }

    public double MaxStartTime()
    {
        if (_cachedMaxStartTime != null) return _cachedMaxStartTime.Value;
        if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
            return double.NaN;

        var max = Events.Count == 0 ? double.MinValue : Events.Max(k => k.StartTime);
        var loopMax = LoopList.Count == 0 ? double.MinValue : LoopList.Max(k => k.OuterMinTime());
        max = max >= loopMax ? max : loopMax;

        var triggerMax = TriggerList.Count == 0 ? double.MinValue : TriggerList.Max(k => k.MinTime());
        max = max >= triggerMax ? max : triggerMax;
        return (double)(_cachedMaxStartTime = max);
    }

    public double MinEndTime()
    {
        if (_cachedMinEndTime != null) return _cachedMinEndTime.Value;
        if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
            return double.NaN;

        var min = Events.Count == 0 ? double.MaxValue : Events.Min(k => k.EndTime);
        var loopMin = LoopList.Count == 0 ? double.MaxValue : LoopList.Min(k => k.OuterMaxTime());
        min = min <= loopMin ? min : loopMin;

        var triggerMin = TriggerList.Count == 0 ? double.MaxValue : TriggerList.Min(k => k.MaxTime());
        min = min <= triggerMin ? min : triggerMin;
        return (double)(_cachedMinEndTime = min);
    }

    public bool EnableGroupedSerialization { get; set; }/* = true;*/

    // Loop control
    private bool _isTriggering;
    private bool _isLooping;
    private List<Loop> _loopList = new();
    private List<Trigger> _triggerList = new();

    /// <summary>
    /// Create a storyboard sprite by a static image.
    /// </summary>
    /// <param name="layerType">Set sprite layer.</param>
    /// <param name="originType">Set sprite origin.</param>
    /// <param name="imagePath">Set image path.</param>
    /// <param name="defaultX">Set default x-coordinate of location.</param>
    /// <param name="defaultY">Set default x-coordinate of location.</param>
    public Sprite(LayerType layerType, OriginType originType, string imagePath, double defaultX, double defaultY)
    {
        LayerType = layerType;
        OriginType = originType;
        ImagePath = imagePath;
        DefaultX = defaultX;
        DefaultY = defaultY;
    }

    public Sprite(ReadOnlySpan<char> layer, ReadOnlySpan<char> origin, ReadOnlySpan<char> imagePath,
        double defaultX, double defaultY)
    {
        //ObjectType = OsbObjectType.Parse(type);
        LayerType = layer.ToLayerType();
        OriginType = origin.ToOriginType();
        ImagePath = imagePath.ToString();
        DefaultX = defaultX;
        DefaultY = defaultY;
    }

    /// <summary>
    /// Create a loop object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="loopCount"></param>
    /// <remarks>This method returns a standalone disposable loop object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public IEventHostDisposable CreateLoop(int startTime, int loopCount)
    {
        var loop = new Loop(startTime, loopCount);
        return new LoopTriggerCreationWrapper(this, loop);
    }

    /// <summary>
    /// Create a trigger object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerType">Trigger type. It can be specified in a flag form like TriggerType.HitSoundWhistle | TriggerType.HitSoundSoft.</param>
    /// <param name="listenSample">If use the listenSample, the trigger will listen to all hitsound in a track like HitsoundAllNormal.</param>
    /// <param name="customSampleSet">Listen to a specific track. 0 represents default track.</param>
    /// <remarks>This method returns a standalone disposable trigger object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public IEventHostDisposable CreateTrigger(int startTime, int endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
    {
        var trigger = new Trigger(startTime, endTime, triggerType, listenSample, customSampleSet);
        return new LoopTriggerCreationWrapper(this, trigger);
    }

    /// <summary>
    /// Create a trigger object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerName">A valid trigger name.</param>
    /// <remarks>This method returns a standalone disposable trigger object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public IEventHostDisposable CreateTrigger(int startTime, int endTime, string triggerName)
    {
        var trigger = new Trigger(startTime, endTime, triggerName);
        return new LoopTriggerCreationWrapper(this, trigger);
    }

    public Loop StartLoop(int startTime, int loopCount)
    {
        if (_isLooping || _isTriggering)
            throw new Exception("You can not start another loop when the previous one isn't end.");

        var loop = new Loop(startTime, loopCount);
        AddLoop(loop);
        _isLooping = true;
        return loop;
    }

    /// <summary>
    /// Start a trigger group.
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerType">Trigger type. It can be specified in a flag form like TriggerType.HitSoundWhistle | TriggerType.HitSoundSoft.</param>
    /// <param name="listenSample">If use the listenSample, the trigger will listen to all hitsound in a track like HitsoundAllNormal.</param>
    /// <param name="customSampleSet">Listen to a specific track. 0 represents default track.</param>
    public Trigger StartTrigger(int startTime, int endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
    {
        if (_isLooping || _isTriggering)
            throw new Exception("You can not start another loop when the previous one isn't end.");

        var trig = new Trigger(startTime, endTime, triggerType, listenSample, customSampleSet);
        AddTrigger(trig);
        _isTriggering = true;
        return trig;
    }

    /// <summary>
    /// Start a trigger group.
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerName">A valid trigger name.</param>
    public Trigger StartTrigger(int startTime, int endTime, string triggerName)
    {
        if (_isLooping || _isTriggering)
            throw new Exception("You can not start another loop when the previous one isn't end.");

        var trig = new Trigger(startTime, endTime, triggerName);
        AddTrigger(trig);
        _isTriggering = true;
        return trig;
    }

    public void EndLoop()
    {
        if (!_isLooping && !_isTriggering)
            throw new Exception("You can not stop a loop when a loop isn't started.");

        TryEndLoop();
    }

    public bool TryEndLoop()
    {
        if (!_isLooping && !_isTriggering) return false;
        _isLooping = false;
        _isTriggering = false;
        return true;
    }

    public virtual async Task WriteHeaderAsync(TextWriter writer)
    {
        await writer.WriteAsync(ObjectType.GetString(ObjectType));
        await writer.WriteAsync(',');
        await writer.WriteAsync(LayerType);
        await writer.WriteAsync(',');
        await writer.WriteAsync(OriginType);
        await writer.WriteAsync(",\"");
        await writer.WriteAsync(ImagePath);
        await writer.WriteAsync("\",");
        await writer.WriteStandardizedNumberAsync(DefaultX);
        await writer.WriteAsync(',');
        await writer.WriteStandardizedNumberAsync(DefaultY);
    }

    public async Task WriteScriptAsync(TextWriter writer)
    {
        await WriteHeaderAsync(writer);
        await writer.WriteLineAsync();
        await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
    }

    IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;

    public void AddEvent(IKeyEvent @event)
    {
        if (_isLooping)
            LoopList[LoopList.Count - 1].AddEvent(@event);
        else if (_isTriggering)
            TriggerList[TriggerList.Count - 1].AddEvent(@event);
        else
        {
            _events.Add(@event);
            @event.TimingChanged += ResetCacheAndRaiseTimingChanged;
            ResetCacheAndRaiseTimingChanged();
        }
    }

    public bool RemoveEvent(IKeyEvent @event)
    {
        @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        var removeEvent = _events.Remove(@event);
        if (removeEvent)
            ResetCacheAndRaiseTimingChanged();
        return removeEvent;
    }

    public void AddEvents(IEnumerable<IKeyEvent> events)
    {
        foreach (var keyEvent in events)
        {
            AddEvent(keyEvent);
        }
    }

    public void AddLoop(Loop loop)
    {
        TryEndLoop();
        _loopList.Add(loop);
        loop.TimingChanged += ResetCacheAndRaiseTimingChanged;
        ResetCacheAndRaiseTimingChanged();
    }

    public bool RemoveLoop(Loop loop)
    {
        TryEndLoop();
        loop.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        var removeLoop = _loopList.Remove(loop);
        if (removeLoop)
            ResetCacheAndRaiseTimingChanged();
        return removeLoop;
    }

    public void AddTrigger(Trigger trigger)
    {
        TryEndLoop();
        _triggerList.Add(trigger);
        trigger.TimingChanged += ResetCacheAndRaiseTimingChanged;
    }

    public bool RemoveTrigger(Trigger trigger)
    {
        TryEndLoop();
        trigger.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        var removeTrigger = _triggerList.Remove(trigger);
        if (removeTrigger)
            ResetCacheAndRaiseTimingChanged();
        return removeTrigger;
    }

    public void ResetEventCollection(IComparer<IKeyEvent>? comparer)
    {
        var valid = _events.Count > 0;
        foreach (var @event in _events)
            @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        _events.Clear();
        if (comparer == null)
            _events = new HashSet<IKeyEvent>();
        else
            _events = new SortedSet<IKeyEvent>(comparer);
        if (valid)
            ResetCacheAndRaiseTimingChanged();
    }

    public void ClearLoops()
    {
        TryEndLoop();
        var valid = _loopList.Count > 0;
        for (var i = 0; i < _loopList.Count; i++)
        {
            var loop = _loopList[i];
            loop.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        }

        _loopList.Clear();
        if (valid)
            ResetCacheAndRaiseTimingChanged();
    }

    public void ClearTriggers()
    {
        TryEndLoop();
        var valid = _triggerList.Count > 0;
        for (var i = 0; i < _triggerList.Count; i++)
        {
            var trigger = _triggerList[i];
            trigger.TimingChanged -= ResetCacheAndRaiseTimingChanged;
        }

        _triggerList.Clear();
        if (valid)
            ResetCacheAndRaiseTimingChanged();
    }

    public object Clone()
    {
        var sprite = new Sprite(LayerType, OriginType, ImagePath, DefaultX, DefaultY)
        {
            DefaultZ = DefaultZ,
            CameraIdentifier = CameraIdentifier,
            EnableGroupedSerialization = EnableGroupedSerialization,
        };

        foreach (var keyEvent in _events)
            sprite.AddEvent((IKeyEvent)keyEvent.Clone());

        if (LoopList.Count > 0)
            foreach (var loop in LoopList)
                sprite.AddLoop((Loop)loop.Clone());

        if (TriggerList.Count > 0)
            foreach (var trigger in TriggerList)
                sprite.AddTrigger((Trigger)trigger.Clone());

        return sprite;
    }

    private void ResetCacheAndRaiseTimingChanged()
    {
        _cachedMaxTime = null;
        _cachedMinTime = null;
        _cachedMaxStartTime = null;
        _cachedMinEndTime = null;
    }
}