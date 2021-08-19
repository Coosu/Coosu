using System;

namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that backs up before going to the destination.
    /// </summary>
    public class BackEase : EasingFunctionBase
    {
        public BackEase()
        {
        }

        /// <summary>
        /// Specifies how much the function will pull back
        /// </summary>
        public double Amplitude { get; init; } = 1;

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

        public static BackEase InstanceIn => new() { EasingMode = EasingMode.EaseIn };
        public static BackEase InstanceOut => new() { EasingMode = EasingMode.EaseOut };
        public static BackEase InstanceInOut => new() { EasingMode = EasingMode.EaseInOut };
    }
}