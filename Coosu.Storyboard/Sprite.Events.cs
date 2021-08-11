using System.Numerics;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard
{
    partial class Sprite
    {
        // Move
        public void Move(int startTime, Vector2 point) =>
            AddEvent(EventTypes.Move, 0, startTime, startTime, point.X, point.Y, point.X, point.Y);
        public void Move(int startTime, float x, float y) =>
            AddEvent(EventTypes.Move, 0, startTime, startTime, x, y, x, y);
        public void Move(int startTime, int endTime, float x, float y) =>
            AddEvent(EventTypes.Move, 0, startTime, endTime, x, y, x, y);
        public void Move(int startTime, int endTime, float x1, float y1, float x2, float y2) =>
            AddEvent(EventTypes.Move, 0, startTime, endTime, x1, y1, x2, y2);
        public void Move(int startTime, int endTime, Vector2 startPoint, Vector2 endPoint) =>
            AddEvent(EventTypes.Move, 0, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        public void Move(EasingType easing, int startTime, int endTime, Vector2 startPoint, Vector2 endPoint) =>
            AddEvent(EventTypes.Move, easing, startTime, endTime, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        public void Move(EasingType easing, int startTime, int endTime, float x1, float y1, float x2, float y2) =>
            AddEvent(EventTypes.Move, easing, startTime, endTime, x1, y1, x2, y2);

        // Fade
        public void Fade(int startTime, float opacity) =>
            AddEvent(EventTypes.Fade, 0, startTime, startTime, opacity, opacity);
        public void Fade(int startTime, int endTime, float opacity) =>
            AddEvent(EventTypes.Fade, 0, startTime, endTime, opacity, opacity);
        public void Fade(int startTime, int endTime, float startOpacity, float endOpacity) =>
            AddEvent(EventTypes.Fade, 0, startTime, endTime, startOpacity, endOpacity);
        public void Fade(EasingType easing, int startTime, int endTime, float startOpacity, float endOpacity) =>
            AddEvent(EventTypes.Fade, easing, startTime, endTime, startOpacity, endOpacity);

        // Scale
        public void Scale(int startTime, float scale) =>
            AddEvent(EventTypes.Scale, 0, startTime, startTime, scale, scale);
        public void Scale(int startTime, int endTime, float scale) =>
            AddEvent(EventTypes.Scale, 0, startTime, endTime, scale, scale);
        public void Scale(int startTime, int endTime, float startScale, float endScale) =>
            AddEvent(EventTypes.Scale, 0, startTime, endTime, startScale, endScale);
        public void Scale(EasingType easing, int startTime, int endTime, float startScale, float endScale) =>
            AddEvent(EventTypes.Scale, easing, startTime, endTime, startScale, endScale);

        // Rotate
        public void Rotate(int startTime, float rotate) =>
            AddEvent(EventTypes.Rotate, 0, startTime, startTime, rotate, rotate);
        public void Rotate(int startTime, int endTime, float rotate) =>
            AddEvent(EventTypes.Rotate, 0, startTime, endTime, rotate, rotate);
        public void Rotate(int startTime, int endTime, float startRotate, float endRotate) =>
            AddEvent(EventTypes.Rotate, 0, startTime, endTime, startRotate, endRotate);
        public void Rotate(EasingType easing, int startTime, int endTime, float startRotate, float endRotate) =>
            AddEvent(EventTypes.Rotate, easing, startTime, endTime, startRotate, endRotate);

        // MoveX
        public void MoveX(int startTime, float x) =>
            AddEvent(EventTypes.MoveX, 0, startTime, startTime, x, x);
        public void MoveX(int startTime, int endTime, float x) =>
            AddEvent(EventTypes.MoveX, 0, startTime, endTime, x, x);
        public void MoveX(int startTime, int endTime, float startX, float endX) =>
            AddEvent(EventTypes.MoveX, 0, startTime, endTime, startX, endX);
        public void MoveX(EasingType easing, int startTime, int endTime, float startX, float endX) =>
            AddEvent(EventTypes.MoveX, easing, startTime, endTime, startX, endX);

        // MoveY
        public void MoveY(int startTime, float y) =>
            AddEvent(EventTypes.MoveY, 0, startTime, startTime, y, y);
        public void MoveY(int startTime, int endTime, float y) =>
            AddEvent(EventTypes.MoveY, 0, startTime, endTime, y, y);
        public void MoveY(int startTime, int endTime, float startY, float endY) =>
            AddEvent(EventTypes.MoveY, 0, startTime, endTime, startY, endY);
        public void MoveY(EasingType easing, int startTime, int endTime, float startY, float endY) =>
            AddEvent(EventTypes.MoveY, easing, startTime, endTime, startY, endY);

        // Color
        public void Color(int startTime, Vector3 color) =>
            AddEvent(EventTypes.Color, 0, startTime, startTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        public void Color(int startTime, int endTime, Vector3 color) =>
            AddEvent(EventTypes.Color, 0, startTime, endTime, color.X, color.Y, color.Z, color.X, color.Y, color.Z);
        public void Color(int startTime, int endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventTypes.Color, 0, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        public void Color(EasingType easing, int startTime, int endTime, Vector3 color1, Vector3 color2) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, color1.X, color1.Y, color1.Z, color2.X, color2.Y, color2.Z);
        public void Color(int startTime, int r, int g, int b) =>
            AddEvent(EventTypes.Color, 0, startTime, startTime, r, g, b, r, g, b);
        public void Color(int startTime, int endTime, int r, int g, int b) =>
            AddEvent(EventTypes.Color, 0, startTime, endTime, r, g, b, r, g, b);
        public void Color(int startTime, int endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventTypes.Color, 0, startTime, endTime, startR, startG, startB, endR, endG, endB);
        public void Color(EasingType easing, int startTime, int endTime, int startR, int startG, int startB, int endR, int endG, int endB) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);
        public void Color(EasingType easing, int startTime, int endTime, float startR, float startG, float startB, float endR, float endG, float endB) =>
            AddEvent(EventTypes.Color, easing, startTime, endTime, startR, startG, startB, endR, endG, endB);

        // Vector
        public void Vector(int startTime, Vector2 vector) =>
            AddEvent(EventTypes.Vector, 0, startTime, startTime, vector.X, vector.Y, vector.X, vector.Y);
        public void Vector(int startTime, float w, float h) =>
            AddEvent(EventTypes.Vector, 0, startTime, startTime, w, h, w, h);
        public void Vector(int startTime, int endTime, float w, float h) =>
            AddEvent(EventTypes.Vector, 0, startTime, endTime, w, h, w, h);
        public void Vector(int startTime, int endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventTypes.Vector, 0, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public void Vector(EasingType easing, int startTime, int endTime, Vector2 startZoom, Vector2 endZoom) =>
            AddEvent(EventTypes.Vector, easing, startTime, endTime, startZoom.X, startZoom.Y, endZoom.X, endZoom.Y);
        public void Vector(int startTime, int endTime, float w1, float h1, float w2, float h2) =>
            AddEvent(EventTypes.Vector, 0, startTime, endTime, w1, h1, w2, h2);
        public void Vector(EasingType easing, int startTime, int endTime, float w1, float h1, float w2, float h2) =>
            AddEvent(EventTypes.Vector, easing, startTime, endTime, w1, h1, w2, h2);

        //Extra
        public void FlipH(int startTime) => AddEvent(0, startTime, startTime, ParameterType.Horizontal);
        public void FlipH(int startTime, int endTime) => AddEvent(0, startTime, endTime, ParameterType.Horizontal);

        public void FlipV(int startTime) => AddEvent(0, startTime, startTime, ParameterType.Vertical);
        public void FlipV(int startTime, int endTime) => AddEvent(0, startTime, endTime, ParameterType.Vertical);

        public void Additive(int startTime) =>
            AddEvent(0, startTime, startTime, ParameterType.Additive);
        public void Additive(int startTime, int endTime) =>
            AddEvent(0, startTime, endTime, ParameterType.Additive);

        public void Parameter(EasingType easing, int startTime, int endTime, ParameterType p) =>
            AddEvent(easing, startTime, endTime, p);

        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime,
            float x1, float x2)
        {
            AddEvent(e, easing, startTime, endTime, new[] { x1 }, new[] { x2 });
        }

        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime,
            float x1, float y1, float x2, float y2)
        {
            AddEvent(e, easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 });
        }

        public void AddEvent(EventType e, EasingType easing, float startTime, float endTime,
            float x1, float y1, float z1, float x2, float y2, float z2)
        {
            AddEvent(e, easing, startTime, endTime, new[] { x1, y1, z1 }, new[] { x2, y2, z2 });
        }

        public void AddEvent(EasingType easing, float startTime, float endTime, ParameterType p)
        {
            AddEvent(EventTypes.Parameter, easing, startTime, endTime,
                new[] { (float)(int)p }, new[] { (float)(int)p });
        }

        internal override void AddEvent(EventType e, EasingType easing, float startTime, float endTime, float[] start, float[]? end)
        {
            if (_isLooping)
                LoopList[LoopList.Count - 1].AddEvent(e, easing, startTime, endTime, start, end);
            else if (_isTriggering)
                TriggerList[TriggerList.Count - 1].AddEvent(e, easing, startTime, endTime, start, end);
            else
                base.AddEvent(e, easing, startTime, endTime, start, end);
        }
    }
}
