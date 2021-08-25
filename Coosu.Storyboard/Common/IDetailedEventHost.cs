using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface IEventHost : IScriptable
    {
        ICollection<ICommonEvent> Events { get; set; }
        void AddEvent(ICommonEvent @event);
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