using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives an exponential curve
    /// </summary>
    public sealed class ExponentialEase : EasingFunctionBase
    {
        private double _exponent = 2;

        /// <summary>
        /// Specifies the factor which controls the shape of easing.
        /// </summary>
        public double Exponent
        {
            get => _exponent;
            set
            {
                if (ThrowIfChangeProperty)
                    throw new NotSupportedException("The preset easing property could not be changed.");
                _exponent = value;
            }
        }

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

        public static ExponentialEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
        public static ExponentialEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
        public static ExponentialEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
    }
}