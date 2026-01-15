using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Utils;
using Coosu.Shared;
using Xunit;
using Xunit.Abstractions;

namespace CoosuUnitTest.Beatmap;

public sealed class SliderDiscreteSamplingComparisonTest
{
    private readonly ITestOutputHelper _output;

    public SliderDiscreteSamplingComparisonTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CompareLegacyAndCurrentBezierDiscreteSampling()
    {
        var cases = new[]
        {
            ("linear-repeat3", CreateSlider(SliderType.Linear, new Vector2(64, 192), new[]
            {
                new Vector2(448, 192),
            }, repeat: 3)),
            ("bezier-single", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(320, 64),
                new Vector2(448, 192),
            }, repeat: 1)),
            ("bezier-multi-repeat2", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(256, 64),
                new Vector2(256, 64),
                new Vector2(320, 256),
                new Vector2(448, 192),
            }, repeat: 2)),
        };

        const double interval = 50;

        foreach (var (name, slider) in cases)
        {
            var current = slider.ComputeDiscreteData(interval);
            var legacy = LegacyComputeDiscreteData(slider, interval);

            _output.WriteLine($"case={name}");
            _output.WriteLine($"count current={current.Length} legacy={legacy.Length}");

            var maxOffsetDelta = 0d;
            var maxDistanceDelta = 0d;
            var mismatchCount = 0;

            var count = Math.Min(current.Length, legacy.Length);
            for (var i = 0; i < count; i++)
            {
                var offsetDelta = Math.Abs(current[i].Offset - legacy[i].Offset);
                if (offsetDelta > maxOffsetDelta) maxOffsetDelta = offsetDelta;

                var dx = current[i].Point.X - legacy[i].Point.X;
                var dy = current[i].Point.Y - legacy[i].Point.Y;
                var distDelta = Math.Sqrt(dx * dx + dy * dy);
                if (distDelta > maxDistanceDelta) maxDistanceDelta = distDelta;

                if (offsetDelta > 1e-7 || distDelta > 1e-4)
                {
                    mismatchCount++;
                    _output.WriteLine(
                        $"idx={i} off(cur={current[i].Offset:0.###} leg={legacy[i].Offset:0.###} d={offsetDelta:0.########}) " +
                        $"pt(cur={current[i].Point.X:0.###},{current[i].Point.Y:0.###} leg={legacy[i].Point.X:0.###},{legacy[i].Point.Y:0.###} d={distDelta:0.######})");
                }
            }

            _output.WriteLine($"maxOffsetDelta={maxOffsetDelta:0.########} maxDistanceDelta={maxDistanceDelta:0.######} mismatches={mismatchCount}");

            Assert.Equal(legacy.Length, current.Length);
            Assert.True(maxOffsetDelta <= 1e-7, $"Unexpected offset delta in case={name}: {maxOffsetDelta}");
            Assert.True(maxDistanceDelta <= 1e-4, $"Unexpected point delta in case={name}: {maxDistanceDelta}");
        }
    }

    private static ExtendedSliderInfo CreateSlider(SliderType sliderType, Vector2 startPoint, IReadOnlyList<Vector2> controlPoints, int repeat)
    {
        var raw = new RawHitObject
        {
            X = (int)startPoint.X,
            Y = (int)startPoint.Y,
            Offset = 1000,
            RawType = RawObjectType.Slider,
            Hitsound = 0,
            SampleSet = 0,
            AdditionSet = 0,
        };

        var slider = new ExtendedSliderInfo(raw)
        {
            SliderType = sliderType,
            StartPoint = startPoint,
            StartTime = raw.Offset,
            ControlPoints = controlPoints,
            Repeat = repeat,
            PixelLength = 400,
        };

        slider.UpdateComputedValues(lastRedFactor: 500, lastLineMultiple: 1, diffSliderMultiplier: 1, diffTickRate: 1);
        return slider;
    }

    private static SliderTick[] LegacyComputeDiscreteData(ExtendedSliderInfo sliderInfo, double intervalMilliseconds)
    {
        switch (sliderInfo.SliderType)
        {
            case SliderType.Bezier:
            case SliderType.Linear:
                return LegacyComputeBezierDiscreteData(sliderInfo, intervalMilliseconds);
            case SliderType.Perfect:
                return LegacyComputePerfectDiscreteData(sliderInfo, intervalMilliseconds);
            default:
                return EmptyArray<SliderTick>.Value;
        }
    }

    private static SliderTick[] LegacyComputePerfectDiscreteData(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        if (sliderInfo.ControlPoints.Count < 2)
        {
            sliderInfo.SliderType = SliderType.Linear;
            return LegacyComputeBezierDiscreteData(sliderInfo, fixedInterval);
        }

        var p1 = sliderInfo.StartPoint;
        var p2 = sliderInfo.ControlPoints[0];
        var p3 = sliderInfo.ControlPoints[1];

        var circle = LegacyGetCircle(p1, p2, p3);

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

    private static SliderTick[] LegacyComputeBezierDiscreteData(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var groupedPoints = LegacyGetGroupedPoints(sliderInfo);
        var groupedBezierLengths = LegacyGetGroupedBezierLengths(groupedPoints);
        var totalLength = groupedBezierLengths.Sum();
        var ticks = new List<SliderTick>();

        for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
        {
            var offset = i * fixedInterval;
            var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
            if (isOnEdges) continue;

            var ratio = offset / sliderInfo.CurrentSingleDuration;
            var relativeLen = totalLength * ratio;

            var (index, lenInPart) = LegacyCalculateWhichPart(sliderInfo, relativeLen);
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

    private static (int index, double lenInPart) LegacyCalculateWhichPart(SliderInfo sliderInfo, double relativeLen)
    {
        double sum = 0;
        var groupedBezierLengths = LegacyGetGroupedBezierLengths(LegacyGetGroupedPoints(sliderInfo));
        for (var i = 0; i < groupedBezierLengths.Count; i++)
        {
            var len = groupedBezierLengths[i];
            sum += len;
            if (relativeLen < sum) return (i, len - (sum - relativeLen));
        }

        return (-1, -1);
    }

    private static List<Vector2[]> LegacyGetGroupedPoints(SliderInfo sliderInfo)
    {
        IReadOnlyList<Vector2> rawPoints = sliderInfo.ControlPoints;

        var groupedPoints = new List<Vector2[]>();
        var currentGroup = new List<Vector2>();

        Vector2? nextPoint = default;
        for (var i = -1; i < rawPoints.Count - 1; i++)
        {
            var thisPoint = i == -1 ? sliderInfo.StartPoint : rawPoints[i];
            nextPoint = rawPoints[i + 1];
            currentGroup.Add(thisPoint);
            if (thisPoint.Equals(nextPoint))
            {
                if (currentGroup.Count > 1)
                {
                    groupedPoints.Add(currentGroup.ToArray());
                }

                currentGroup = new List<Vector2>();
            }
        }

        if (currentGroup.Count != 0 && nextPoint != null)
        {
            currentGroup.Add(nextPoint.Value);
        }

        if (currentGroup.Count != 0)
        {
            groupedPoints.Add(currentGroup.ToArray());
        }

        if (groupedPoints.Count == 0 && rawPoints.Count != 0)
        {
            currentGroup.AddRange(rawPoints);
        }

        return groupedPoints;
    }

    private static IReadOnlyList<double> LegacyGetGroupedBezierLengths(IReadOnlyList<IReadOnlyList<Vector2>> groupedBezierPoints)
    {
        var array = new double[groupedBezierPoints.Count];
        for (var i = 0; i < groupedBezierPoints.Count; i++)
        {
            var controlPoints = groupedBezierPoints[i];
            var length = BezierHelper.Length(controlPoints);
            array[i] = length;
        }

        return array;
    }

    private static (Vector2 p, double r) LegacyGetCircle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var e = 2 * (p2.X - p1.X);
        var f = 2 * (p2.Y - p1.Y);
        var g = Math.Pow(p2.X, 2) - Math.Pow(p1.X, 2) + Math.Pow(p2.Y, 2) - Math.Pow(p1.Y, 2);
        var a = 2 * (p3.X - p2.X);
        var b = 2 * (p3.Y - p2.Y);
        var c = Math.Pow(p3.X, 2) - Math.Pow(p2.X, 2) + Math.Pow(p3.Y, 2) - Math.Pow(p2.Y, 2);
        var x = (g * b - c * f) / (e * b - a * f);
        var y = (a * g - c * e) / (a * f - b * e);
        var r = Math.Pow(Math.Pow(x - p1.X, 2) + Math.Pow(y - p1.Y, 2), 0.5);
        return (new Vector2((float)x, (float)y), r);
    }
}

