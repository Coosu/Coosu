using System.Collections.Generic;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Events.EventHosts;

namespace Coosu.Storyboard
{
    public interface ISubEventHost : IEventHost
    {
        ISceneObject? BaseObject { get; internal set; }
    }

    public interface IEventHost : IScriptable
    {
        float MaxTime { get; }
        float MinTime { get; }
        float MaxStartTime { get; }
        float MinEndTime { get; }

        bool EnableGroupedSerialization { get; set; }
        SortedSet<CommonEvent> Events { get; }
    }

    public interface IOsbObject
    {
        OsbObjectType ObjectType { get; }
    }

    public interface ISceneObject : IOsbObject, IEventHost
    {
        public float DefaultY { get; set; }
        public float DefaultX { get; set; }
        public float ZDistance { get; }
        public int CameraId { get; }
        List<Loop> LoopList { get; }
        List<Trigger> TriggerList { get; }
    }
}