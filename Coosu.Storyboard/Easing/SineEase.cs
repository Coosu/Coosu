using System;

namespace Coosu.Storyboard.Easing;

/// <summary>
/// This class implements an easing function that gives a Sine curve toward the destination.
/// </summary>
public sealed class SineEase : EasingFunctionBase
{
    protected override double EaseInCore(double normalizedTime)
    {
        return 1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
    }

    public override EasingType? TryGetEasingType()
    {
        return EasingMode switch
        {
            EasingMode.EaseIn => EasingType.SineIn,
            EasingMode.EaseOut => EasingType.SineOut,
            EasingMode.EaseInOut => EasingType.SineInOut,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static SineEase InstanceIn { get; } = new() { EasingMode = EasingMode.EaseIn, ThrowIfChangeProperty = true };
    public static SineEase InstanceOut { get; } = new() { EasingMode = EasingMode.EaseOut, ThrowIfChangeProperty = true };
    public static SineEase InstanceInOut { get; } = new() { EasingMode = EasingMode.EaseInOut, ThrowIfChangeProperty = true };
}