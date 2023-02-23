using System;
using Coosu.Shared;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;

namespace Coosu.Storyboard.OsbX.SubjectHandlers;

public class AnimationHandler : SubjectHandler<Animation>
{
    public AnimationHandler()
    {
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveXActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveYActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveZActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<FadeActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<ScaleActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<RotateActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<VectorActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<ColorActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<OriginActionHandler>());
    }

    public override string Flag => "Animation";
    public override string Serialize(Animation raw)
    {
        return string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7},{8},{9},{10}", Flag, raw.LayerType, raw.OriginType, raw.ImagePath,
            raw.DefaultX, raw.DefaultY, raw.FrameCount, raw.FrameDelay, raw.LoopType, raw.DefaultZ, raw.CameraIdentifier);
    }

    public override Animation Deserialize(ref ValueListBuilder<string> split)
    {
        if (split.Length is not (8 or 9 or 11)) throw new ArgumentOutOfRangeException();

        var type = ObjectType.Parse(split[0]);
        var layerType = (LayerType)Enum.Parse(typeof(LayerType), split[1]);
        var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
        var path = split[3].Trim('\"');
        var defX = double.Parse(split[4]);
        var defY = double.Parse(split[5]);
        var frameCount = int.Parse(split[6]);
        var frameDelay = double.Parse(split[7]);
        var loopType = split.Length == 9
            ? (LoopType)Enum.Parse(typeof(LoopType), split[8])
            : LoopType.LoopForever;

        var defaultZ = 1d;
        if (split.Length >= 10)
        {
            defaultZ = double.TryParse(split[9], out var result) ? result : 1d;
        }

        var cameraIdentifier = "default";
        if (split.Length >= 11)
        {
            cameraIdentifier = split[10];
        }

        return new Animation(layerType, origin, path, defX, defY, frameCount, frameDelay, loopType)
        {
            DefaultZ = defaultZ,
            CameraIdentifier = cameraIdentifier
        };
    }
}