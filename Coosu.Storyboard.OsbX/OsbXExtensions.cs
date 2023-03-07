using System.Collections.Generic;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.OsbX.Actions;

namespace Coosu.Storyboard.OsbX;

public static class OsbXExtensions
{
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