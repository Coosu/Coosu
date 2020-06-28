using OSharp.Storyboard.Common;
using OSharp.Storyboard.Events;
using OSharp.Storyboard.Events.Containers;
using OSharp.Storyboard.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OSharp.Storyboard
{
    /// <summary>
    /// Represents a storyboard element. This class cannot be inherited.
    /// </summary>
    public class Element : EventContainer
    {
        protected override string Head =>
            $"{Type},{Layer},{Origin},\"{ImagePath}\",{DefaultX},{DefaultY}";
        public ElementType Type { get; }
        public LayerType Layer { get; }
        public OriginType Origin { get; }
        public string ImagePath { get; }
        public float DefaultY { get; internal set; }
        public float DefaultX { get; internal set; }

        // Containers
        public List<Loop> LoopList { get; } = new List<Loop>();
        public List<Trigger> TriggerList { get; } = new List<Trigger>();

        public new Element BaseElement => null;

        public override float MaxTime =>
             NumericUtility.GetMaxValue(
                 EventList.Select(k => k.EndTime),
                 LoopList.Select(k => k.OuterMaxTime),
                 TriggerList.Select(k => k.MaxTime)
             );

        public override float MinTime =>
            NumericUtility.GetMinValue(
                EventList.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public override float MaxStartTime =>
            NumericUtility.GetMaxValue(
                EventList.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public override float MinEndTime =>
            NumericUtility.GetMinValue(
                EventList.Select(k => k.EndTime),
                LoopList.Select(k => k.OuterMaxTime),
                TriggerList.Select(k => k.MaxTime)
            );

        public bool IsWorthy => !MinTime.Equals(MaxTime) || IsBackground;

        public override int MaxTimeCount
        {
            get
            {
                var maxTime = MaxTime;
                return EventList.Count(k => k.EndTime.Equals(maxTime)) +
                       LoopList.Count(k => k.OuterMaxTime.Equals(maxTime)) +
                       TriggerList.Count(k => k.MaxTime.Equals(maxTime));
            }
        }

        public override int MinTimeCount
        {
            get
            {
                var minTime = MinTime;
                return EventList.Count(k => k.StartTime.Equals(minTime)) +
                       LoopList.Count(k => k.OuterMinTime.Equals(minTime)) +
                       TriggerList.Count(k => k.MinTime.Equals(minTime));
            }
        }

        public bool IsBackground { get; internal set; }
        public int RowInSource { get; internal set; }

        // Loop control
        private bool _isTriggering, _isLooping;

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="type">Set element type.</param>
        /// <param name="layer">Set element layer.</param>
        /// <param name="origin">Set element origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        public Element(ElementType type, LayerType layer, OriginType origin, string imagePath, float defaultX, float defaultY)
        {
            Type = type;
            Layer = layer;
            Origin = origin;
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
            _isLooping = false;
        }

        public Element(string type, string layer, string origin, string imagePath, float defaultX, float defaultY)
        {
            Type = (ElementType)Enum.Parse(typeof(ElementType), type);
            Layer = (LayerType)Enum.Parse(typeof(LayerType), layer);
            Origin = (OriginType)Enum.Parse(typeof(OriginType), origin);
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
            _isLooping = false;
        }

        public Loop StartLoop(int startTime, int loopCount)
        {
            if (_isLooping || _isTriggering)
                throw new Exception("You can not start another loop when the previous one isn't end.");

            _isLooping = true;
            var loop = new Loop(startTime, loopCount);
            LoopList.Add(loop);
            return loop;
        }

        /// <summary>
        /// Start a trigger group.
        /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
        /// </summary>
        /// <param name="startTime">Group start time.</param>
        /// <param name="endTime">Group end time.</param>
        /// <param name="triggerType">Trigger type. It can be specified in a flag form like TriggerType.HitSoundWhistle | TriggerType.HitSoundSoft.</param>
        /// <param name="listenSample">If use the listenSample, the trigger will listen to all hitsound in a track like HitsoundAllNormal.</param>
        /// <param name="customSampleSet">Listen to a specific track. 0 represents default track.</param>
        public Trigger StartTrigger(int startTime, int endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
        {
            if (_isLooping || _isTriggering)
                throw new Exception("You can not start another loop when the previous one isn't end.");

            _isTriggering = true;
            var trig = new Trigger(startTime, endTime, triggerType, listenSample, customSampleSet);
            TriggerList.Add(trig);
            return trig;
        }

        /// <summary>
        /// Start a trigger group.
        /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
        /// </summary>
        /// <param name="startTime">Group start time.</param>
        /// <param name="endTime">Group end time.</param>
        /// <param name="triggerName">A valid trigger name.</param>
        public Trigger StartTrigger(int startTime, int endTime, string triggerName)
        {
            if (_isLooping || _isTriggering)
                throw new Exception("You can not start another loop when the previous one isn't end.");

            _isTriggering = true;
            var trig = new Trigger(startTime, endTime, triggerName);
            TriggerList.Add(trig);
            return trig;
        }

        public void EndLoop()
        {
            if (!_isLooping && !_isTriggering)
                throw new Exception("You can not stop a loop when a loop isn't started.");

            TryEndLoop();
        }

        public bool TryEndLoop()
        {
            if (!_isLooping && !_isTriggering) return false;
            _isLooping = false;
            _isTriggering = false;
            return true;
        }

        #region 折叠：Event function

        // Move
        public void Move(int startTime, Vector2 point) =>
            AddEvent(EventType.Move, 0, startTime, startTime, point.X, point.Y, point.X, point.Y);
        public void Move(int startTime, float x, float y) =>
            AddEvent(EventType.Move, 0, startTime, startTime, x, y, x, y);
        public void Move(int startTime, int endTime, float x, float y) =>
            AddEvent(EventType.Move, 0, startTime, endTime, x, y, x, y);
        public void Move(int startTime, int endTime, float x1, float y1, float x2, float y2) =>
            AddEvent(EventType.Move, 0, startTime, endTime, x1, y1, x2, y2);
        public void Move(int startTime, int endTime, Vector2 startPoint, Vector2 endPoint) =>
            AddEvent(EventType.Move, 0, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        public void Move(EasingType easing, int startTime, int endTime, Vector2 startPoint, Vector2 endPoint) =>
            AddEvent(EventType.Move, easing, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        public void Move(EasingType easing, int startTime, int endTime, float x1, float y1, float x2, float y2) =>
            AddEvent(EventType.Move, easing, startTime, endTime, x1, y1, x2, y2);

        // Fade
        public void Fade(int startTime, float opacity) =>
            AddEvent(EventType.Fade, 0, startTime, startTime, opacity, opacity);
        public void Fade(int startTime, int endTime, float opacity) =>
            AddEvent(EventType.Fade, 0, startTime, endTime, opacity, opacity);
        public void Fade(int startTime, int endTime, float startOpacity, float endOpacity) =>
            AddEvent(EventType.Fade, 0, startTime, endTime, startOpacity, endOpacity);
        public void Fade(EasingType easing, int startTime, int endTime, float startOpacity, float endOpacity) =>
            AddEvent(EventType.Fade, easing, startTime, endTime, startOpacity, endOpacity);

        // Scale
        public void Scale(int startTime, float scale) =>
            AddEvent(EventType.Scale, 0, startTime, startTime, scale, scale);
        public void Scale(int startTime, int endTime, float scale) =>
            AddEvent(EventType.Scale, 0, startTime, endTime, scale, scale);
        public void Scale(int startTime, int endTime, float startScale, float endScale) =>
            AddEvent(EventType.Scale, 0, startTime, endTime, startScale, endScale);
        public void Scale(EasingType easing, int startTime, int endTime, float startScale, float endScale) =>
            AddEvent(EventType.Scale, easing, startTime, endTime, startScale, endScale);

        // Rotate
        public void Rotate(int startTime, float rotate) =>
            AddEvent(EventType.Rotate, 0, startTime, startTime, rotate, rotate);
        public void Rotate(int startTime, int endTime, float rotate) =>
            AddEvent(EventType.Rotate, 0, startTime, endTime, rotate, rotate);
        public void Rotate(int startTime, int endTime, float startRotate, float endRotate) =>
            AddEvent(EventType.Rotate, 0, startTime, endTime, startRotate, endRotate);
        public void Rotate(EasingType easing, int startTime, int endTime, float startRotate, float endRotate) =>
            AddEvent(EventType.Rotate, easing, startTime, endTime, startRotate, endRotate);

        // MoveX
        public void MoveX(int startTime, float x) =>
            AddEvent(EventType.MoveX, 0, startTime, startTime, x, x);
        public void MoveX(int startTime, int endTime, float x) =>
            AddEvent(EventType.MoveX, 0, startTime, endTime, x, x);
        public void MoveX(int startTime, int endTime, float startX, float endX) =>
            AddEvent(EventType.MoveX, 0, startTime, endTime, startX, endX);
        public void MoveX(EasingType easing, int startTime, int endTime, float startX, float endX) =>
            AddEvent(EventType.MoveX, easing, startTime, endTime, startX, endX);

        // MoveY
        public void MoveY(int startTime, float y) =>
            AddEvent(EventType.MoveY, 0, startTime, startTime, y, y);
        public void MoveY(int startTime, int endTime, float y) =>
            AddEvent(EventType.MoveY, 0, startTime, endTime, y, y);
        public void MoveY(int startTime, int endTime, float startY, float endY) =>
            AddEvent(EventType.MoveY, 0, startTime, endTime, startY, endY);
        public void MoveY(EasingType easing, int startTime, int endTime, float startY, float endY) =>
            AddEvent(EventType.MoveY, easing, startTime, endTime, startY, endY);

        // Color
        public void Color(int startTime, Vector3 color) =>
            AddEvent(EventType.Color, 0, startTime, startTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        public void Color(int startTime, int endTime, Vector3 color) =>
            AddEvent(EventType.Color, 0, startTime, endTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        public void Color(int startTime, int endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventType.Color, 0, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        public void Color(EasingType easing, int startTime, int endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventType.Color, easing, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        public void Color(int startTime, int r, int g, int b) =>
            AddEvent(EventType.Color, 0, startTime, startTime, r, g, b, r, g, b);
        public void Color(int startTime, int endTime, int r, int g, int b) =>
            AddEvent(EventType.Color, 0, startTime, endTime, r, g, b, r, g, b);
        public void Color(int startTime, int endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventType.Color, 0, startTime, endTime, startR, startG, startB, endR, endG, endB);
        public void Color(EasingType easing, int startTime, int endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventType.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);
        public void Color(EasingType easing, int startTime, int endTime, float startR, float startG, float startB, float endR, float endG, float endB) =>
            AddEvent(EventType.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);

        // Vector
        public void Vector(int startTime, Vector2 vector) =>
            AddEvent(EventType.Vector, 0, startTime, startTime, vector.X, vector.Y, vector.X, vector.Y);
        public void Vector(int startTime, float w, float h) =>
            AddEvent(EventType.Vector, 0, startTime, startTime, w, h, w, h);
        public void Vector(int startTime, int endTime, float w, float h) =>
            AddEvent(EventType.Vector, 0, startTime, endTime, w, h, w, h);
        public void Vector(int startTime, int endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventType.Vector, 0, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public void Vector(EasingType easing, int startTime, int endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventType.Vector, easing, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public void Vector(int startTime, int endTime, float w1, float h1, float w2, float h2) =>
            AddEvent(EventType.Vector, 0, startTime, endTime, w1, h1, w2, h2);
        public void Vector(EasingType easing, int startTime, int endTime, float w1, float h1, float w2, float h2) =>
            AddEvent(EventType.Vector, easing, startTime, endTime, w1, h1, w2, h2);

        //Extra
        public void FlipH(int startTime) => AddEvent(0, startTime, startTime, ParameterType.Horizontal);
        public void FlipH(int startTime, int endTime) => AddEvent(0, startTime, endTime, ParameterType.Horizontal);

        public void FlipV(int startTime) => AddEvent(0, startTime, startTime, ParameterType.Vertical);
        public void FlipV(int startTime, int endTime) => AddEvent(0, startTime, endTime, ParameterType.Vertical);

        public void Additive(int startTime) =>
            AddEvent(0, startTime, startTime, ParameterType.Additive);
        public void Additive(int startTime, int endTime) =>
            AddEvent(0, startTime, endTime, ParameterType.Additive);

        public void Parameter(EasingType easing, int startTime, int endTime, ParameterType p) =>
            AddEvent(easing, startTime, endTime, p);

        #endregion

        public override string ToString() => Head;

        public override void WriteOsbString(TextWriter sw, bool group = false)
        {
            if (!IsWorthy) return;
            sw.WriteLine(Head);
            sw.WriteElementEvents(this, group);
        }

        public Element Clone() => (Element)MemberwiseClone();

        public override void Adjust(float offsetX, float offsetY, int offsetTiming)
        {
            DefaultX += offsetX;
            DefaultY += offsetY;

            if (LoopList != null)
                foreach (var loop in LoopList)
                    loop.Adjust(offsetX, offsetY, offsetTiming);
            if (TriggerList != null)
                foreach (var trigger in TriggerList)
                    trigger.Adjust(offsetX, offsetY, offsetTiming);

            base.Adjust(offsetX, offsetY, offsetTiming);
        }

        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float x1, float x2) =>
            AddEvent(e, easing, startTime, endTime, new[] { x1 }, new[] { x2 });
        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float x1, float y1, float x2, float y2) =>
            AddEvent(e, easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 });
        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime,
            float x1, float y1, float z1, float x2, float y2, float z2) =>
            AddEvent(e, easing, startTime, endTime, new[] { x1, y1, z1 }, new[] { x2, y2, z2 });
        public void AddEvent(EasingType easing, float startTime, float endTime, ParameterType p) =>
            AddEvent(EventType.Parameter, easing, startTime, endTime, new[] { (float)(int)p }, new[] { (float)(int)p });

        internal override void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float[] start, float[] end)
        {
            if (_isLooping)
                LoopList[LoopList.Count - 1].AddEvent(e, easing, startTime, endTime, start, end);
            else if (_isTriggering)
                TriggerList[TriggerList.Count - 1].AddEvent(e, easing, startTime, endTime, start, end);
            else
                base.AddEvent(e, easing, startTime, endTime, start, end);
        }
    }
}
