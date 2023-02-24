using System;
using System.IO;
using Coosu.Shared;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;

namespace Coosu.Storyboard.OsbX.SubjectHandlers;

public class SpriteHandler : SubjectHandler<Sprite>
{
    public SpriteHandler()
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
        RegisterAction(HandlerRegister.GetActionHandlerInstance<ParameterActionHandler>());

        RegisterAction(HandlerRegister.GetActionHandlerInstance<LoopActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<TriggerActionHandler>());
    }

    public override string Flag => "Sprite";

    //Sprite,0,Centre,"",320,240
    public override string Serialize(Sprite sprite)
    {
        var header = $"{Flag},{sprite.LayerType},{sprite.OriginType},\"{sprite.ImagePath}\",{sprite.DefaultX},{sprite.DefaultY},{sprite.DefaultZ},{sprite.CameraIdentifier}";
        if (sprite.LoopList.Count == 0 && sprite.TriggerList.Count == 0)
        {
            return header;
        }

        using var sw = new StringWriter();
        sw.WriteLine(header);
        foreach (var loop in sprite.LoopList)
        {
            loop.WriteScriptAsync(sw).Wait();
        }

        foreach (var trigger in sprite.TriggerList)
        {
            trigger.WriteScriptAsync(sw).Wait();
        }

        return sw.ToString().TrimEnd('\r', '\n');
    }

    public override Sprite Deserialize(ref ValueListBuilder<string> split)
    {
        if (split.Length is not (6 or 8)) throw new ArgumentOutOfRangeException();

        var type = ObjectType.Parse(split[0]);
        var layerType = (LayerType)Enum.Parse(typeof(LayerType), split[1]);
        var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
        var path = split[3].Trim('\"');
        var defX = double.Parse(split[4]);
        var defY = double.Parse(split[5]);

        var defaultZ = 1d;
        if (split.Length >= 7)
        {
            defaultZ = double.TryParse(split[6], out var result) ? result : 1d;
        }

        var cameraIdentifier = "default";
        if (split.Length >= 8)
        {
            cameraIdentifier = split[7];
        }

        return new Sprite(layerType, origin, path, defX, defY)
        {
            DefaultZ = defaultZ,
            CameraIdentifier = cameraIdentifier
        };
    }
}