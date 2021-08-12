using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface IEventHost : IScriptable
    {
        float MaxTime { get; }
        float MinTime { get; }
        float MaxStartTime { get; }
        float MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
        SortedSet<ICommonEvent> Events { get; }
        void AddEvent(ICommonEvent @event);
    }
}