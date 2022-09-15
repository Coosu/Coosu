using System;
using System.Collections;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common;

public interface IEventHost : IScriptable
{
    IReadOnlyCollection<IKeyEvent> Events { get; }
    void AddEvent(IKeyEvent @event);
    bool RemoveEvent(IKeyEvent @event);
    void ResetEventCollection(IComparer<IKeyEvent>? comparer);
}

public interface IDetailedEventHost : IScriptable, ICloneable, IEventHost
{
    double MaxTime();
    double MinTime();
    double MaxStartTime();
    double MinEndTime();

    bool EnableGroupedSerialization { get; set; }
}