using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a cubic curve toward the destination
    /// </summary>
    public class CubicEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime;
        }

        public override EasingType? TryGetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.CubicIn,
                EasingMode.EaseOut => EasingType.CubicOut,
                EasingMode.EaseInOut => EasingType.CubicInOut,
                _ => null
            };
        }

        public static CubicEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static CubicEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static CubicEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}