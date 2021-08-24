using System.Numerics;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Extensions;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class SpriteEventExtensions
    {
        // Move
        public static void MoveBy(this Sprite sprite,
            double startTime, double endTime,
            Vector2 point) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, point.X, point.Y);
        public static void MoveBy(this Sprite sprite,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, LinearEase.Instance, startTime, endTime, x, y);
        public static void MoveBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            Vector2 point) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, point.X, point.Y);
        public static void MoveBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveBy, easing, startTime, endTime, x, y);

        // Fade
        public static void FadeBy(this Sprite sprite,
            double startTime, double endTime,
            double opacity) =>
            sprite.AddEventRelative(MoreEventTypes.FadeBy, LinearEase.Instance, startTime, endTime, opacity);
        public static void FadeBy(this Sprite sprite
            , EasingFunctionBase easing, 
            double startTime, double endTime,
            double opacity) =>
            sprite.AddEventRelative(MoreEventTypes.FadeBy, easing, startTime, endTime, opacity);

        // Scale
        public static void ScaleBy(this Sprite sprite, 
            double startTime, double endTime,
            double scale) =>
            sprite.AddEventRelative(MoreEventTypes.ScaleBy, LinearEase.Instance, startTime, endTime, scale);
        public static void ScaleBy(this Sprite sprite, 
            EasingFunctionBase easing,
            double startTime, double endTime, 
            double scale) =>
            sprite.AddEventRelative(MoreEventTypes.ScaleBy, easing, startTime, endTime, scale);

        // Rotate
        public static void RotateBy(this Sprite sprite,
            double startTime, double endTime, 
            double rotate) =>
            sprite.AddEventRelative(MoreEventTypes.RotateBy, LinearEase.Instance, startTime, endTime, rotate);
        public static void RotateBy(this Sprite sprite,
            EasingFunctionBase easing, 
            double startTime, double endTime,
            double rotate) =>
            sprite.AddEventRelative(MoreEventTypes.RotateBy, easing, startTime, endTime, rotate);

        // MoveX
        public static void MoveXBy(this Sprite sprite, 
            double startTime, double endTime,
            double x) =>
            sprite.AddEventRelative(MoreEventTypes.MoveXBy, LinearEase.Instance, startTime, endTime, x);
        public static void MoveXBy(this Sprite sprite,
            EasingFunctionBase easing, 
            double startTime, double endTime,
            double x) =>
            sprite.AddEventRelative(MoreEventTypes.MoveXBy, easing, startTime, endTime, x);

        // MoveY
        public static void MoveYBy(this Sprite sprite,
            double startTime, double endTime,
            double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveYBy, LinearEase.Instance, startTime, endTime, y);
        public static void MoveYBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            double y) =>
            sprite.AddEventRelative(MoreEventTypes.MoveYBy, easing, startTime, endTime, y);

        // Color
        public static void ColorBy(this Sprite sprite,
            double startTime, double endTime, 
            Vector3 color) =>
            sprite.AddEventRelative(MoreEventTypes.ColorBy, LinearEase.Instance, startTime, endTime, color.X, color.Y, color.Z);
        public static void ColorBy(this Sprite sprite, 
            double startTime, double endTime,
            byte r, byte g, byte b) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, r, g, b);
        public static void ColorBy(this Sprite sprite,
            EasingFunctionBase easing, 
            double startTime, double endTime,
            Vector3 color) =>
            sprite.AddEventRelative(MoreEventTypes.ColorBy, easing, startTime, endTime, color.X, color.Y, color.Z);
        public static void ColorBy(this Sprite sprite,
            EasingFunctionBase easing, 
            double startTime, double endTime,
            byte r, byte g, byte b) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, r, g, b);

        // Vector
        public static void VectorBy(this Sprite sprite,
            double startTime, double endTime,
            Vector2 vector) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, vector.X, vector.Y);
        public static void VectorBy(this Sprite sprite,
            double startTime, double endTime,
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, LinearEase.Instance, startTime, endTime, x, y);
        public static void VectorBy(this Sprite sprite,
            EasingFunctionBase easing,
            double startTime, double endTime,
            Vector2 vector) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, vector.X, vector.Y);
        public static void VectorBy(this Sprite sprite, 
            EasingFunctionBase easing, 
            double startTime, double endTime, 
            double x, double y) =>
            sprite.AddEventRelative(MoreEventTypes.VectorBy, easing, startTime, endTime, x, y);

        public static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x }));
        }

        public static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y }));
        }

        public static void AddEventRelative(this Sprite sprite, EventType e, EasingFunctionBase easing,
            double startTime, double endTime,
            double x, double y, double z)
        {
            sprite.AddEvent(new RelativeEvent(e, easing, startTime, endTime, new[] { x, y, z }));
        }
    }
}