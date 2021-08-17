using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a circular curve toward the destination.
    /// </summary>
    public class CircleEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            normalizedTime = Math.Max(0.0, Math.Min(1.0, normalizedTime));
            return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }

        public override EasingType? TryGetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.CircIn,
                EasingMode.EaseOut => EasingType.CircOut,
                EasingMode.EaseInOut => EasingType.CircInOut,
                _ => null
            };
        }

        public static CircleEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static CircleEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static CircleEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}