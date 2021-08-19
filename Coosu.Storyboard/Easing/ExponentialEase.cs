using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives an exponential curve
    /// </summary>
    public class ExponentialEase : EasingFunctionBase
    {
        public ExponentialEase()
        {
        }

        /// <summary>
        /// Specifies the factor which controls the shape of easing.
        /// </summary>
        public double Exponent { get; set; } = 2;

        protected override double EaseInCore(double normalizedTime)
        {
            double factor = Exponent;
            if (DoubleUtil.IsZero(factor))
            {
                return normalizedTime;
            }
            else
            {
                return (Math.Exp(factor * normalizedTime) - 1.0) / (Math.Exp(factor) - 1.0);
            }
        }

        public override EasingType? TryGetEasingType()
        {
            if (!Exponent.Equals(2d)) return null;
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.ExpoIn,
                EasingMode.EaseOut => EasingType.ExpoOut,
                EasingMode.EaseInOut => EasingType.ExpoInOut,
                _ => null
            };
        }

        public static ExponentialEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static ExponentialEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static ExponentialEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}