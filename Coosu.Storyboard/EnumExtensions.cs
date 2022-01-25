using System;

namespace Coosu.Storyboard
{
    public static class EnumExtensions
    {
        private const string S_LoopForever = "LoopForever";
        private const string S_LoopOnce = "LoopOnce";

        public static LoopType ToLoopType(this ReadOnlySpan<char> loopType)
        {
//#if NET6_0_OR_GREATER
//            return Enum.Parse<LoopType>(loopType);
//#else
            var t = loopType/*.ToString()*/;
            if (t.SequenceEqual(S_LoopForever.AsSpan()))
                return LoopType.LoopForever;
            if (t.SequenceEqual(S_LoopOnce.AsSpan()))
                return LoopType.LoopOnce;
            throw new ArgumentOutOfRangeException(nameof(loopType), loopType.ToString(), null);
//#endif
        }

        private const string S_Background = "Background";
        private const string S_Fail = "Fail";
        private const string S_Pass = "Pass";
        private const string S_Foreground = "Foreground";
        private const string S_Overlay = "Overlay";

        public static LayerType ToLayerType(this ReadOnlySpan<char> layerType)
        {
//#if NET6_0_OR_GREATER
//            return Enum.Parse<LayerType>(layerType);
//#else
            var t = layerType/*.ToString()*/;
            if (t.SequenceEqual(S_Background.AsSpan()))
                return LayerType.Background;
            if (t.SequenceEqual(S_Fail.AsSpan()))
                return LayerType.Fail;
            if (t.SequenceEqual(S_Pass.AsSpan()))
                return LayerType.Pass;
            if (t.SequenceEqual(S_Foreground.AsSpan()))
                return LayerType.Foreground;
            if (t.SequenceEqual(S_Overlay.AsSpan()))
                return LayerType.Overlay;
            throw new ArgumentOutOfRangeException(nameof(layerType), layerType.ToString(), null);
//#endif
        }

        private const string S_TopLeft = "TopLeft";
        private const string S_TopCentre = "TopCentre";
        private const string S_TopRight = "TopRight";
        private const string S_CentreLeft = "CentreLeft";
        private const string S_Centre = "Centre";
        private const string S_CentreRight = "CentreRight";
        private const string S_BottomLeft = "BottomLeft";
        private const string S_BottomCentre = "BottomCentre";
        private const string S_BottomRight = "BottomRight";
        private const string S_Custom = "Custom";

        public static OriginType ToOriginType(this ReadOnlySpan<char> originType)
        {
//#if NET6_0_OR_GREATER
//            return Enum.Parse<OriginType>(originType);
//#else
            var t = originType/*.ToString()*/;
            if (t.SequenceEqual(S_TopLeft.AsSpan()))
                return OriginType.TopLeft;
            if (t.SequenceEqual(S_TopCentre.AsSpan()))
                return OriginType.TopCentre;
            if (t.SequenceEqual(S_TopRight.AsSpan()))
                return OriginType.TopRight;
            if (t.SequenceEqual(S_CentreLeft.AsSpan()))
                return OriginType.CentreLeft;
            if (t.SequenceEqual(S_Centre.AsSpan()))
                return OriginType.Centre;
            if (t.SequenceEqual(S_CentreRight.AsSpan()))
                return OriginType.CentreRight;
            if (t.SequenceEqual(S_BottomLeft.AsSpan()))
                return OriginType.BottomLeft;
            if (t.SequenceEqual(S_BottomCentre.AsSpan()))
                return OriginType.BottomCentre;
            if (t.SequenceEqual(S_BottomRight.AsSpan()))
                return OriginType.BottomRight;
            if (t.SequenceEqual(S_Custom.AsSpan()))
                return OriginType.Custom;
            throw new ArgumentOutOfRangeException(nameof(originType), originType.ToString(), null);
//#endif
        }
    }
}