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
        float MaxTime { get; }
        float MinTime { get; }
        float MaxStartTime { get; }
        float MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
    }
}