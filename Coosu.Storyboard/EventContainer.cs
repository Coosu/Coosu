﻿using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coosu.Storyboard
{
    public abstract class EventContainer
    {
        public ElementType Type { get; protected set; }

        public EventHandler<ErrorEventArgs> OnErrorOccured;
        protected abstract string Head { get; }
        public SortedSet<CommonEvent> EventList { get; } = new SortedSet<CommonEvent>(new EventComparer());
        //public List<Event> EventList { get; } = new List<Event>();
        public abstract float MaxTime { get; }
        public abstract float MinTime { get; }
        public abstract float MaxStartTime { get; }
        public abstract float MinEndTime { get; }

        public virtual int MaxTimeCount => EventList.Count(k => k.EndTime.Equals(MaxTime));
        public virtual int MinTimeCount => EventList.Count(k => k.StartTime.Equals(MinTime));

        public float ZDistance { get; set; }
        public int CameraId { get; set; }

        // Extension
        public IEnumerable<Fade> FadeList =>
            EventList.Where(k => k.EventType == EventTypes.Fade).Select(k => k as Fade);
        public IEnumerable<Color> ColorList =>
            EventList.Where(k => k.EventType == EventTypes.Color).Select(k => k as Color);
        public IEnumerable<Move> MoveList =>
            EventList.Where(k => k.EventType == EventTypes.Move).Select(k => k as Move);
        public IEnumerable<MoveX> MoveXList =>
            EventList.Where(k => k.EventType == EventTypes.MoveX).Select(k => k as MoveX);
        public IEnumerable<MoveY> MoveYList =>
            EventList.Where(k => k.EventType == EventTypes.MoveY).Select(k => k as MoveY);
        public IEnumerable<Parameter> ParameterList =>
            EventList.Where(k => k.EventType == EventTypes.Parameter).Select(k => k as Parameter);
        public IEnumerable<Rotate> RotateList =>
            EventList.Where(k => k.EventType == EventTypes.Rotate).Select(k => k as Rotate);
        public IEnumerable<Scale> ScaleList =>
            EventList.Where(k => k.EventType == EventTypes.Scale).Select(k => k as Scale);
        public IEnumerable<Vector> VectorList =>
            EventList.Where(k => k.EventType == EventTypes.Vector).Select(k => k as Vector);

        public Element BaseElement { get; internal set; }

        public virtual void Adjust(float offsetX, float offsetY, int offsetTiming)
        {
            var events = EventList.GroupBy(k => k.EventType);
            foreach (var kv in events)
            {
                foreach (var e in kv)
                {
                    if (e is IAdjustablePositionEvent adjustable)
                        adjustable.AdjustPosition(offsetX, offsetY);

                    e.AdjustTiming(offsetTiming);
                }
            }
        }

        public TimeRange ObsoleteList { get; } = new TimeRange();

        internal virtual void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float[] start, float[] end)
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
                newCommonEvent = Register.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, start, end);
                if (newCommonEvent == null)
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            //List
            //if (sequential)
            //    EventList.AddSorted(newEvent);
            //else
            //    EventList.Add(newEvent);

            //SortedSet
            EventList.Add(newCommonEvent);
        }

        public virtual string ToOsbString(bool group = false)
        {
            using (var sw = new StringWriter())
            {
                WriteOsbString(sw, group);
                return sw.ToString();
            }
        }

        public abstract void WriteOsbString(TextWriter sw, bool group = false);
    }
}
