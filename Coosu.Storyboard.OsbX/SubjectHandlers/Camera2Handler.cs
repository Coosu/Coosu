using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Management;
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
            RegisterAction(Register.GetActionHandlerInstance<MoveActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveXActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveYActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<FadeActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<ScaleActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<RotateActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<VectorActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<OriginActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<ZoomInActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<ZoomOutActionHandler>());
        }

        public override string Flag => "Camera2";
        // Camera
        public override string Serialize(Camera2Element raw)
        {
            return $"{Flag},{raw.CameraId}";
        }

        public override Camera2Element Deserialize(string[] split)
        {
            if (split.Length >= 1)
            {
                var type = ObjectTypeManager.Parse(split[0]);
                int cameraId = 0;
                if (split.Length >= 2)
                {
                    cameraId = int.Parse(split[1]);
                }

                return new Camera2Element(type) { CameraId = cameraId };
            }

            throw new ArgumentOutOfRangeException();
        }
    }

    public class Camera2Element : EventHost, IOsbObject
    {
        public OsbObjectType ObjectType { get; }
        protected override string Header => $"{ObjectTypeManager.GetString(ObjectType)},{CameraId}";

        static Camera2Element()
        {
            ObjectTypeManager.SignType(99, "Camera2");
        }

        public Camera2Element(OsbObjectType type)
        {
            ObjectType = type;
        }

        public override float MaxTime =>
            NumericUtility.GetMaxValue(Events.Select(k => k.EndTime));

        public override float MinTime =>
            NumericUtility.GetMinValue(Events.Select(k => k.StartTime));

        public override float MaxStartTime =>
            NumericUtility.GetMaxValue(Events.Select(k => k.StartTime));

        public override float MinEndTime =>
            NumericUtility.GetMinValue(Events.Select(k => k.EndTime));

        public override int MaxTimeCount => Events.Count(k => k.EndTime.Equals(MaxTime));

        public override int MinTimeCount => Events.Count(k => k.StartTime.Equals(MaxTime));

        public override async Task WriteScriptAsync(TextWriter sw)
        {
            await sw.WriteLineAsync(Header);
            await ScriptHelper.WriteHostEventsAsync(sw, this, EnableGroupedSerialization);
        }
    }
}