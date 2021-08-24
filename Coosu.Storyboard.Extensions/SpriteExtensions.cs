using System.Numerics;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    /// <summary>
    /// Extension methods for <see cref="Sprite"/>.
    /// This includes relative events.
    /// </summary>
    public static class SpriteEventExtensions
    {
        #region Move

        /// <summary>
        /// Move by relative displacement.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="point">Relative displacement.</param>
        public static void MoveBy(this Sprite sprite,
            double startTime, double endTime,
            Vector2 point) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, point.X, point.Y);

        /// <summary>
        /// Move by relative displacement.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        /// <param name="y">Relative y.</param>
        public static void MoveBy(this Sprite sprite,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, x, y);

        /// <summary>
        /// Move by relative displacement.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="point">Relative displacement.</param>
        public static void MoveBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            Vector2 point) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, point.X, point.Y);

        /// <summary>
        /// Move by relative displacement.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        /// <param name="y">Relative y.</param>
        public static void MoveBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, x, y);

        #endregion

        #region Fade

        /// <summary>
        /// Fade by relative opacity.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="opacity">Relative opacity.</param>
        public static void FadeBy(this Sprite sprite,
            double startTime, double endTime,
            double opacity) =>
            sprite.AddEventRelative(MoreEventTypes.FadeBy, LinearEase.Instance, startTime, endTime, opacity);

        /// <summary>
        /// Fade by relative opacity.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="opacity">Relative opacity.</param>
        public static void FadeBy(this Sprite sprite
            , EasingFunctionBase easing,
            double startTime, double endTime,
            double opacity) =>
            sprite.AddEventRelative(MoreEventTypes.FadeBy, easing, startTime, endTime, opacity);

        #endregion

        #region Scale

        /// <summary>
        /// Scale by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="scale">Relative size.</param>
        public static void ScaleBy(this Sprite sprite,
            double startTime, double endTime,
            double scale) =>
            sprite.AddEventRelative(MoreEventTypes.ScaleBy, LinearEase.Instance, startTime, endTime, scale);

        /// <summary>
        /// Scale by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="scale">Relative size.</param>
        public static void ScaleBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double scale) =>
            sprite.AddEventRelative(MoreEventTypes.ScaleBy, easing, startTime, endTime, scale);

        #endregion

        #region Rotate

        /// <summary>
        /// Rotate by relative rotation.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="rotate">Relative rotation.</param>
        public static void RotateBy(this Sprite sprite,
            double startTime, double endTime,
            double rotate) =>
            sprite.AddEventRelative(MoreEventTypes.RotateBy, LinearEase.Instance, startTime, endTime, rotate);

        /// <summary>
        /// Rotate by relative rotation.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="rotate">Relative rotation.</param>
        public static void RotateBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double rotate) =>
            sprite.AddEventRelative(MoreEventTypes.RotateBy, easing, startTime, endTime, rotate);


        #endregion

        #region MoveX

        /// <summary>
        /// Move by relative x.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        public static void MoveXBy(this Sprite sprite,
            double startTime, double endTime,
            double x) =>
            sprite.AddEventRelative(MoreEventTypes.MoveXBy, LinearEase.Instance, startTime, endTime, x);

        /// <summary>
        /// Move by relative x.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        public static void MoveXBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double x) =>
            sprite.AddEventRelative(MoreEventTypes.MoveXBy, easing, startTime, endTime, x);

        #endregion

        #region MoveY

        /// <summary>
        /// Move by relative y.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="y">Relative y.</param>
        public static void MoveYBy(this Sprite sprite,
            double startTime, double endTime,
            double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveYBy, LinearEase.Instance, startTime, endTime, y);

        /// <summary>
        /// Move by relative y.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="y">Relative y.</param>
        public static void MoveYBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveYBy, easing, startTime, endTime, y);

        #endregion

        #region Color

        /// <summary>
        /// Color by relative rgb values.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="color">Relative color. The range of each value is 0-255.</param>
        public static void ColorBy(this Sprite sprite,
            double startTime, double endTime,
            Vector3 color) =>
            sprite.AddEventRelative(MoreEventTypes.ColorBy, LinearEase.Instance, startTime, endTime, color.X, color.Y,
                color.Z);
        /// <summary>
        /// Color by relative rgb values.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="r">Relative R. The range of the value is 0-255.</param>
        /// <param name="g">Relative G. The range of the value is 0-255.</param>
        /// <param name="b">Relative B. The range of the value is 0-255.</param>
        public static void ColorBy(this Sprite sprite,
            double startTime, double endTime,
            byte r, byte g, byte b) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, r, g, b);

        /// <summary>
        /// Color by relative rgb values.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="color">Relative color. The range of each value is 0-255.</param>
        public static void ColorBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            Vector3 color) =>
            sprite.AddEventRelative(MoreEventTypes.ColorBy, easing, startTime, endTime, color.X, color.Y, color.Z);

        /// <summary>
        /// Color by relative rgb values.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="r">Relative R. The range of the value is 0-255.</param>
        /// <param name="g">Relative G. The range of the value is 0-255.</param>
        /// <param name="b">Relative B. The range of the value is 0-255.</param>
        public static void ColorBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            byte r, byte g, byte b) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, r, g, b);

        #endregion

        /// <summary>
        /// Vector by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="vector">Relative vector.</param>
        public static void VectorBy(this Sprite sprite,
            double startTime, double endTime,
            Vector2 vector) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, vector.X,
                vector.Y);

        /// <summary>
        /// Vector by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        /// <param name="y">Relative y.</param>
        public static void VectorBy(this Sprite sprite,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, x, y);

        /// <summary>
        /// Vector by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="vector">Relative vector.</param>
        public static void VectorBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            Vector2 vector) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, vector.X, vector.Y);

        /// <summary>
        /// Vector by relative size.
        /// </summary>
        /// <param name="sprite">Specific sprite.</param>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime">Event start time.</param>
        /// <param name="endTime">Event end time.</param>
        /// <param name="x">Relative x.</param>
        /// <param name="y">Relative y.</param>
        public static void VectorBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, x, y);

        private static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x }));
        }

        private static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y }));
        }

        private static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y, double z)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y, z }));
        }
    }
}