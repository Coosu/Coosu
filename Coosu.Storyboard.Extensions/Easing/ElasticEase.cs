using System;

namespace Coosu.Storyboard.Extensions.Easing
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
        public int Oscillations { get; set; } = 3;

        /// <summary>
        /// Specifies the amount of springiness
        /// </summary>
        public double Springiness { get; set; } = 3;

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
    }
}