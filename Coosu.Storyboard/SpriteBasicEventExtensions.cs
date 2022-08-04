using System.Collections.Generic;
using Coosu.Shared.Numerics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard
{
    public static class SpriteBasicEventExtensions
    {
        // Move
        public static void Move(this IEventHost host, double startTime, Vector2D point) =>
            host.AddEvent(EventTypes.Move, LinearEase.Instance, startTime, startTime, point.X, point.Y, point.X, point.Y);
        public static void Move(this IEventHost host, double startTime, double x, double y) =>
            host.AddEvent(EventTypes.Move, LinearEase.Instance, startTime, startTime, x, y, x, y);
        public static void Move(this IEventHost host, double startTime, double endTime, double x, double y) =>
            host.AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, x, y, x, y);
        public static void Move(this IEventHost host, double startTime, double endTime, double x1, double y1, double x2, double y2) =>
            host.AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, x1, y1, x2, y2);
        public static void Move(this IEventHost host, double startTime, double endTime, Vector2D startPoint, Vector2D endPoint) =>
            host.AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public static void Move(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, Vector2D startPoint, Vector2D endPoint) =>
            host.AddEvent(EventTypes.Move, easing, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static void Move(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double x1, double y1, double x2, double y2) =>
            host.AddEvent(EventTypes.Move, easing, startTime, endTime, x1, y1, x2, y2);

        // Fade
        public static void Fade(this IEventHost host, double startTime, double opacity) =>
            host.AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, startTime, opacity, opacity);
        public static void Fade(this IEventHost host, double startTime, double endTime, double opacity) =>
            host.AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, endTime, opacity, opacity);
        public static void Fade(this IEventHost host, double startTime, double endTime, double startOpacity, double endOpacity) =>
            host.AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, endTime, startOpacity, endOpacity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startOpacity"></param>
        /// <param name="endOpacity"></param>
        public static void Fade(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startOpacity, double endOpacity) =>
            host.AddEvent(EventTypes.Fade, easing, startTime, endTime, startOpacity, endOpacity);

        // Scale
        public static void Scale(this IEventHost host, double startTime, double scale) =>
            host.AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, startTime, scale, scale);
        public static void Scale(this IEventHost host, double startTime, double endTime, double scale) =>
            host.AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, endTime, scale, scale);
        public static void Scale(this IEventHost host, double startTime, double endTime, double startScale, double endScale) =>
            host.AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, endTime, startScale, endScale);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startScale"></param>
        /// <param name="endScale"></param>
        public static void Scale(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startScale, double endScale) =>
            host.AddEvent(EventTypes.Scale, easing, startTime, endTime, startScale, endScale);

        // Rotate
        public static void Rotate(this IEventHost host, double startTime, double rotate) =>
            host.AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, startTime, rotate, rotate);
        public static void Rotate(this IEventHost host, double startTime, double endTime, double rotate) =>
            host.AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, endTime, rotate, rotate);
        public static void Rotate(this IEventHost host, double startTime, double endTime, double startRotate, double endRotate) =>
            host.AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, endTime, startRotate, endRotate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startRotate"></param>
        /// <param name="endRotate"></param>
        public static void Rotate(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startRotate, double endRotate) =>
            host.AddEvent(EventTypes.Rotate, easing, startTime, endTime, startRotate, endRotate);

        // MoveX
        public static void MoveX(this IEventHost host, double startTime, double x) =>
            host.AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, startTime, x, x);
        public static void MoveX(this IEventHost host, double startTime, double endTime, double x) =>
            host.AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, endTime, x, x);
        public static void MoveX(this IEventHost host, double startTime, double endTime, double startX, double endX) =>
            host.AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, endTime, startX, endX);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startX"></param>
        /// <param name="endX"></param>
        public static void MoveX(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startX, double endX) =>
            host.AddEvent(EventTypes.MoveX, easing, startTime, endTime, startX, endX);

        // MoveY
        public static void MoveY(this IEventHost host, double startTime, double y) =>
            host.AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, startTime, y, y);
        public static void MoveY(this IEventHost host, double startTime, double endTime, double y) =>
            host.AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, endTime, y, y);
        public static void MoveY(this IEventHost host, double startTime, double endTime, double startY, double endY) =>
            host.AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, endTime, startY, endY);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startY"></param>
        /// <param name="endY"></param>
        public static void MoveY(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startY, double endY) =>
            host.AddEvent(EventTypes.MoveY, easing, startTime, endTime, startY, endY);

        #region Color

        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="color">Fixed rgb value. The range of each value is 0-255.</param>
        public static void Color(this IEventHost host, double startTime, Vector3D color) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, startTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="color">Fixed rgb value. The range of each value is 0-255.</param>
        public static void Color(this IEventHost host, double startTime, double endTime, Vector3D color) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="color1">Start rgb value. The range of each value is 0-255.</param>
        /// <param name="color2">End rgb value. The range of each value is 0-255.</param>
        public static void Color(this IEventHost host, double startTime, double endTime, Vector3D color1, Vector3D color2) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="color1">Start rgb value. The range of each value is 0-255.</param>
        /// <param name="color2">End rgb value. The range of each value is 0-255.</param>
        public static void Color(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, Vector3D color1, Vector3D color2) =>
            host.AddEvent(EventTypes.Color, easing, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="r">Fixed Red value. The range of the value is 0-255.</param>
        /// <param name="g">Fixed Green value. The range of the value is 0-255.</param>
        /// <param name="b">Fixed Blue value. The range of the value is 0-255.</param>
        public static void Color(this IEventHost host, double startTime, int r, int g, int b) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, startTime, r, g, b, r, g, b);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="r">Fixed Red value. The range of the value is 0-255.</param>
        /// <param name="g">Fixed Green value. The range of the value is 0-255.</param>
        /// <param name="b">Fixed Blue value. The range of the value is 0-255.</param>
        public static void Color(this IEventHost host, double startTime, double endTime, int r, int g, int b) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, r, g, b, r, g, b);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startR">Start Red value. The range of the value is 0-255</param>
        /// <param name="startG">Start Green value. The range of the value is 0-255</param>
        /// <param name="startB">Start Blue value. The range of the value is 0-255</param>
        /// <param name="endR">End Red value. The range of the value is 0-255</param>
        /// <param name="endG">End Green value. The range of the value is 0-255</param>
        /// <param name="endB">End Blue value. The range of the value is 0-255</param>
        public static void Color(this IEventHost host, double startTime, double endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            host.AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, startR, startG, startB, endR, endG, endB);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startR">Start Red value. The range of the value is 0-255</param>
        /// <param name="startG">Start Green value. The range of the value is 0-255</param>
        /// <param name="startB">Start Blue value. The range of the value is 0-255</param>
        /// <param name="endR">End Red value. The range of the value is 0-255</param>
        /// <param name="endG">End Green value. The range of the value is 0-255</param>
        /// <param name="endB">End Blue value. The range of the value is 0-255</param>
        public static void Color(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            host.AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startR">Start Red value. The range of the value is 0-255</param>
        /// <param name="startG">Start Green value. The range of the value is 0-255</param>
        /// <param name="startB">Start Blue value. The range of the value is 0-255</param>
        /// <param name="endR">End Red value. The range of the value is 0-255</param>
        /// <param name="endG">End Green value. The range of the value is 0-255</param>
        /// <param name="endB">End Blue value. The range of the value is 0-255</param>
        public static void Color(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double startR, double startG, double startB, double endR, double endG, double endB) =>
            host.AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);


        #endregion

        // Vector
        public static void Vector(this IEventHost host, double startTime, Vector2D vector) =>
            host.AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, startTime, vector.X, vector.Y, vector.X, vector.Y);
        public static void Vector(this IEventHost host, double startTime, double w, double h) =>
            host.AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, startTime, w, h, w, h);
        public static void Vector(this IEventHost host, double startTime, double endTime, double w, double h) =>
            host.AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, w, h, w, h);
        public static void Vector(this IEventHost host, double startTime, double endTime, Vector2D startZoom, Vector2D endZoom) =>
            host.AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startZoom"></param>
        /// <param name="endZoom"></param>
        public static void Vector(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, Vector2D startZoom, Vector2D endZoom) =>
            host.AddEvent(EventTypes.Vector, easing, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public static void Vector(this IEventHost host, double startTime, double endTime, double w1, double h1, double w2, double h2) =>
            host.AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, w1, h1, w2, h2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing">Easing. This parameter can be <see cref="EasingFunctionBase"/>,
        ///     <see cref="EasingType"/> or
        ///     index integer of <see cref="EasingType"/>.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="w1"></param>
        /// <param name="h1"></param>
        /// <param name="w2"></param>
        /// <param name="h2"></param>
        public static void Vector(this IEventHost host, EasingFunctionBase easing, double startTime, double endTime, double w1, double h1, double w2, double h2) =>
            host.AddEvent(EventTypes.Vector, easing, startTime, endTime, w1, h1, w2, h2);

        //Extra
        public static void FlipH(this IEventHost host, double startTime) => host.AddEvent(startTime, startTime, ParameterType.Horizontal);
        public static void FlipH(this IEventHost host, double startTime, double endTime) => host.AddEvent(startTime, endTime, ParameterType.Horizontal);

        public static void FlipV(this IEventHost host, double startTime) => host.AddEvent(startTime, startTime, ParameterType.Vertical);
        public static void FlipV(this IEventHost host, double startTime, double endTime) => host.AddEvent(startTime, endTime, ParameterType.Vertical);

        public static void Additive(this IEventHost host, double startTime) =>
            host.AddEvent(startTime, startTime, ParameterType.Additive);
        public static void Additive(this IEventHost host, double startTime, double endTime) =>
            host.AddEvent(startTime, endTime, ParameterType.Additive);

        public static void Parameter(this IEventHost host, double startTime, double endTime, ParameterType p) =>
            host.AddEvent(startTime, endTime, p);

        private static void AddEvent(this IEventHost host, double startTime, double endTime, ParameterType p)
        {
            host.AddEvent(BasicEvent.Create(EventTypes.Parameter, EasingType.Linear,
                startTime, endTime, new List<double>(1) { (int)p }));
        }

        private static void AddEvent(this IEventHost host, EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double x2)
        {
            host.AddEvent(BasicEvent.Create(e, easing,
                startTime, endTime, new List<double>(2) { x1, x2 }));
        }

        private static void AddEvent(this IEventHost host, EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double y1, double x2, double y2)
        {
            host.AddEvent(BasicEvent.Create(e, easing,
                startTime, endTime, new List<double>(4) { x1, y1, x2, y2 }));
        }

        private static void AddEvent(this IEventHost host, EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double y1, double z1, double x2, double y2, double z2)
        {
            host.AddEvent(BasicEvent.Create(e, easing,
                startTime, endTime, new List<double>(6) { x1, y1, z1, x2, y2, z2 }));
        }
    }
}
