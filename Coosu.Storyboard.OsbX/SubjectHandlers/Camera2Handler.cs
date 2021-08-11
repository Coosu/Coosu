using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;
using Coosu.Storyboard.Utils;
using System;
﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.OsbX.SubjectHandlers
{
    public class Camera2Handler : SubjectHandler<Camera2Element>
    {
        static Camera2Handler()
        {
            ElementTypeSign.SignType(99, "Camera2");
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
                var type = ElementTypeSign.Parse(split[0]);
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

    public class Camera2Element : EventContainer
    {
        protected override string Header => $"{ElementTypeSign.GetString(Type)},{CameraId}";

        static Camera2Element()
        {
            ElementTypeSign.SignType(99, "Camera2");
        }

        public Camera2Element(ElementType type)
        {
            Type = type;
        }

        public override float MaxTime =>
            NumericUtility.GetMaxValue(EventList.Select(k => k.EndTime));

        public override float MinTime =>
            NumericUtility.GetMinValue(EventList.Select(k => k.StartTime));

        public override float MaxStartTime =>
            NumericUtility.GetMaxValue(EventList.Select(k => k.StartTime));

        public override float MinEndTime =>
            NumericUtility.GetMinValue(EventList.Select(k => k.EndTime));

        public override int MaxTimeCount => EventList.Count(k => k.EndTime.Equals(MaxTime));

        public override int MinTimeCount => EventList.Count(k => k.StartTime.Equals(MaxTime));

        public override void WriteOsbString(TextWriter sw, bool @group = false)
        {
            sw.WriteLine(Head);
            StringUtility.WriteContainerEvents(sw, this, group);
        }
    }
}