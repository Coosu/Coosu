namespace Coosu.Storyboard.Easing;

/// <summary>
/// This class implements an easing function that gives a quadratic curve toward the destination
/// </summary>
public sealed class QuadraticEase : EasingFunctionBase
{
    protected override double EaseInCore(double normalizedTime)
    {
        return normalizedTime * normalizedTime;
    }

    public override EasingType? TryGetEasingType()
    {
        return EasingMode switch
        {
            EasingMode.EaseIn => EasingType.EasingIn,
            EasingMode.EaseOut => EasingType.EasingOut,
            EasingMode.EaseInOut => EasingType.QuadInOut,
            _ => null
        };
    }

    public static QuadraticEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
    public static QuadraticEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
    public static QuadraticEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
}