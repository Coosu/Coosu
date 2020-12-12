using Coosu.Osbx.ActionHandlers;
using Coosu.Storyboard;
using Coosu.Storyboard.Parsing;
using System;

namespace Coosu.Osbx.SubjectHandlers
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
            return string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7},{8}", Flag, raw.Layer, raw.Origin, raw.ImagePath,
                    raw.DefaultX, raw.DefaultY, raw.FrameCount, raw.FrameDelay, raw.LoopType);
        }

        public override AnimatedElement Deserialize(string[] split)
        {
            if (split.Length == 8 || split.Length == 9)
            {
                var type = ElementTypeSign.Parse(split[0]);
                var zIndex = int.TryParse(split[1], out var result) ? result : 1;
                var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
                var path = split[3].Trim('\"');
                var defX = float.Parse(split[4]);
                var defY = float.Parse(split[5]);
                var frameCount = int.Parse(split[6]);
                var frameDelay = float.Parse(split[7]);
                var loopType = split.Length == 9
                    ? (LoopType)Enum.Parse(typeof(LoopType), split[8])
                    : LoopType.LoopForever;
                return new AnimatedElement(type, LayerType.Foreground, origin, path, defX, defY, frameCount, frameDelay, loopType)
                {
                    ZIndex = zIndex
                };
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}