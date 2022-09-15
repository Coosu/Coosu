namespace Coosu.Storyboard.Easing;

/// <summary>
/// This class implements an easing function that gives a quintic curve toward the destination
/// </summary>
public sealed class QuinticEase : EasingFunctionBase
{
    protected override double EaseInCore(double normalizedTime)
    {
        return normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
    }

    public override EasingType? TryGetEasingType()
    {
        return EasingMode switch
        {
            EasingMode.EaseIn => EasingType.QuintIn,
            EasingMode.EaseOut => EasingType.QuintOut,
            EasingMode.EaseInOut => EasingType.QuintInOut,
            _ => null
        };
    }

    public static QuinticEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
    public static QuinticEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
    public static QuinticEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
}