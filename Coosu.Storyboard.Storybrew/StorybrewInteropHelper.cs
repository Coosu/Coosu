using System;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using StorybrewCommon.Storyboarding;
using ParameterType = Coosu.Storyboard.Events.ParameterType;

namespace Coosu.Storyboard.Storybrew;

public static class StorybrewInteropHelper
{
    public static void ExecuteEvent(IKeyEvent e, OsbSprite brewObj)
    {
        var easing = ConvertEasing(e.Easing.GetEasingType());
        if (e.EventType == EventTypes.Scale)
            brewObj.Scale(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1));
        else if (e.EventType == EventTypes.Fade)
            brewObj.Fade(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1));
        else if (e.EventType == EventTypes.Rotate)
            brewObj.Rotate(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1));
        else if (e.EventType == EventTypes.MoveX)
            brewObj.MoveX(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1));
        else if (e.EventType == EventTypes.MoveY)
            brewObj.MoveY(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1));
        else if (e.EventType == EventTypes.Move)
            brewObj.Move(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1), e.GetValue(2), e.GetValue(3));
        else if (e.EventType == EventTypes.Vector)
            brewObj.ScaleVec(easing, e.StartTime, e.EndTime, e.GetValue(0), e.GetValue(1), e.GetValue(2), e.GetValue(3));
        else if (e.EventType == EventTypes.Color)
            brewObj.Color(easing, e.StartTime, e.EndTime,
                e.GetValue(0) / 255d, e.GetValue(1) / 255d, e.GetValue(2) / 255d,
                e.GetValue(3) / 255d, e.GetValue(4) / 255d, e.GetValue(5) / 255d);
        else if (e.EventType == EventTypes.Parameter)
        {
            var type = ((Parameter)e).Type;
            switch (type)
            {
                case ParameterType.Horizontal:
                    if (e.StartTime.Equals(e.EndTime))
                        brewObj.FlipH(e.StartTime);
                    else
                        brewObj.FlipH(e.StartTime, e.EndTime);
                    break;
                case ParameterType.Vertical:
                    if (e.StartTime.Equals(e.EndTime))
                        brewObj.FlipV(e.StartTime);
                    else
                        brewObj.FlipV(e.StartTime, e.EndTime);
                    break;
                case ParameterType.Additive:
                    if (e.StartTime.Equals(e.EndTime))
                        brewObj.Additive(e.StartTime);
                    else
                        brewObj.Additive(e.StartTime, e.EndTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            throw new NotSupportedException($"Unsupported converting for Coosu's event \"{e.EventType.Flag}\".");
        }
    }

    public static OsbOrigin ConvertOrigin(OriginType coosuOrigin)
    {
        return coosuOrigin switch
        {
            OriginType.BottomCentre => OsbOrigin.BottomCentre,
            OriginType.BottomLeft => OsbOrigin.BottomLeft,
            OriginType.BottomRight => OsbOrigin.BottomRight,
            OriginType.Centre => OsbOrigin.Centre,
            OriginType.CentreLeft => OsbOrigin.CentreLeft,
            OriginType.CentreRight => OsbOrigin.CentreRight,
            OriginType.TopCentre => OsbOrigin.TopCentre,
            OriginType.TopLeft => OsbOrigin.TopLeft,
            OriginType.TopRight => OsbOrigin.TopRight,
            _ => throw new NotSupportedException($"Unsupported converting for Coosu's origin \"{coosuOrigin}\".")
        };
    }

    public static OsbLoopType ConvertLoopType(LoopType coosuLoop)
    {
        return coosuLoop switch
        {
            LoopType.LoopForever => OsbLoopType.LoopForever,
            LoopType.LoopOnce => OsbLoopType.LoopOnce,
            _ => throw new NotSupportedException($"Unsupported converting for Coosu's loop \"{coosuLoop}\".")
        };
    }

    public static OsbEasing ConvertEasing(EasingType coosuEasing)
    {
        return coosuEasing switch
        {
            EasingType.Linear => OsbEasing.None,
            EasingType.EasingOut => OsbEasing.Out,
            EasingType.EasingIn => OsbEasing.In,
            EasingType.QuadIn => OsbEasing.InQuad,
            EasingType.QuadOut => OsbEasing.OutQuad,
            EasingType.QuadInOut => OsbEasing.InOutQuad,
            EasingType.CubicIn => OsbEasing.InCubic,
            EasingType.CubicOut => OsbEasing.OutCubic,
            EasingType.CubicInOut => OsbEasing.InOutCubic,
            EasingType.QuartIn => OsbEasing.InQuart,
            EasingType.QuartOut => OsbEasing.OutQuart,
            EasingType.QuartInOut => OsbEasing.InOutQuart,
            EasingType.QuintIn => OsbEasing.InQuint,
            EasingType.QuintOut => OsbEasing.OutQuint,
            EasingType.QuintInOut => OsbEasing.InOutQuart,
            EasingType.SineIn => OsbEasing.InSine,
            EasingType.SineOut => OsbEasing.OutSine,
            EasingType.SineInOut => OsbEasing.InOutSine,
            EasingType.ExpoIn => OsbEasing.InExpo,
            EasingType.ExpoOut => OsbEasing.OutExpo,
            EasingType.ExpoInOut => OsbEasing.InOutExpo,
            EasingType.CircIn => OsbEasing.InCirc,
            EasingType.CircOut => OsbEasing.OutCirc,
            EasingType.CircInOut => OsbEasing.InOutCirc,
            EasingType.ElasticIn => OsbEasing.InElastic,
            EasingType.ElasticOut => OsbEasing.OutElastic,
            EasingType.ElasticHalfOut => OsbEasing.OutElasticHalf,
            EasingType.ElasticQuarterOut => OsbEasing.OutElasticQuarter,
            EasingType.ElasticInOut => OsbEasing.InOutElastic,
            EasingType.BackIn => OsbEasing.InBack,
            EasingType.BackOut => OsbEasing.OutBack,
            EasingType.BackInOut => OsbEasing.InOutBack,
            EasingType.BounceIn => OsbEasing.InBounce,
            EasingType.BounceOut => OsbEasing.OutBounce,
            EasingType.BounceInOut => OsbEasing.InOutBounce,
            _ => throw new NotSupportedException($"Unsupported converting for Coosu's easing \"{coosuEasing}\".")
        };
    }
}