using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.OsbX.SubjectHandlers
{
    public class Camera2Handler : SubjectHandler<Camera2Element>
    {
        static Camera2Handler()
        {
            ObjectTypeManager.SignType(99, "Camera2");
        }

        public Camera2Handler()
        {
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveXActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveYActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<FadeActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<ScaleActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<RotateActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<VectorActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<OriginActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<ZoomInActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<ZoomOutActionHandler>());
        }

        public override string Flag => "Camera2";
        // Camera
        public override string Serialize(Camera2Element raw)
        {
            return $"{Flag},{raw.CameraId}";
        }

        public override Camera2Element Deserialize(string[] split)
        {
            if (split.Length < 1) throw new ArgumentOutOfRangeException();
            
            //var type = ObjectTypeManager.Parse(split[0]);
            int cameraId = 0;
            if (split.Length >= 2)
            {
                cameraId = int.Parse(split[1]);
            }

            return new Camera2Element() { CameraId = cameraId };

        }
    }

    public class Camera2Element : ISceneObject, IDefinedObject
    {
        public ObjectType ObjectType { get; } = 99;
        protected string Header => $"{ObjectTypeManager.GetString(ObjectType)},{CameraId}";
        public float DefaultY { get; set; }
        public float DefaultX { get; set; }
        public float ZDistance { get; set; }
        public int CameraId { get; set; }
        public List<Loop> LoopList { get; } = new();
        public List<Trigger> TriggerList { get; } = new();
        public float MaxTime => Events.Count > 0 ? Events.Max(k => k.EndTime) : 0;
        public float MinTime => Events.Count > 0 ? Events.Min(k => k.StartTime) : 0;
        public float MaxStartTime => Events.Count > 0 ? Events.Max(k => k.StartTime) : 0;
        public float MinEndTime => Events.Count > 0 ? Events.Min(k => k.EndTime) : 0;
        public bool EnableGroupedSerialization { get; set; }

        // EventHosts
        public SortedSet<ICommonEvent> Events { get; } = new(new EventTimingComparer());
        public void AddEvent(ICommonEvent @event)
        {
            Events.Add(@event);
        }

        static Camera2Element()
        {
            ObjectTypeManager.SignType(99, "Camera2");
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await writer.WriteLineAsync(Header);
            await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
        }
    }
}