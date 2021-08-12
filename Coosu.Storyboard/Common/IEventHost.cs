using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface IEventHost : IScriptable
    {
        double MaxTime { get; }
        double MinTime { get; }
        double MaxStartTime { get; }
        double MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
        SortedSet<ICommonEvent> Events { get; }
        void AddEvent(ICommonEvent @event);
    }
}