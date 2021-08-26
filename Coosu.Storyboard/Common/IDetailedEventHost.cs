using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface IEventHost : IScriptable
    {
        ICollection<IKeyEvent> Events { get; set; }
        void AddEvent(IKeyEvent @event);
    }

    public interface IDetailedEventHost : IScriptable, ICloneable, IEventHost
    {
        double MaxTime { get; }
        double MinTime { get; }
        double MaxStartTime { get; }
        double MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
    }
}