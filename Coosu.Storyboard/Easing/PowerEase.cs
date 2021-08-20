using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a polynomial curve of arbitrary degree.
    /// If the curve you desire is cubic, quadratic, quartic, or quintic it is better to use the 
    /// specialized easing functions.
    /// </summary>
    public class PowerEase : EasingFunctionBase
    {
        private double _power = 2;

        public PowerEase()
        {
        }

        /// <summary>
        /// Specifies the power for the polynomial equation.
        /// </summary>
        public double Power
        {
            get => _power;
            set
            {
                if (ThrowIfChangeProperty)
                    throw new NotSupportedException("The preset easing property could not be changed.");
                _power = value;
            }
        }


        protected override double EaseInCore(double normalizedTime)
        {
            double power = Math.Max(0.0, Power);
            return Math.Pow(normalizedTime, power);
        }

        public override EasingType? TryGetEasingType()
        {
            return null;
        }

        public static PowerEase InstanceIn => new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
        public static PowerEase InstanceOut => new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
        public static PowerEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
    }
}