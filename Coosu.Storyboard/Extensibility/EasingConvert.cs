using System;

namespace Coosu.Storyboard.Extensibility
{
    public class EasingConvert
    {
        public static EasingType ToEasing(string s)
        {
            var easing = int.Parse(s);
            if (easing is > 34 or < 0)
                throw new FormatException("Unknown easing");
            return (EasingType)easing;
        }
    }
}