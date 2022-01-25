using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives an elastic/springy curve
    /// </summary>
    public class ElasticEase : EasingFunctionBase
    {
        private int _oscillations = 3;
        private double _springiness = 3;

        /// <summary>
        /// Specifies the number of oscillations
        /// </summary>
        public int Oscillations
        {
            get => _oscillations;
            set
            {
                if (ThrowIfChangeProperty)
                    throw new NotSupportedException("The preset easing property could not be changed.");
                _oscillations = value;
            }
        }

        /// <summary>
        /// Specifies the amount of springiness
        /// </summary>
        public double Springiness
        {
            get => _springiness;
            set
            {
                if (ThrowIfChangeProperty)
                    throw new NotSupportedException("The preset easing property could not be changed.");
                _springiness = value;
            }
        }

        protected override double EaseInCore(double normalizedTime)
        {
            double oscillations = Math.Max(0.0, (double)Oscillations);
            double springiness = Math.Max(0.0, Springiness);
            double expo;
            if (DoubleUtil.IsZero(springiness))
            {
                expo = normalizedTime;
            }
            else
            {
                expo = (Math.Exp(springiness * normalizedTime) - 1.0) / (Math.Exp(springiness) - 1.0);
            }

            return expo * (Math.Sin((Math.PI * 2.0 * oscillations + Math.PI * 0.5) * normalizedTime));
        }

        public override EasingType? TryGetEasingType()
        {
            if (Oscillations != 3) return null;
            if (EasingMode == EasingMode.EaseOut)
            {
                return Springiness switch
                {
                    3d => EasingType.ElasticOut,
                    3d / 2 => EasingType.ElasticHalfOut,
                    3d / 4 => EasingType.ElasticQuarterOut,
                    _ => null
                };
            }

            if (!Springiness.Equals(3d)) return null;
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.ElasticIn,
                EasingMode.EaseInOut => EasingType.ElasticInOut,
                _ => null
            };
        }

        public static ElasticEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
        public static ElasticEase InstanceQuarterOut { get; } = new() { EasingMode = EasingMode.EaseOut, Springiness = 3d / 4, ThrowIfChangeProperty = true };
        public static ElasticEase InstanceHalfOut { get; } = new() { EasingMode = EasingMode.EaseOut, Springiness = 3d / 2, ThrowIfChangeProperty = true };
        public static ElasticEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
        public static ElasticEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
    }
}