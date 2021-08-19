using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a quintic curve toward the destination
    /// </summary>
    public class QuinticEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }

        public override EasingType? TryGetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.QuintIn,
                EasingMode.EaseOut => EasingType.QuintOut,
                EasingMode.EaseInOut => EasingType.QuintInOut,
                _ => null
            };
        }

        public static QuinticEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static QuinticEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static QuinticEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}