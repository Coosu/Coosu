using System.Numerics;
using Coosu.Storyboard.Extensions.Easing;

namespace Coosu.Storyboard.Extensions
{
    public static class SpriteEventExtensions
    {
        // Move
        public static void MoveBy(this Sprite sprite, int startTime, int endTime, Vector2 point) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, new LinearEase(), startTime, endTime, point.X, point.Y);
        public static void MoveBy(this Sprite sprite, int startTime, int endTime, double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, new LinearEase(), startTime, endTime, x, y);
        // Fade
        public static void FadeBy(this Sprite sprite, int startTime, int endTime, double opacity) =>
            sprite.AddEventRelative(MoreEventTypes.FadeBy, new LinearEase(), startTime, endTime, opacity);
        // Scale
        public static void ScaleBy(this Sprite sprite, int startTime, int endTime, double scale) =>
            sprite.AddEventRelative(MoreEventTypes.ScaleBy, new LinearEase(), startTime, endTime, scale);
        // Rotate
        public static void RotateBy(this Sprite sprite, int startTime, int endTime, double rotate) =>
            sprite.AddEventRelative(MoreEventTypes.RotateBy, new LinearEase(), startTime, endTime, rotate);
        // MoveX
        public static void MoveXBy(this Sprite sprite, int startTime, int endTime, double x) =>
            sprite.AddEventRelative(MoreEventTypes.MoveXBy, new LinearEase(), startTime, endTime, x);
        // MoveY
        public static void MoveYBy(this Sprite sprite, int startTime, int endTime, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveYBy, new LinearEase(), startTime, endTime, y);
        // Color
        public static void ColorBy(this Sprite sprite, int startTime, int endTime, Vector3 color) =>
            sprite.AddEventRelative(MoreEventTypes.ColorBy, new LinearEase(), startTime, endTime, color.X, color.Y, color.Z);
        // Vector
        public static void VectorBy(this Sprite sprite, int startTime, int endTime, Vector2 vector) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, new LinearEase(), startTime, endTime, vector.X, vector.Y);

        public static void AddEventRelative(this Sprite sprite, EventType e, IEasingFunction easing,
            double startTime, double endTime,
            double x)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x }));
        }

        public static void AddEventRelative(this Sprite sprite, EventType e, IEasingFunction easing,
            double startTime, double endTime,
            double x, double y)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y }));
        }

        public static void AddEventRelative(this Sprite sprite, EventType e, IEasingFunction easing,
            double startTime, double endTime,
            double x, double y, double z)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y, z }));
        }
    }
}