namespace Coosu.Storyboard.Easing
{
    /// <summary>
    /// This class implements an easing function that gives a quartic curve toward the destination
    /// </summary>
    public class QuarticEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }

        public override EasingType? TryGetEasingType()
        {
            return EasingMode switch
            {
                EasingMode.EaseIn => EasingType.QuartIn,
                EasingMode.EaseOut => EasingType.QuartOut,
                EasingMode.EaseInOut => EasingType.QuartInOut,
                _ => null
            };
        }

        public static QuarticEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
        public static QuarticEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
        public static QuarticEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
    }
}