using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Utils;
using Coosu.Shared;

namespace CoosuUnitTest.Beatmap;

internal static class SliderDiscreteSamplingLegacy
{
    internal static SliderTick[] ComputeDiscreteData(ExtendedSliderInfo sliderInfo, double intervalMilliseconds)
    {
        switch (sliderInfo.SliderType)
        {
            case SliderType.Bezier:
            case SliderType.Linear:
                return ComputeBezierDiscreteData(sliderInfo, intervalMilliseconds);
            case SliderType.Perfect:
                return ComputePerfectDiscreteData(sliderInfo, intervalMilliseconds);
            default:
                return EmptyArray<SliderTick>.Value;
        }
    }

    private static SliderTick[] ComputePerfectDiscreteData(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        if (sliderInfo.ControlPoints.Count < 2)
        {
            sliderInfo.SliderType = SliderType.Linear;
            return ComputeBezierDiscreteData(sliderInfo, fixedInterval);
        }

        var p1 = sliderInfo.StartPoint;
        var p2 = sliderInfo.ControlPoints[0];
        var p3 = sliderInfo.ControlPoints[1];

        var circle = SliderDiscreteSamplingShared.GetCircle(p1, p2, p3);

        var radStart = Math.Atan2(p1.Y - circle.p.Y, p1.X - circle.p.X);
        var radMid = Math.Atan2(p2.Y - circle.p.Y, p2.X - circle.p.X);
        var radEnd = Math.Atan2(p3.Y - circle.p.Y, p3.X - circle.p.X);
        if (radMid - radStart > Math.PI)
        {
            radMid -= Math.PI * 2;
        }
        else if (radMid - radStart < -Math.PI)
        {
            radMid += Math.PI * 2;
        }

        if (radEnd - radMid > Math.PI)
        {
            radEnd -= Math.PI * 2;
        }
        else if (radEnd - radMid < -Math.PI)
        {
            radEnd += Math.PI * 2;
        }

        var ticks = new List<SliderTick>();

        for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
        {
            var offset = i * fixedInterval;
            var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
            if (isOnEdges) continue;
            var ratio = offset / sliderInfo.CurrentSingleDuration;
            var relativeRad = (radEnd - radStart) * ratio;
            var offsetRad = radStart + relativeRad;
            var x = circle.p.X + circle.r * Math.Cos(offsetRad);
            var y = circle.p.Y + circle.r * Math.Sin(offsetRad);

            ticks.Add(new SliderTick(sliderInfo.StartTime + offset, new Vector2((float)x, (float)y)));
        }

        if (sliderInfo.Repeat > 1)
        {
            var firstSingleCopy = ticks.ToArray();
            for (int i = 2; i <= sliderInfo.Repeat; i++)
            {
                var reverse = i % 2 == 0;
                if (reverse)
                {
                    ticks.AddRange(firstSingleCopy.Reverse().Select(k =>
                        new SliderTick(
                            (sliderInfo.CurrentSingleDuration - (k.Offset - sliderInfo.StartTime)) +
                            (i - 1) * sliderInfo.CurrentSingleDuration + sliderInfo.StartTime,
                            k.Point)));
                }
                else
                {
                    ticks.AddRange(firstSingleCopy.Select(k =>
                        new SliderTick(k.Offset + (i - 1) * sliderInfo.CurrentSingleDuration, k.Point)));
                }
            }
        }

        return ticks.ToArray();
    }

    private static SliderTick[] ComputeBezierDiscreteData(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var groupedPoints = SliderDiscreteSamplingShared.GetGroupedPoints(sliderInfo);
        var groupedBezierLengths = SliderDiscreteSamplingShared.GetGroupedBezierLengths(groupedPoints);
        var totalLength = groupedBezierLengths.Sum();
        var ticks = new List<SliderTick>();

        for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
        {
            var offset = i * fixedInterval;
            var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
            if (isOnEdges) continue;

            var ratio = offset / sliderInfo.CurrentSingleDuration;
            var relativeLen = totalLength * ratio;

            var (index, lenInPart) = SliderDiscreteSamplingShared.CalculateWhichPart(groupedBezierLengths, relativeLen);
            var len = groupedBezierLengths[index];
            var tickPoint = BezierHelper.Compute(groupedPoints[index], (float)(lenInPart / len));
            ticks.Add(new SliderTick(sliderInfo.StartTime + offset, tickPoint));
        }

        if (sliderInfo.Repeat > 1)
        {
            Span<SliderTick> span = stackalloc SliderTick[ticks.Count];
            for (var i = 0; i < ticks.Count; i++)
            {
                span[i] = ticks[i];
            }

            for (int i = 2; i <= sliderInfo.Repeat; i++)
            {
                span.Reverse();
                var reverse = i % 2 == 0;
                if (reverse)
                {
                    foreach (var baseTick in span)
                    {
                        var tick = new SliderTick(
                            i * sliderInfo.CurrentSingleDuration - baseTick.Offset + sliderInfo.StartTime * 2,
                            baseTick.Point);
                        ticks.Add(tick);
                    }
                }
                else
                {
                    foreach (var baseTick in span)
                    {
                        var tick = new SliderTick(baseTick.Offset + (i - 1) * sliderInfo.CurrentSingleDuration, baseTick.Point);
                        ticks.Add(tick);
                    }
                }
            }
        }

        return ticks.ToArray();
    }
}