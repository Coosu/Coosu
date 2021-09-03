using System;

namespace Coosu.Storyboard
{
    public static class Anchors
    {
        public static Anchor<double> TopLeft => new(0, 0);
        public static Anchor<double> TopCenter => new(0.5, 0);
        public static Anchor<double> TopRight => new(1, 0);
        public static Anchor<double> CenterLeft => new(0, 0.5);
        public static Anchor<double> Center => new(0.5, 0.5);
        public static Anchor<double> CenterRight => new(1, 0.5);
        public static Anchor<double> BottomLeft => new(0, 1);
        public static Anchor<double> BottomCenter => new(0.5, 1);
        public static Anchor<double> BottomRight => new(1, 1);

        public static Anchor<double> FromOriginType(OriginType originType)
        {
            return originType switch
            {
                OriginType.TopLeft => TopLeft,
                OriginType.TopCentre => TopCenter,
                OriginType.TopRight => TopRight,
                OriginType.CentreLeft => CenterLeft,
                OriginType.Centre => Center,
                OriginType.CentreRight => CenterRight,
                OriginType.BottomLeft => BottomLeft,
                OriginType.BottomCentre => BottomCenter,
                OriginType.BottomRight => BottomRight,
                OriginType.Custom => TopLeft,
                _ => throw new ArgumentOutOfRangeException(nameof(originType), originType, null)
            };
        }
    }
}
