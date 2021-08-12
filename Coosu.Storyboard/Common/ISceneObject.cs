using System.Collections.Generic;
using Coosu.Storyboard.Events.EventHosts;

namespace Coosu.Storyboard.Common
{
    public interface ISceneObject : IDefinedObject, IEventHost
    {
        public float DefaultY { get; set; }
        public float DefaultX { get; set; }
        public float ZDistance { get; }
        public int CameraId { get; }
        List<Loop> LoopList { get; }
        List<Trigger> TriggerList { get; }
    }
}