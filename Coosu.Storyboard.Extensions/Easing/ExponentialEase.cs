using System;

namespace Coosu.Storyboard.Extensions.Easing
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
    }
}