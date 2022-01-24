using System;

namespace Coosu.Storyboard
{
    public static class EnumExtensions
    {
        public static LoopType ToLoopType(this ReadOnlySpan<char> loopType)
        {
#if NET6_0_OR_GREATER
            return Enum.Parse<LoopType>(loopType);
#else
            var t = loopType.ToString();
            return t switch
            {
                "LoopForever" => LoopType.LoopForever,
                "LoopOnce" => LoopType.LoopOnce,
                _ => throw new ArgumentOutOfRangeException(nameof(loopType), loopType.ToString(), null)
            };
#endif
        }

        public static LayerType ToLayerType(this ReadOnlySpan<char> layerType)
        {
#if NET6_0_OR_GREATER
            return Enum.Parse<LayerType>(layerType);
#else
            var t = layerType.ToString();
            return t switch
            {
                "Background" => LayerType.Background,
                "Fail" => LayerType.Fail,
                "Pass" => LayerType.Pass,
                "Foreground" => LayerType.Foreground,
                "Overlay" => LayerType.Overlay,
                _ => throw new ArgumentOutOfRangeException(nameof(layerType), layerType.ToString(), null)
            };
#endif
        }

        public static OriginType ToOriginType(this ReadOnlySpan<char> originType)
        {
#if NET6_0_OR_GREATER
            return Enum.Parse<OriginType>(originType);
#else
            var t = originType.ToString();
            return t switch
            {
                "TopLeft" => OriginType.TopLeft,
                "TopCentre" => OriginType.TopCentre,
                "TopRight" => OriginType.TopRight,
                "CentreLeft" => OriginType.CentreLeft,
                "Centre" => OriginType.Centre,
                "CentreRight" => OriginType.CentreRight,
                "BottomLeft" => OriginType.BottomLeft,
                "BottomCentre" => OriginType.BottomCentre,
                "BottomRight" => OriginType.BottomRight,
                "Custom" => OriginType.Custom,
                _ => throw new ArgumentOutOfRangeException(nameof(originType), originType.ToString(), null)
            };
#endif
        }
    }
}