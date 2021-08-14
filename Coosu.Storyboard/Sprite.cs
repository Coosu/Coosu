using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    /// <summary>
    /// Represents a storyboard sprite. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("Header = {DebuggerDisplay}")]
    public partial class Sprite : ISceneObject
    {
        private string DebuggerDisplay => this.GetHeaderString();

        public virtual ObjectType ObjectType { get; } = ObjectTypes.Sprite;

        public int? RowInSource { get; set; }

        public LayerType LayerType { get; }
        public OriginType OriginType { get; }
        public string ImagePath { get; }
        public double DefaultY { get; set; }
        public double DefaultX { get; set; }

        public double ZDistance { get; set; }
        public int CameraId { get; set; }

        // EventHosts
        public ICollection<ICommonEvent> Events { get; set; } =
            new SortedSet<ICommonEvent>(new EventTimingComparer());

        // ISceneObject
        public List<Loop> LoopList { get; } = new();
        public List<Trigger> TriggerList { get; } = new();

        public double MaxTime =>
             NumericHelper.GetMaxValue(
                 Events.Select(k => k.EndTime),
                 LoopList.Select(k => k.OuterMaxTime),
                 TriggerList.Select(k => k.MaxTime)
             );

        public double MinTime =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public double MaxStartTime =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public double MinEndTime =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.EndTime),
                LoopList.Select(k => k.OuterMaxTime),
                TriggerList.Select(k => k.MaxTime)
            );
        public bool EnableGroupedSerialization { get; set; }/* = true;*/

        //public bool IsWorthy => !MinTime.Equals(MaxTime) || IsBackground;
        //public bool IsBackground { get; internal set; }

        // Loop control
        private bool _isTriggering;
        private bool _isLooping;

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="layerType">Set sprite layer.</param>
        /// <param name="originType">Set sprite origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        public Sprite(LayerType layerType, OriginType originType, string imagePath, double defaultX, double defaultY)
        {
            LayerType = layerType;
            OriginType = originType;
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
        }

        public Sprite(string layer, string origin, string imagePath, double defaultX, double defaultY)
        {
            //ObjectType = OsbObjectType.Parse(type);
            LayerType = (LayerType)Enum.Parse(typeof(LayerType), layer);
            OriginType = (OriginType)Enum.Parse(typeof(OriginType), origin);
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

        public virtual async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync(ObjectType.GetString(ObjectType));
            await writer.WriteAsync(',');
            await writer.WriteAsync(LayerType);
            await writer.WriteAsync(',');
            await writer.WriteAsync(OriginType);
            await writer.WriteAsync(",\"");
            await writer.WriteAsync(ImagePath);
            await writer.WriteAsync("\",");
            await writer.WriteAsync(DefaultX);
            await writer.WriteAsync(',');
            await writer.WriteAsync(DefaultY);
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            //if (!IsWorthy) return;
            await WriteHeaderAsync(writer);
            await writer.WriteLineAsync();
            await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
        }

        public void AddEvent(ICommonEvent @event)
        {
            if (_isLooping)
                LoopList[LoopList.Count - 1].AddEvent(@event);
            else if (_isTriggering)
                TriggerList[TriggerList.Count - 1].AddEvent(@event);
            else
                Events.Add(@event);
        }

        public Sprite Clone() => throw new NotImplementedException();
    }
}
