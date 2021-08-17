using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a quadratic curve toward the destination
    /// </summary>
    public class QuadraticEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime;
        }

        public override EasingType? GetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.QuadIn,
                EasingMode.EaseOut => EasingType.QuadOut,
                EasingMode.EaseInOut => EasingType.QuadInOut,
                _ => null
            };
        }

        public static QuadraticEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static QuadraticEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static QuadraticEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}