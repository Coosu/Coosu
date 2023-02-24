using System;
using System.IO;
using Coosu.Shared;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;

namespace Coosu.Storyboard.OsbX.SubjectHandlers;

public class Camera25Handler : SubjectHandler<Camera25Object>
{
    static Camera25Handler()
    {
        ObjectType.SignType(99, "Camera25");
    }

    public Camera25Handler()
    {
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveXActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveYActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveZActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<RotateActionHandler>());
        RegisterAction(HandlerRegister.GetActionHandlerInstance<OriginActionHandler>());

        RegisterAction(HandlerRegister.GetActionHandlerInstance<LoopActionHandler>());
    }

    public override string Flag => "Camera25";

    // Camera,default
    public override string Serialize(Camera25Object camera25Object)
    {
        var header = $"{Flag},{camera25Object.CameraIdentifier}";
        if (camera25Object.LoopList.Count == 0)
        {
            return header;
        }

        using var sw = new StringWriter();
        sw.WriteLine(header);
        foreach (var loop in camera25Object.LoopList)
        {
            loop.WriteScriptAsync(sw).Wait();
        }

        return sw.ToString().TrimEnd('\r', '\n');
    }

    public override Camera25Object Deserialize(ref ValueListBuilder<string> split)
    {
        if (split.Length < 1) throw new ArgumentOutOfRangeException();

        //var type = ObjectTypeManager.Parse(split[0]);
        string cameraIdentifier = "default";
        if (split.Length >= 2)
        {
            cameraIdentifier = split[1];
        }

        return new Camera25Object { CameraIdentifier = cameraIdentifier };
    }
}