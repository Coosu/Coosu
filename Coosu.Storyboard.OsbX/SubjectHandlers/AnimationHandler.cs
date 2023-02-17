using System;
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
        RegisterAction(HandlerRegister.GetActionHandlerInstance<OriginActionHandler>());
    }

    public override string Flag => "Animation";
    public override string Serialize(Animation raw)
    {
        return string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7},{8},{9},{10},{11}", Flag, raw.LayerType, raw.OriginType, raw.ImagePath,
            raw.DefaultX, raw.DefaultY, raw.FrameCount, raw.FrameDelay, raw.LoopType, raw.CameraIdentifier, raw.DefaultZ, 0);
    }

    public override Animation Deserialize(string[] split)
    {
        if (split.Length is not (8 or 9 or 11 or 12)) throw new ArgumentOutOfRangeException();

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

        double defaultZ = 1;
        string cameraIdentifier = "default";
        if (split.Length >= 11)
        {
            cameraIdentifier = split[9];
            defaultZ = double.TryParse(split[10], out var result) ? result : 1d;
        }

        if (split.Length >= 12)
        {
            var absolute = int.Parse(split[11]) != 0;
            if (absolute)
            {
                throw new NotImplementedException("Absolute mode currently is not supported.");
            }
        }

        return new Animation(layerType, origin, path, defX, defY, frameCount, frameDelay, loopType)
        {
            DefaultZ = defaultZ,
            CameraIdentifier = cameraIdentifier
        };
    }
}