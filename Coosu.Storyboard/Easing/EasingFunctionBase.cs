﻿using System;
using System.Diagnostics;
using static Coosu.Storyboard.EasingType;

namespace Coosu.Storyboard.Easing;

[DebuggerDisplay("{DebuggerDisplay}")]
public abstract class EasingFunctionBase : IEasingFunction
{
    private static readonly Type EasingTypeT = typeof(EasingType);

    public EasingMode EasingMode { get; set; }
    internal bool ThrowIfChangeProperty { get; set; } = false;
    private string DebuggerDisplay => GetDescription();

    public double Ease(double normalizedTime)
    {
        return EasingMode switch
        {
            EasingMode.EaseIn => EaseInCore(normalizedTime),
            EasingMode.EaseOut => 1.0 - EaseInCore(1.0 - normalizedTime),
            _ => normalizedTime >= 0.5
                ? (1.0 - EaseInCore((1.0 - normalizedTime) * 2.0)) * 0.5 + 0.5
                : EaseInCore(normalizedTime * 2.0) * 0.5
        };
    }

    public string GetDescription()
    {
        var s = TryGetEasingType()?.ToString();
        if (s != null) return s;

        var name = GetType().Name;
        return "Variant " + name.Substring(0, name.Length - 4) + EasingMode.ToString().Substring(4);
    }

    public EasingType GetEasingType()
    {
        var easingType = TryGetEasingType();
        if (easingType == null)
        {
            Console.WriteLine("The target easing is not standard storyboard easing, use \"Linear\" instead.");
            return Linear;
        }

        return easingType.Value;
    }

    public abstract EasingType? TryGetEasingType();

    public static implicit operator EasingFunctionBase(EasingType easing)
    {
        var easingFunction = easing.ToEasingFunction();
        return easingFunction;
    }

    public static implicit operator EasingFunctionBase(int easingIndex)
    {
        var index = (byte)easingIndex;
        if (!Enum.IsDefined(EasingTypeT, index))
            throw new ArgumentOutOfRangeException(nameof(easingIndex), easingIndex, null);
        var easing = (EasingType)index;
        var easingFunction = easing.ToEasingFunction();
        return easingFunction;
    }

    protected abstract double EaseInCore(double normalizedTime);
}

public static class EasingFunctionExtensions
{
    public static EasingFunctionBase ToEasingFunction(this EasingType easingType)
    {
#pragma warning disable format
            // @formatter:off
            return easingType switch
            {
                Linear =>            LinearEase.Instance,
                EasingOut =>         QuadraticEase.InstanceOut,
                EasingIn =>          QuadraticEase.InstanceIn,
                QuadIn =>            QuadraticEase.InstanceIn,
                QuadOut =>           QuadraticEase.InstanceOut,
                QuadInOut =>         QuadraticEase.InstanceInOut,
                CubicIn =>           CubicEase.InstanceIn,
                CubicOut =>          CubicEase.InstanceOut,
                CubicInOut =>        CubicEase.InstanceInOut,
                QuartIn =>           QuarticEase.InstanceIn,
                QuartOut =>          QuarticEase.InstanceOut,
                QuartInOut =>        QuarticEase.InstanceInOut,
                QuintIn =>           QuinticEase.InstanceIn,
                QuintOut =>          QuinticEase.InstanceOut,
                QuintInOut =>        QuinticEase.InstanceInOut,
                SineIn =>            SineEase.InstanceIn,
                SineOut =>           SineEase.InstanceOut,
                SineInOut =>         SineEase.InstanceInOut,
                ExpoIn =>            ExponentialEase.InstanceIn,
                ExpoOut =>           ExponentialEase.InstanceOut,
                ExpoInOut =>         ExponentialEase.InstanceInOut,
                CircIn =>            CircleEase.InstanceIn,
                CircOut =>           CircleEase.InstanceOut,
                CircInOut =>         CircleEase.InstanceInOut,
                ElasticIn =>         ElasticEase.InstanceIn,
                ElasticOut =>        ElasticEase.InstanceOut,
                ElasticHalfOut =>    ElasticEase.InstanceHalfOut,
                ElasticQuarterOut => ElasticEase.InstanceQuarterOut,
                ElasticInOut =>      ElasticEase.InstanceInOut,
                BackIn =>            BackEase.InstanceIn,
                BackOut =>           BackEase.InstanceOut,
                BackInOut =>         BackEase.InstanceInOut,
                BounceIn =>          BounceEase.InstanceIn,
                BounceOut =>         BounceEase.InstanceOut,
                BounceInOut =>       BounceEase.InstanceInOut,
                _ =>                 throw new ArgumentOutOfRangeException(nameof(easingType), easingType, null)
            };
        // @formatter:on
#pragma warning restore format
    }
}