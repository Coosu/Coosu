namespace Coosu.Storyboard.Extensions.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a quintic curve toward the destination
    /// </summary>
    public class QuinticEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }
    }
}