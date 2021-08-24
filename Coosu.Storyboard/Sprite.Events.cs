using System.Numerics;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard
{
    partial class Sprite
    {
        // Move
        public void Move(double startTime, Vector2 point) =>
            AddEvent(EventTypes.Move, LinearEase.Instance, startTime, startTime, point.X, point.Y, point.X, point.Y);
        public void Move(double startTime, double x, double y) =>
            AddEvent(EventTypes.Move, LinearEase.Instance, startTime, startTime, x, y, x, y);
        public void Move(double startTime, double endTime, double x, double y) =>
            AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, x, y, x, y);
        public void Move(double startTime, double endTime, double x1, double y1, double x2, double y2) =>
            AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, x1, y1, x2, y2);
        public void Move(double startTime, double endTime, Vector2 startPoint, Vector2 endPoint) =>
            AddEvent(EventTypes.Move, LinearEase.Instance, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
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
        public void Move(EasingFunctionBase easing, double startTime, double endTime, Vector2 startPoint, Vector2 endPoint) =>
               AddEvent(EventTypes.Move, easing, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
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
        public void Move(EasingFunctionBase easing, double startTime, double endTime, double x1, double y1, double x2, double y2) =>
            AddEvent(EventTypes.Move, easing, startTime, endTime, x1, y1, x2, y2);

        // Fade
        public void Fade(double startTime, double opacity) =>
            AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, startTime, opacity, opacity);
        public void Fade(double startTime, double endTime, double opacity) =>
            AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, endTime, opacity, opacity);
        public void Fade(double startTime, double endTime, double startOpacity, double endOpacity) =>
            AddEvent(EventTypes.Fade, LinearEase.Instance, startTime, endTime, startOpacity, endOpacity);
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
        public void Fade(EasingFunctionBase easing, double startTime, double endTime, double startOpacity, double endOpacity) =>
               AddEvent(EventTypes.Fade, easing, startTime, endTime, startOpacity, endOpacity);

        // Scale
        public void Scale(double startTime, double scale) =>
            AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, startTime, scale, scale);
        public void Scale(double startTime, double endTime, double scale) =>
            AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, endTime, scale, scale);
        public void Scale(double startTime, double endTime, double startScale, double endScale) =>
            AddEvent(EventTypes.Scale, LinearEase.Instance, startTime, endTime, startScale, endScale);
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
        public void Scale(EasingFunctionBase easing, double startTime, double endTime, double startScale, double endScale) =>
                  AddEvent(EventTypes.Scale, easing, startTime, endTime, startScale, endScale);

        // Rotate
        public void Rotate(double startTime, double rotate) =>
            AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, startTime, rotate, rotate);
        public void Rotate(double startTime, double endTime, double rotate) =>
            AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, endTime, rotate, rotate);
        public void Rotate(double startTime, double endTime, double startRotate, double endRotate) =>
            AddEvent(EventTypes.Rotate, LinearEase.Instance, startTime, endTime, startRotate, endRotate);
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
        public void Rotate(EasingFunctionBase easing, double startTime, double endTime, double startRotate, double endRotate) =>
            AddEvent(EventTypes.Rotate, easing, startTime, endTime, startRotate, endRotate);

        // MoveX
        public void MoveX(double startTime, double x) =>
            AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, startTime, x, x);
        public void MoveX(double startTime, double endTime, double x) =>
            AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, endTime, x, x);
        public void MoveX(double startTime, double endTime, double startX, double endX) =>
            AddEvent(EventTypes.MoveX, LinearEase.Instance, startTime, endTime, startX, endX);
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
        public void MoveX(EasingFunctionBase easing, double startTime, double endTime, double startX, double endX) =>
            AddEvent(EventTypes.MoveX, easing, startTime, endTime, startX, endX);

        // MoveY
        public void MoveY(double startTime, double y) =>
            AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, startTime, y, y);
        public void MoveY(double startTime, double endTime, double y) =>
            AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, endTime, y, y);
        public void MoveY(double startTime, double endTime, double startY, double endY) =>
            AddEvent(EventTypes.MoveY, LinearEase.Instance, startTime, endTime, startY, endY);
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
        public void MoveY(EasingFunctionBase easing, double startTime, double endTime, double startY, double endY) =>
            AddEvent(EventTypes.MoveY, easing, startTime, endTime, startY, endY);

        #region Color

        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="color">Fixed rgb value. The range of each value is 0-255.</param>
        public void Color(double startTime, Vector3 color) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, startTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="color">Fixed rgb value. The range of each value is 0-255.</param>
        public void Color(double startTime, double endTime, Vector3 color) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="color1">Start rgb value. The range of each value is 0-255.</param>
        /// <param name="color2">End rgb value. The range of each value is 0-255.</param>
        public void Color(double startTime, double endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
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
        public void Color(EasingFunctionBase easing, double startTime, double endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="r">Fixed Red value. The range of the value is 0-255.</param>
        /// <param name="g">Fixed Green value. The range of the value is 0-255.</param>
        /// <param name="b">Fixed Blue value. The range of the value is 0-255.</param>
        public void Color(double startTime, int r, int g, int b) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, startTime, r, g, b, r, g, b);
        /// <summary>
        /// Color by rgb values.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="r">Fixed Red value. The range of the value is 0-255.</param>
        /// <param name="g">Fixed Green value. The range of the value is 0-255.</param>
        /// <param name="b">Fixed Blue value. The range of the value is 0-255.</param>
        public void Color(double startTime, double endTime, int r, int g, int b) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, r, g, b, r, g, b);
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
        public void Color(double startTime, double endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventTypes.Color, LinearEase.Instance, startTime, endTime, startR, startG, startB, endR, endG, endB);
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
        public void Color(EasingFunctionBase easing, double startTime, double endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);
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
        public void Color(EasingFunctionBase easing, double startTime, double endTime, double startR, double startG, double startB, double endR, double endG, double endB) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);


        #endregion

        // Vector
        public void Vector(double startTime, Vector2 vector) =>
            AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, startTime, vector.X, vector.Y, vector.X, vector.Y);
        public void Vector(double startTime, double w, double h) =>
            AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, startTime, w, h, w, h);
        public void Vector(double startTime, double endTime, double w, double h) =>
            AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, w, h, w, h);
        public void Vector(double startTime, double endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
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
        public void Vector(EasingFunctionBase easing, double startTime, double endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventTypes.Vector, easing, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public void Vector(double startTime, double endTime, double w1, double h1, double w2, double h2) =>
            AddEvent(EventTypes.Vector, LinearEase.Instance, startTime, endTime, w1, h1, w2, h2);
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
        public void Vector(EasingFunctionBase easing, double startTime, double endTime, double w1, double h1, double w2, double h2) =>
            AddEvent(EventTypes.Vector, easing, startTime, endTime, w1, h1, w2, h2);

        //Extra
        public void FlipH(double startTime) => AddEvent(startTime, startTime, ParameterType.Horizontal);
        public void FlipH(double startTime, double endTime) => AddEvent(startTime, endTime, ParameterType.Horizontal);

        public void FlipV(double startTime) => AddEvent(startTime, startTime, ParameterType.Vertical);
        public void FlipV(double startTime, double endTime) => AddEvent(startTime, endTime, ParameterType.Vertical);

        public void Additive(double startTime) =>
            AddEvent(startTime, startTime, ParameterType.Additive);
        public void Additive(double startTime, double endTime) =>
            AddEvent(startTime, endTime, ParameterType.Additive);

        public void Parameter(double startTime, double endTime, ParameterType p) =>
            AddEvent(startTime, endTime, p);

        private void AddEvent(double startTime, double endTime, ParameterType p)
        {
            AddEvent(CommonEvent.Create(EventTypes.Parameter, EasingType.Linear, startTime, endTime,
                new[] { (double)(int)p }, new[] { (double)(int)p }));
        }

        private void AddEvent(EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double x2)
        {
            AddEvent(CommonEvent.Create(e, easing, startTime, endTime, new[] { x1 }, new[] { x2 }));
        }

        private void AddEvent(EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double y1, double x2, double y2)
        {
            AddEvent(CommonEvent.Create(e, easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 }));
        }

        private void AddEvent(EventType e, EasingFunctionBase easing, double startTime, double endTime,
            double x1, double y1, double z1, double x2, double y2, double z2)
        {
            AddEvent(CommonEvent.Create(e, easing, startTime, endTime, new[] { x1, y1, z1 }, new[] { x2, y2, z2 }));
        }
    }
}
