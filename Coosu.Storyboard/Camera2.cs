using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public class Camera2 : IEventHost, ICameraUsable, ICloneable
    {
        public string CameraIdentifier { get; set; } = Guid.NewGuid().ToString();
        public double DefaultX { get; set; } = 320;
        public double DefaultY { get; set; } = 240;
        public double DefaultZ { get; set; } = 1;
        public OriginType OriginType { get; set; } = OriginType.Centre;
        public ICollection<ICommonEvent> Events { get; set; } = new List<ICommonEvent>();

        public void AddEvent(ICommonEvent @event)
        {
            Events.Add(@event);
        }

        public State GetCameraState(double time)
        {
            return new State();
        }

        public State GetObjectState(double time, State oldState)
        {
            return oldState;
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
        }

        public object Clone()
        {
            return new Camera2
            {
                CameraIdentifier = CameraIdentifier,
                DefaultX = DefaultX,
                DefaultY = DefaultY,
                DefaultZ = DefaultZ,
                OriginType = OriginType,
                Events = Events.Select(k=>k.Clone()).Cast<ICommonEvent>().ToList()
            };
        }
    }
}