using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a quartic curve toward the destination
    /// </summary>
    public class QuarticEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }

        public override EasingType? GetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.QuartIn,
                EasingMode.EaseOut => EasingType.QuartOut,
                EasingMode.EaseInOut => EasingType.QuartInOut,
                _ => null
            };
        }

        public static QuarticEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static QuarticEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static QuarticEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}