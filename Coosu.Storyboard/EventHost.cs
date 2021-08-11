using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Management;

namespace Coosu.Storyboard
{
    public abstract class EventHost : IScriptable, IEventHost
    {
        //public EventHandler<ProcessErrorEventArgs>? OnErrorOccurred;
        public SortedSet<CommonEvent> Events { get; } = new(new EventTimingComparer());
        public abstract float MaxTime { get; }
        public abstract float MinTime { get; }
        public abstract float MaxStartTime { get; }
        public abstract float MinEndTime { get; }


        //public TimeRange ObsoleteList { get; } = new TimeRange();

        internal virtual void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float[] start, float[]? end)
        {
            CommonEvent newCommonEvent;
            if (end == null || end.Length == 0)
                end = start;

            if (e == EventTypes.Fade)
            {
                newCommonEvent = new Fade(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Move)
            {
                newCommonEvent = new Move(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.MoveX)
            {
                newCommonEvent = new MoveX(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.MoveY)
            {
                newCommonEvent = new MoveY(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Scale)
            {
                newCommonEvent = new Scale(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Vector)
            {
                newCommonEvent = new Vector(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.Rotate)
            {
                newCommonEvent = new Rotate(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Color)
            {
                newCommonEvent = new Color(easing, startTime, endTime, start[0], start[1], start[2], end[0], end[1],
                    end[2]);
            }
            else if (e == EventTypes.Parameter)
            {
                newCommonEvent = new Parameter(easing, startTime, endTime, (ParameterType)(int)start[0]);
            }
            else
            {
                var result = Register.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, start, end);
                newCommonEvent = result ?? throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
            
            Events.Add(newCommonEvent);
        }

        public bool EnableGroupedSerialization { get; set; }

        protected abstract string Header { get; }
        public abstract Task WriteScriptAsync(TextWriter writer);
    }
}
