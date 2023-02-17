using System;
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
    }

    public override string Flag => "Camera25";
    // Camera
    public override string Serialize(Camera25Object raw)
    {
        return $"{Flag},{raw.CameraIdentifier}";
    }

    public override Camera25Object Deserialize(string[] split)
    {
        if (split.Length < 1) throw new ArgumentOutOfRangeException();

        //var type = ObjectTypeManager.Parse(split[0]);
        string cameraIdentifier = Guid.Empty.ToString();
        if (split.Length >= 2)
        {
            cameraIdentifier = split[1];
        }

        return new Camera25Object { CameraIdentifier = cameraIdentifier };
    }
}