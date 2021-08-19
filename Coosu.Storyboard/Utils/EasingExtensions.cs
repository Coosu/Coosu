using System;

namespace Coosu.Storyboard.Utils
{
    public static class EasingExtensions
    {
        public static double Reverse(Func<double, double> function, double value) => 1 - function(1 - value);
        public static double ToInOut(Func<double, double> function, double value) => .5 * (value < .5 ? function(2 * value) : (2 - function(2 - 2 * value)));

        public static readonly Func<double, double> Step = x => x >= 1 ? 1 : 0;
        public static readonly Func<double, double> Linear = x => x;

        public static readonly Func<double, double> QuadIn = x => x * x;
        public static readonly Func<double, double> QuadOut = x => Reverse(QuadIn, x);
        public static readonly Func<double, double> QuadInOut = x => ToInOut(QuadIn, x);
        public static readonly Func<double, double> CubicIn = x => x * x * x;
        public static readonly Func<double, double> CubicOut = x => Reverse(CubicIn, x);
        public static readonly Func<double, double> CubicInOut = x => ToInOut(CubicIn, x);
        public static readonly Func<double, double> QuartIn = x => x * x * x * x;
        public static readonly Func<double, double> QuartOut = x => Reverse(QuartIn, x);
        public static readonly Func<double, double> QuartInOut = x => ToInOut(QuartIn, x);
        public static readonly Func<double, double> QuintIn = x => x * x * x * x * x;
        public static readonly Func<double, double> QuintOut = x => Reverse(QuintIn, x);
        public static readonly Func<double, double> QuintInOut = x => ToInOut(QuintIn, x);

        public static readonly Func<double, double> SineIn = x => 1 - Math.Cos(x * Math.PI / 2);
        public static readonly Func<double, double> SineOut = x => Reverse(SineIn, x);
        public static readonly Func<double, double> SineInOut = x => ToInOut(SineIn, x);

        public static readonly Func<double, double> ExpoIn = x => Math.Pow(2, 10 * (x - 1));
        public static readonly Func<double, double> ExpoOut = x => Reverse(ExpoIn, x);
        public static readonly Func<double, double> ExpoInOut = x => ToInOut(ExpoIn, x);

        public static readonly Func<double, double> CircIn = x => 1 - Math.Sqrt(1 - x * x);
        public static readonly Func<double, double> CircOut = x => Reverse(CircIn, x);
        public static readonly Func<double, double> CircInOut = x => ToInOut(CircIn, x);

        public static readonly Func<double, double> BackIn = x => x * x * ((1.70158 + 1) * x - 1.70158);
        public static readonly Func<double, double> BackOut = x => Reverse(BackIn, x);
        public static readonly Func<double, double> BackInOut = x => ToInOut((y) => y * y * ((1.70158 * 1.525 + 1) * y - 1.70158 * 1.525), x);

        public static readonly Func<double, double> BounceIn = x => Reverse(BounceOut, x);
        public static readonly Func<double, double> BounceOut = x => x < 1 / 2.75 ? 7.5625 * x * x : x < 2 / 2.75 ? 7.5625 * (x -= (1.5 / 2.75)) * x + .75 : x < 2.5 / 2.75 ? 7.5625 * (x -= (2.25 / 2.75)) * x + .9375 : 7.5625 * (x -= (2.625 / 2.75)) * x + .984375;
        public static readonly Func<double, double> BounceInOut = x => ToInOut(BounceIn, x);

        public static readonly Func<double, double> ElasticIn = x => Reverse(ElasticOut, x);
        public static readonly Func<double, double> ElasticOut = x => Math.Pow(2, -10 * x) * Math.Sin((x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticOutHalf = x => Math.Pow(2, -10 * x) * Math.Sin((0.5 * x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticOutQuarter = x => Math.Pow(2, -10 * x) * Math.Sin((0.25 * x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticInOut = x => ToInOut(ElasticIn, x);

        public static double Ease(this EasingType easing, double value)
            => easing.ToEasingFunc().Invoke(value);

        public static Func<double, double> ToEasingFunc(this EasingType easing)
        {
            switch (easing)
            {
                default:
                case EasingType.Linear: return Linear;

                case EasingType.EasingIn:
                case EasingType.QuadIn: return QuadIn;
                case EasingType.EasingOut:
                case EasingType.QuadOut: return QuadOut;
                case EasingType.QuadInOut: return QuadInOut;

                case EasingType.CubicIn: return CubicIn;
                case EasingType.CubicOut: return CubicOut;
                case EasingType.CubicInOut: return CubicInOut;
                case EasingType.QuartIn: return QuartIn;
                case EasingType.QuartOut: return QuartOut;
                case EasingType.QuartInOut: return QuartInOut;
                case EasingType.QuintIn: return QuintIn;
                case EasingType.QuintOut: return QuintOut;
                case EasingType.QuintInOut: return QuintInOut;

                case EasingType.SineIn: return SineIn;
                case EasingType.SineOut: return SineOut;
                case EasingType.SineInOut: return SineInOut;
                case EasingType.ExpoIn: return ExpoIn;
                case EasingType.ExpoOut: return ExpoOut;
                case EasingType.ExpoInOut: return ExpoInOut;
                case EasingType.CircIn: return CircIn;
                case EasingType.CircOut: return CircOut;
                case EasingType.CircInOut: return CircInOut;
                case EasingType.ElasticIn: return ElasticIn;
                case EasingType.ElasticOut: return ElasticOut;
                case EasingType.ElasticHalfOut: return ElasticOutHalf;
                case EasingType.ElasticQuarterOut: return ElasticOutQuarter;
                case EasingType.ElasticInOut: return ElasticInOut;
                case EasingType.BackIn: return BackIn;
                case EasingType.BackOut: return BackOut;
                case EasingType.BackInOut: return BackInOut;
                case EasingType.BounceIn: return BounceIn;
                case EasingType.BounceOut: return BounceOut;
                case EasingType.BounceInOut: return BounceInOut;
            }
        }
    }
}
