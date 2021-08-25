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
    public class Camera2Handler : SubjectHandler<Camera2Object>
    {
        static Camera2Handler()
        {
            ObjectType.SignType(99, "Camera2");
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
        public override string Serialize(Camera2Object raw)
        {
            return $"{Flag},{raw.CameraIdentifier}";
        }

        public override Camera2Object Deserialize(string[] split)
        {
            if (split.Length < 1) throw new ArgumentOutOfRangeException();

            //var type = ObjectTypeManager.Parse(split[0]);
            string cameraIdentifier = Guid.Empty.ToString();
            if (split.Length >= 2)
            {
                cameraIdentifier = (split[1]);
            }

            return new Camera2Object { CameraIdentifier = cameraIdentifier };

        }
    }

    public class Camera2Object : ISceneObject, IDefinedObject
    {
        public ObjectType ObjectType { get; } = 99;
        public int? RowInSource { get; set; }
        public double DefaultY { get; set; }
        public double DefaultX { get; set; }
        public double ZDistance { get; set; }
        public string CameraIdentifier { get; set; }
        public List<Loop> LoopList { get; } = new();
        public List<Trigger> TriggerList { get; } = new();
        public double MaxTime => Events.Count > 0 ? Events.Max(k => k.EndTime) : 0;
        public double MinTime => Events.Count > 0 ? Events.Min(k => k.StartTime) : 0;
        public double MaxStartTime => Events.Count > 0 ? Events.Max(k => k.StartTime) : 0;
        public double MinEndTime => Events.Count > 0 ? Events.Min(k => k.EndTime) : 0;
        public bool EnableGroupedSerialization { get; set; }

        // EventHosts
        public ICollection<ICommonEvent> Events { get; set; } =
            new SortedSet<ICommonEvent>(new EventTimingComparer());

        public void AddEvent(ICommonEvent @event)
        {
            Events.Add(@event);
        }

        static Camera2Object()
        {
            ObjectType.SignType(99, "Camera2");
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync($"{ObjectType.GetString(ObjectType)},{CameraIdentifier}");
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await WriteHeaderAsync(writer);
            await writer.WriteLineAsync();
            await ScriptHelper.WriteElementEventsAsync(writer, this, EnableGroupedSerialization);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}