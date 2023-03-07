using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.OsbX.Actions;

namespace Coosu.Storyboard.OsbX;

public static class OsbXExtensions
{
    public static Camera25Object GetOrCreateCamera25Control(this Scene scene)
    {
        var layer = scene.GetOrAddLayer("DefaultCamera25ControlLayer");
        var camera25Object = layer.SceneObjects.OfType<Camera25Object>().FirstOrDefault();
        if (camera25Object != null) return camera25Object;
        camera25Object = new Camera25Object { CameraIdentifier = "default" };
        layer.SceneObjects.Add(camera25Object);
        return camera25Object;
    }

    // MoveZ
    public static void MoveZ(this IEventHost host, double startTime, double z) =>
        host.AddEvent(LinearEase.Instance, startTime, startTime, z, z);
    public static void MoveZ(this IEventHost host, double startTime, double endTime, double z) =>
        host.AddEvent(LinearEase.Instance, startTime, endTime, z, z);
    public static void MoveZ(this IEventHost host, double startTime, double endTime, double startZ, double endZ) =>
        host.AddEvent(LinearEase.Instance, startTime, endTime, startZ, endZ);
    public static void MoveZ(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startZ, double endZ) =>
        host.AddEvent(easing, startTime, endTime, startZ, endZ);

    private static void AddEvent(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime,
        double x1, double x2)
    {
        var moveZ = new MoveZ(easing, startTime, endTime, new List<double>(2) { x1, x2 });
        host.AddEvent(moveZ);
    }
}