namespace Coosu.Storyboard.Easing
{
    public sealed class LinearEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime;
        }

        public override EasingType? TryGetEasingType() => EasingType.Linear;

        public static LinearEase Instance { get; } = new();
    }
}