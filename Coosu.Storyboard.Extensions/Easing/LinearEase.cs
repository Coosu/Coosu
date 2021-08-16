namespace Coosu.Storyboard.Extensions.Easing
{
    public class LinearEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime;
        }
    }
}