using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Events.EventHosts;
using Coosu.Storyboard.Management;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    /// <summary>
    /// Represents a storyboard element. This class cannot be inherited.
    /// </summary>
    public partial class Sprite : EventHost, ISceneObject
    {
        protected override string Header =>
            $"{ObjectTypeManager.GetString(ObjectType)},{LayerType},{OriginType},\"{ImagePath}\",{DefaultX},{DefaultY}";
        public OsbObjectType ObjectType { get; protected set; }
        public LayerType LayerType { get; }
        public OriginType OriginType { get; }
        public string ImagePath { get; }
        public float DefaultY { get; set; }
        public float DefaultX { get; set; }

        public float ZDistance { get; set; }
        public int CameraId { get; set; }

        // EventHosts
        public List<Loop> LoopList { get; } = new();
        public List<Trigger> TriggerList { get; } = new();

        public override float MaxTime =>
             NumericUtility.GetMaxValue(
                 Events.Select(k => k.EndTime),
                 LoopList.Select(k => k.OuterMaxTime),
                 TriggerList.Select(k => k.MaxTime)
             );

        public override float MinTime =>
            NumericUtility.GetMinValue(
                Events.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public override float MaxStartTime =>
            NumericUtility.GetMaxValue(
                Events.Select(k => k.StartTime),
                LoopList.Select(k => k.OuterMinTime),
                TriggerList.Select(k => k.MinTime)
            );

        public override float MinEndTime =>
            NumericUtility.GetMinValue(
                Events.Select(k => k.EndTime),
                LoopList.Select(k => k.OuterMaxTime),
                TriggerList.Select(k => k.MaxTime)
            );

        //public bool IsWorthy => !MinTime.Equals(MaxTime) || IsBackground;
        //public bool IsBackground { get; internal set; }
        //public int RowInSource { get; internal set; }

        // Loop control
        private bool _isTriggering = false;
        private bool _isLooping = false;

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="type">Set element type.</param>
        /// <param name="layerType">Set element layer.</param>
        /// <param name="originType">Set element origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        public Sprite(OsbObjectType type, LayerType layerType, OriginType originType, string imagePath, float defaultX, float defaultY)
        {
            ObjectType = type;
            LayerType = layerType;
            OriginType = originType;
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
        }

        public Sprite(string type, string layer, string origin, string imagePath, float defaultX, float defaultY)
        {
            ObjectType = OsbObjectType.Parse(type);
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

        public override async Task WriteScriptAsync(TextWriter sw)
        {
            //if (!IsWorthy) return;
            await sw.WriteLineAsync(Header);
            await sw.WriteElementEventsAsync(this, EnableGroupedSerialization);
        }

        public Sprite Clone() => throw new NotImplementedException();
    }
}
