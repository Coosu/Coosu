using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a Sine curve toward the destination.
    /// </summary>
    public class SineEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return 1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
        }

        public override EasingType? GetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.SineIn,
                EasingMode.EaseOut => EasingType.SineOut,
                EasingMode.EaseInOut => EasingType.SineInOut,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static SineEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static SineEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static SineEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}