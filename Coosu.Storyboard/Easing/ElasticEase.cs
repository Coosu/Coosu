using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives an elastic/springy curve
    /// </summary>
    public class ElasticEase : EasingFunctionBase
    {
        public ElasticEase()
        {
        }

        /// <summary>
        /// Specifies the number of oscillations
        /// </summary>
        public int Oscillations { get; init; } = 3;

        /// <summary>
        /// Specifies the amount of springiness
        /// </summary>
        public double Springiness { get; init; } = 3;

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

        public override EasingType? GetEasingType()
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

        public static ElasticEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static ElasticEase InstanceQuarterOut => new() { EasingMode = EasingMode.EaseOut, Springiness = 3d / 4 };
        public static ElasticEase InstanceHalfOut => new() { EasingMode = EasingMode.EaseOut, Springiness = 3d / 2 };
        public static ElasticEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static ElasticEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}