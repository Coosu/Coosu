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
        float MaxTime();
        float MinTime();
        float MaxStartTime();
        float MinEndTime();

        bool EnableGroupedSerialization { get; set; }
    }
}