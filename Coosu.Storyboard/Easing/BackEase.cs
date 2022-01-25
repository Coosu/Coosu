using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that backs up before going to the destination.
    /// </summary>
    public class BackEase : EasingFunctionBase
    {
        private double _amplitude = 1;

        /// <summary>
        /// Specifies how much the function will pull back
        /// </summary>
        public double Amplitude
        {
            get => _amplitude;
            set
            {
                if (ThrowIfChangeProperty)
                    throw new NotSupportedException("The preset easing property could not be changed.");
                _amplitude = value;
            }
        }

        protected override double EaseInCore(double normalizedTime)
        {
            double amp = Math.Max(0.0, Amplitude);
            return Math.Pow(normalizedTime, 3.0) - normalizedTime * amp * Math.Sin(Math.PI * normalizedTime);
        }

        public override EasingType? TryGetEasingType()
        {
            if (!Amplitude.Equals(1)) return null;

            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.BackIn,
                EasingMode.EaseOut => EasingType.BackOut,
                EasingMode.EaseInOut => EasingType.BackInOut,
                _ => null
            };
        }

        public static BackEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
        public static BackEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
        public static BackEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
    }
}