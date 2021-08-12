using System.Collections.Generic;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Common
{
    public interface ISceneObject : IDefinedObject, IEventHost
    {
        public double DefaultY { get; set; }
        public double DefaultX { get; set; }
        public double ZDistance { get; }
        public int CameraId { get; }
        List<Loop> LoopList { get; }
        List<Trigger> TriggerList { get; }
    }
}