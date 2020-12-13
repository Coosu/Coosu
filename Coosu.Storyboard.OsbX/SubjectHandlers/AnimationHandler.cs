using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;
using System;

namespace Coosu.Storyboard.OsbX.SubjectHandlers
{
    public class AnimationHandler : SubjectHandler<AnimatedElement>
    {
        public AnimationHandler()
        {
            RegisterAction(Register.GetActionHandlerInstance<MoveActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveXActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveYActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<FadeActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<ScaleActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<RotateActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<VectorActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<OriginActionHandler>());
        }

        public override string Flag => "Animation";
        public override string Serialize(AnimatedElement raw)
        {
            return string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7},{8},{9},{10},{11}", Flag, raw.Layer, raw.Origin, raw.ImagePath,
                    raw.DefaultX, raw.DefaultY, raw.FrameCount, raw.FrameDelay, raw.LoopType, raw.CameraId, raw.ZDistance, 0);
        }

        public override AnimatedElement Deserialize(string[] split)
        {
            if (split.Length == 8 || split.Length == 9 || split.Length == 12)
            {
                var type = ElementTypeSign.Parse(split[0]);
                var layerType = (LayerType)Enum.Parse(typeof(LayerType), split[1]);
                var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
                var path = split[3].Trim('\"');
                var defX = float.Parse(split[4]);
                var defY = float.Parse(split[5]);
                var frameCount = int.Parse(split[6]);
                var frameDelay = float.Parse(split[7]);
                var loopType = split.Length == 9
                    ? (LoopType)Enum.Parse(typeof(LoopType), split[8])
                    : LoopType.LoopForever;

                float zDistance = 1;
                int cameraId = 0;
                if (split.Length == 11)
                {
                    cameraId = int.Parse(split[9]);
                    zDistance = int.TryParse(split[10], out var result) ? result : 1;
                    var absolute = int.Parse(split[11]) != 0;
                    if (absolute)
                    {
                        throw new NotImplementedException("Absolute mode currently is not supported.");
                    }
                }

                return new AnimatedElement(type, layerType, origin, path, defX, defY, frameCount, frameDelay, loopType)
                {
                    ZDistance = zDistance,
                    CameraId = cameraId
                };
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}