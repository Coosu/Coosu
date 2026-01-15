using System;
using System.Collections.Generic;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Shared;

namespace CoosuUnitTest.Beatmap;

internal static class SliderDiscreteSamplingReference
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

        return SliderDiscreteSamplingLegacy.ComputeDiscreteData(sliderInfo, fixedInterval);
    }

    private static SliderTick[] ComputeBezierDiscreteData(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var polyline = SliderDiscreteSamplingShared.CreateHighResolutionBezierPolyline(sliderInfo, 2048);
        if (polyline.Count < 2)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var (segmentLengths, cumulativeLengths, totalLength) = SliderDiscreteSamplingShared.CreateCumulativeLengths(polyline);
        if (totalLength <= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var ticks = new List<SliderTick>();

        for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
        {
            var offset = i * fixedInterval;
            var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
            if (isOnEdges) continue;

            var ratio = offset / sliderInfo.CurrentSingleDuration;
            var relativeLen = totalLength * ratio;

            var tickPoint = SliderDiscreteSamplingShared.SamplePointAtLength(polyline, segmentLengths, cumulativeLengths, relativeLen);
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