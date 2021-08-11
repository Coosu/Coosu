using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Events.Containers;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    /// <summary>
    /// Represents a storyboard element. This class cannot be inherited.
    /// </summary>
    public partial class Element : EventContainer
    {
        protected override string Header =>
            $"{ElementTypeSign.GetString(Type)},{Layer},{Origin},\"{ImagePath}\",{DefaultX},{DefaultY}";
        public LayerType Layer { get; }
        public OriginType Origin { get; }
        public string ImagePath { get; }
        public float DefaultY { get; internal set; }
        public float DefaultX { get; internal set; }

        // Containers
        public List<Loop> LoopList { get; } = new();
        public List<Trigger> TriggerList { get; } = new();

        public override Element? BaseElement => null;

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
        private bool _isTriggering = false;
        private bool _isLooping = false;

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
        }

        public Element(string type, string layer, string origin, string imagePath, float defaultX, float defaultY)
        {
            Type = ElementType.Parse(type);
            Layer = (LayerType)Enum.Parse(typeof(LayerType), layer);
            Origin = (OriginType)Enum.Parse(typeof(OriginType), origin);
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
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

        public override async Task WriteScriptAsync(TextWriter sw)
        {
            if (!IsWorthy) return;
            await sw.WriteLineAsync(Header);
            await sw.WriteElementEventsAsync(this, @group);
        }

        public Element Clone() => throw new NotImplementedException();

        public override void Adjust(float offsetX, float offsetY, int offsetTiming)
        {
            DefaultX += offsetX;
            DefaultY += offsetY;

            foreach (var loop in LoopList)
                loop.Adjust(offsetX, offsetY, offsetTiming);

            foreach (var trigger in TriggerList)
                trigger.Adjust(offsetX, offsetY, offsetTiming);

            base.Adjust(offsetX, offsetY, offsetTiming);
        }
    }
}
