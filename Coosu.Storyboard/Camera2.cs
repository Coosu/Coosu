using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public class Camera2 : IEventHost, ICameraUsable
    {
        public string CameraIdentifier { get; set; } = Guid.NewGuid().ToString();
        public double ZDistance { get; set; } = 1;
        public double DefaultX { get; set; } = 320;
        public double DefaultY { get; set; } = 240;
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

        public Task WriteHeaderAsync(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public Task WriteScriptAsync(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}