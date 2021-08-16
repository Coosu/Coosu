using System;

namespace Coosu.Storyboard.Extensions.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a Sine curve toward the destination.
    /// </summary>
    public class SineEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return 1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
        }
    }
}