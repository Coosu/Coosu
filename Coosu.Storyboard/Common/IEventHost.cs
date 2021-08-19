using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface IEventHost : IScriptable, ICloneable
    {
        double MaxTime { get; }
        double MinTime { get; }
        double MaxStartTime { get; }
        double MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
        ICollection<ICommonEvent> Events { get; set; }
        void AddEvent(ICommonEvent @event);
    }
}