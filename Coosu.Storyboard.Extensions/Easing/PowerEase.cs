using System;

namespace Coosu.Storyboard.Extensions.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a polynomial curve of arbitrary degree.
    /// If the curve you desire is cubic, quadratic, quartic, or quintic it is better to use the 
    /// specialized easing functions.
    /// </summary>
    public class PowerEase : EasingFunctionBase
    {
        public PowerEase()
        {
        }

        /// <summary>
        /// Specifies the power for the polynomial equation.
        /// </summary>
        public double Power { get; set; } = 2;


        protected override double EaseInCore(double normalizedTime)
        {
            double power = Math.Max(0.0, Power);
            return Math.Pow(normalizedTime, power);
        }
    }
}