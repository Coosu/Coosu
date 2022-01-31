using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    /// <summary>
    /// Represents a storyboard sprite. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("Header = {DebuggerDisplay}")]
    public class Sprite : ISceneObject
    {
        private string DebuggerDisplay => this.GetHeaderString();

        public virtual ObjectType ObjectType { get; } = ObjectTypes.Sprite;

        public int? RowInSource { get; set; }
        public object Tag { get; set; }

        public LayerType LayerType { get; }
        public OriginType OriginType { get; }
        public string ImagePath { get; }
        /// <inheritdoc />
        public float DefaultX { get; set; }
        /// <inheritdoc />
        public float DefaultY { get; set; }

        /// <inheritdoc />
        public float DefaultZ { get; set; }
        /// <inheritdoc />
        public string CameraIdentifier { get; set; } = "00000000-0000-0000-0000-000000000000";

        // EventHosts
        public ICollection<IKeyEvent> Events { get; set; } = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);

        // ISceneObject
        public List<Loop> LoopList { get; private set; } = new();
        public List<Trigger> TriggerList { get; private set; } = new();

        public float MaxTime()
        {
            if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
                return float.NaN;

            var max = Events.Count == 0 ? float.MinValue : Events.Max(k => k.EndTime);
            var loopMax = LoopList.Count == 0 ? float.MinValue : LoopList.Max(k => k.OuterMaxTime());
            max = max >= loopMax ? max : loopMax;

            var triggerMax = TriggerList.Count == 0 ? float.MinValue : TriggerList.Max(k => k.MaxTime());
            max = max >= triggerMax ? max : triggerMax;
            return max;
        }

        public float MinTime()
        {
            if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
                return float.NaN;

            var min = Events.Count == 0 ? float.MaxValue : Events.Min(k => k.StartTime);
            var loopMin = LoopList.Count == 0 ? float.MaxValue : LoopList.Min(k => k.OuterMinTime());
            min = min <= loopMin ? min : loopMin;

            var triggerMin = TriggerList.Count == 0 ? float.MaxValue : TriggerList.Min(k => k.MinTime());
            min = min <= triggerMin ? min : triggerMin;
            return min;
        }

        public float MaxStartTime()
        {
            if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
                return float.NaN;

            var max = Events.Count == 0 ? float.MinValue : Events.Max(k => k.StartTime);
            var loopMax = LoopList.Count == 0 ? float.MinValue : LoopList.Max(k => k.OuterMinTime());
            max = max >= loopMax ? max : loopMax;

            var triggerMax = TriggerList.Count == 0 ? float.MinValue : TriggerList.Max(k => k.MinTime());
            max = max >= triggerMax ? max : triggerMax;
            return max;
        }

        public float MinEndTime()
        {
            if (Events.Count == 0 && LoopList.Count == 0 && TriggerList.Count == 0)
                return float.NaN;

            var min = Events.Count == 0 ? float.MaxValue : Events.Min(k => k.EndTime);
            var loopMin = LoopList.Count == 0 ? float.MaxValue : LoopList.Min(k => k.OuterMaxTime());
            min = min <= loopMin ? min : loopMin;

            var triggerMin = TriggerList.Count == 0 ? float.MaxValue : TriggerList.Min(k => k.MaxTime());
            min = min <= triggerMin ? min : triggerMin;
            return min;
        }

        public bool EnableGroupedSerialization { get; set; }/* = true;*/

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
        public Sprite(LayerType layerType, OriginType originType, string imagePath, float defaultX, float defaultY)
        {
            LayerType = layerType;
            OriginType = originType;
            ImagePath = imagePath;
            DefaultX = defaultX;
            DefaultY = defaultY;
        }

        public Sprite(ReadOnlySpan<char> layer, ReadOnlySpan<char> origin, ReadOnlySpan<char> imagePath,
            float defaultX, float defaultY)
        {
            //ObjectType = OsbObjectType.Parse(type);
            LayerType = layer.ToLayerType();
            OriginType = origin.ToOriginType();
            ImagePath = imagePath.ToString();
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
            await WriteHeaderAsync(writer);
            await writer.WriteLineAsync();
            await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
        }

        public void AddEvent(IKeyEvent @event)
        {
            if (_isLooping)
                LoopList[LoopList.Count - 1].AddEvent(@event);
            else if (_isTriggering)
                TriggerList[TriggerList.Count - 1].AddEvent(@event);
            else
                Events.Add(@event);
        }

        public object Clone()
        {
            var sprite = new Sprite(LayerType, OriginType, ImagePath, DefaultX, DefaultY)
            {
                DefaultZ = DefaultZ,
                CameraIdentifier = CameraIdentifier,
                EnableGroupedSerialization = EnableGroupedSerialization,
                Events = Events.Select(k => k.Clone()).Cast<IKeyEvent>().ToList(),
            };

            if (LoopList != null)
                sprite.LoopList = new List<Loop>(LoopList.Select(k => k.Clone()).Cast<Loop>());

            if (TriggerList != null)
                sprite.TriggerList = new List<Trigger>(TriggerList.Select(k => k.Clone()).Cast<Trigger>());

            return sprite;
        }
    }
}
