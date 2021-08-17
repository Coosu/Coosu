using System;
using static Coosu.Storyboard.EasingType;

namespace Coosu.Storyboard.Easing
{
    public abstract class EasingFunctionBase : IEasingFunction
    {
        public EasingMode EasingMode { get; init; }

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
            return GetType().Name + EasingMode.ToString().Substring(4);
        }

        protected abstract double EaseInCore(double normalizedTime);
        public abstract EasingType? GetEasingType();

        public static implicit operator EasingFunctionBase(EasingType easingType)
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
}
