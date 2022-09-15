namespace Coosu.Storyboard.Easing;

/// <summary>
/// This class implements an easing function that gives a cubic curve toward the destination
/// </summary>
public sealed class CubicEase : EasingFunctionBase
{
    protected override double EaseInCore(double normalizedTime)
    {
        return normalizedTime * normalizedTime * normalizedTime;
    }

    public override EasingType? TryGetEasingType()
    {
        return EasingMode switch
        {
            EasingMode.EaseIn => EasingType.CubicIn,
            EasingMode.EaseOut => EasingType.CubicOut,
            EasingMode.EaseInOut => EasingType.CubicInOut,
            _ => null
        };
    }

    public static CubicEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
    public static CubicEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
    public static CubicEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
}