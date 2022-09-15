using System.Collections.Generic;
using System.Numerics;
using Coosu.Shared.Numerics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard;

/// <summary>
/// Extension methods for <see cref="host"/>.
/// This includes relative events.
/// </summary>
public static class SpriteRelativeEventExtensions
{
    #region Move
    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="time">Event start time.</param>
    /// <param name="point">Relative displacement.</param>
    public static void MoveBy(this IEventHost host,
        double time,
        Vector2 point) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, time, time, point.X, point.Y);

    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="time">Event start time.</param>
    /// <param name="x">Relative x.</param>
    /// <param name="y">Relative y.</param>
    public static void MoveBy(this IEventHost host,
        double time,
        double x, double y) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, time, time, x, y);

    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="point">Relative displacement.</param>
    public static void MoveBy(this IEventHost host,
        double startTime, double endTime,
        Vector2 point) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, point.X, point.Y);

    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    /// <param name="y">Relative y.</param>
    public static void MoveBy(this IEventHost host,
        double startTime, double endTime,
        double x, double y) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, x, y);

    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="point">Relative displacement.</param>
    public static void MoveBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        Vector2 point) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, point.X, point.Y);

    /// <summary>
    /// Move by relative displacement.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    /// <param name="y">Relative y.</param>
    public static void MoveBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double x, double y) =>
        host.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, x, y);

    #endregion

    #region Fade

    /// <summary>
    /// Fade by relative opacity.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="opacity">Relative opacity.</param>
    public static void FadeBy(this IEventHost host,
        double startTime, double endTime,
        double opacity) =>
        host.AddEventRelative(MoreEventTypes.FadeBy, LinearEase.Instance, startTime, endTime, opacity);

    /// <summary>
    /// Fade by relative opacity.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="opacity">Relative opacity.</param>
    public static void FadeBy(this IEventHost host
        , EasingFunctionBase easing,
        double startTime, double endTime,
        double opacity) =>
        host.AddEventRelative(MoreEventTypes.FadeBy, easing, startTime, endTime, opacity);

    #endregion

    #region Scale

    /// <summary>
    /// Scale by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="scale">Relative size.</param>
    public static void ScaleBy(this IEventHost host,
        double startTime, double endTime,
        double scale) =>
        host.AddEventRelative(MoreEventTypes.ScaleBy, LinearEase.Instance, startTime, endTime, scale);

    /// <summary>
    /// Scale by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="scale">Relative size.</param>
    public static void ScaleBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double scale) =>
        host.AddEventRelative(MoreEventTypes.ScaleBy, easing, startTime, endTime, scale);

    #endregion

    #region Rotate

    /// <summary>
    /// Rotate by relative rotation.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="rotate">Relative rotation.</param>
    public static void RotateBy(this IEventHost host,
        double startTime, double endTime,
        double rotate) =>
        host.AddEventRelative(MoreEventTypes.RotateBy, LinearEase.Instance, startTime, endTime, rotate);

    /// <summary>
    /// Rotate by relative rotation.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="rotate">Relative rotation.</param>
    public static void RotateBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double rotate) =>
        host.AddEventRelative(MoreEventTypes.RotateBy, easing, startTime, endTime, rotate);

    #endregion

    #region MoveX

    /// <summary>
    /// Move by relative x.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    public static void MoveXBy(this IEventHost host,
        double startTime, double endTime,
        double x) =>
        host.AddEventRelative(MoreEventTypes.MoveXBy, LinearEase.Instance, startTime, endTime, x);

    /// <summary>
    /// Move by relative x.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    public static void MoveXBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double x) =>
        host.AddEventRelative(MoreEventTypes.MoveXBy, easing, startTime, endTime, x);

    #endregion

    #region MoveY

    /// <summary>
    /// Move by relative y.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="y">Relative y.</param>
    public static void MoveYBy(this IEventHost host,
        double startTime, double endTime,
        double y) =>
        host.AddEventRelative(MoreEventTypes.MoveYBy, LinearEase.Instance, startTime, endTime, y);

    /// <summary>
    /// Move by relative y.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="y">Relative y.</param>
    public static void MoveYBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double y) =>
        host.AddEventRelative(MoreEventTypes.MoveYBy, easing, startTime, endTime, y);

    #endregion

    #region Color

    /// <summary>
    /// Color by relative rgb values.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="color">Relative color. The range of each value is 0-255.</param>
    public static void ColorBy(this IEventHost host,
        double startTime, double endTime,
        Vector3 color) =>
        host.AddEventRelative(MoreEventTypes.ColorBy, LinearEase.Instance, startTime, endTime, color.X, color.Y,
            color.Z);

    /// <summary>
    /// Color by relative rgb values.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="r">Relative R. The range of the value is 0-255.</param>
    /// <param name="g">Relative G. The range of the value is 0-255.</param>
    /// <param name="b">Relative B. The range of the value is 0-255.</param>
    public static void ColorBy(this IEventHost host,
        double startTime, double endTime,
        byte r, byte g, byte b) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, r, g, b);

    /// <summary>
    /// Color by relative rgb values.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="color">Relative color. The range of each value is 0-255.</param>
    public static void ColorBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        Vector3 color) =>
        host.AddEventRelative(MoreEventTypes.ColorBy, easing, startTime, endTime, color.X, color.Y, color.Z);

    /// <summary>
    /// Color by relative rgb values.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="r">Relative R. The range of the value is 0-255.</param>
    /// <param name="g">Relative G. The range of the value is 0-255.</param>
    /// <param name="b">Relative B. The range of the value is 0-255.</param>
    public static void ColorBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        byte r, byte g, byte b) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, r, g, b);

    #endregion

    #region Vector

    /// <summary>
    /// Vector by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="vector">Relative vector.</param>
    public static void VectorBy(this IEventHost host,
        double startTime, double endTime,
        Vector2 vector) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, vector.X,
            vector.Y);

    /// <summary>
    /// Vector by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    /// <param name="y">Relative y.</param>
    public static void VectorBy(this IEventHost host,
        double startTime, double endTime,
        double x, double y) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, x, y);

    /// <summary>
    /// Vector by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="vector">Relative vector.</param>
    public static void VectorBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        Vector2 vector) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, vector.X, vector.Y);

    /// <summary>
    /// Vector by relative size.
    /// </summary>
    /// <param name="host">Specific host.</param>
    /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
    ///     <see cref="EasingType"/> or
    ///     index integer of <see cref="EasingType"/>.</param>
    /// <param name="startTime">Event start time.</param>
    /// <param name="endTime">Event end time.</param>
    /// <param name="x">Relative x.</param>
    /// <param name="y">Relative y.</param>
    public static void VectorBy(this IEventHost host,
        EasingFunctionBase easing,
        double startTime, double endTime,
        double x, double y) =>
        host.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, x, y);

    #endregion

    private static void AddEventRelative(this IEventHost host, EventType e, EasingFunctionBase easing,
        double startTime, double endTime,
        double x)
    {
        host.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new List<double>(1) { x }));
    }

    private static void AddEventRelative(this IEventHost host, EventType e, EasingFunctionBase easing,
        double startTime, double endTime,
        double x, double y)
    {
        host.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new List<double>(2) { x, y }));
    }

    private static void AddEventRelative(this IEventHost host, EventType e, EasingFunctionBase easing,
        double startTime, double endTime,
        double x, double y, double z)
    {
        host.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new List<double>(3) { x, y, z }));
    }
}