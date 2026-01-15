using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Shared;
using osu.Framework.Utils;

// ReSharper disable once CheckNamespace
namespace Coosu.Beatmap;

public static class SliderExtensions
{
    #region Public Methods

    public static IEnumerable<SliderEdge> EnumerateEdges(this ExtendedSliderInfo sliderInfo)
    {
        for (var i = 0; i < sliderInfo.Repeat + 1; i++)
        {
            yield return new SliderEdge
            (
                offset: sliderInfo.StartTime + sliderInfo.CurrentSingleDuration * i,
                point: i % 2 == 0 ? sliderInfo.StartPoint : sliderInfo.EndPoint,
                edgeHitsound: sliderInfo.EdgeHitsounds?[i] ?? sliderInfo.BaseObject.Hitsound,
                edgeSample: sliderInfo.EdgeSamples?[i] ?? sliderInfo.BaseObject.SampleSet,
                edgeAddition: sliderInfo.EdgeAdditions?[i] ?? sliderInfo.BaseObject.AdditionSet,
                isHitsoundDefined: sliderInfo.EdgeHitsounds == null
            );
        }
    }

    public static SliderEdge[] GetEdges(this ExtendedSliderInfo sliderInfo)
    {
        return EnumerateEdges(sliderInfo).ToArray();
    }

    public static SliderTick[] GetSliderTicks(this ExtendedSliderInfo sliderInfo)
    {
        var tickInterval = sliderInfo.CurrentBeatDuration / sliderInfo.CurrentTickRate;
        return ComputeTicks(sliderInfo, tickInterval);
    }

    public static SliderTick[] GetSliderSlides(this ExtendedSliderInfo sliderInfo)
    {
        // 60fps
        var interval = 1000 / 60d;
        return ComputeTicks(sliderInfo, interval);
    }

    // todo: not cut by rhythm
    public static SliderTick[] ComputeTicks(this ExtendedSliderInfo sliderInfo, double intervalMilliseconds)
    {
        return ComputeTicksByInterval(sliderInfo, intervalMilliseconds);
    }

    #endregion

    #region Private Core Logic

    // todo: not cut by rhythm
    private static SliderTick[] ComputeTicksByInterval(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var approximated = sliderInfo.ComputePathVertices();
        if (approximated.Count < 2)
        {
            return EmptyArray<SliderTick>.Value;
        }

        var (segmentLengths, cumulativeLengths, totalLength) = CreateCumulativeLengths(approximated);
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
            var targetLen = totalLength * ratio;
            var tickPoint = SamplePointAtLength(approximated, segmentLengths, cumulativeLengths, targetLen);
            ticks.Add(new SliderTick(sliderInfo.StartTime + offset, tickPoint));
        }

        if (sliderInfo.Repeat > 1)
        {
            AppendRepeatTicks(sliderInfo, ticks);
        }

        return ticks.ToArray();
    }

    private static void AppendRepeatTicks(ExtendedSliderInfo sliderInfo, List<SliderTick> ticks)
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
                    var tick = new SliderTick(baseTick.Offset + (i - 1) * sliderInfo.CurrentSingleDuration,
                        baseTick.Point);
                    ticks.Add(tick);
                }
            }
        }
    }

    /// <summary>
    /// osu!lazer implementation to compute approximated data.
    /// </summary>
    /// <returns></returns>
    private static IReadOnlyList<Vector2> ComputePathVertices(this SliderInfo sliderInfo)
    {
        IReadOnlyList<Vector2> ticks;
        switch (sliderInfo.SliderType)
        {
            case SliderType.Bezier:
            case SliderType.Linear:
                ticks = ComputePathVerticesBezier(sliderInfo);
                break;
            case SliderType.Perfect:
                ticks = ComputePathVerticesPerfect(sliderInfo);
                break;
#pragma warning disable CS0618 // 类型或成员已过时
            case SliderType.Catmull:
#pragma warning restore CS0618 // 类型或成员已过时
                ticks = ComputePathVerticesCatmull(sliderInfo);
                break;
            default:
                ticks = EmptyArray<Vector2>.Value;
                break;
        }

        return ticks;
    }

    #endregion

    #region Path Algorithms

    private static IReadOnlyList<Vector2> ComputePathVerticesPerfect(SliderInfo sliderInfo)
    {
        Vector2 p1;
        Vector2 p2;
        Vector2 p3;
        try
        {
            p1 = sliderInfo.StartPoint;
            p2 = sliderInfo.ControlPoints[0];
            p3 = sliderInfo.ControlPoints[1];
        }
        catch (IndexOutOfRangeException)
        {
            return ComputePathVerticesBezier(sliderInfo);
        }

        return PathApproximator.CircularArcToPiecewiseLinear(new[] { p1, p2, p3 });
    }

    private static IReadOnlyList<Vector2> ComputePathVerticesBezier(SliderInfo sliderInfo)
    {
        var points = new List<Vector2>();
        var groupedPoints = GetGroupedPoints(sliderInfo);
        for (var i = 0; i < groupedPoints.Count; i++)
        {
            var groupedPoint = groupedPoints[i];
            var bezierTrail = PathApproximator.BezierToPiecewiseLinear(groupedPoint);

            points.AddRange(bezierTrail);
        }

        return points;
    }

    private static IReadOnlyList<Vector2> ComputePathVerticesCatmull(SliderInfo sliderInfo)
    {
        var all = sliderInfo.ControlPoints.ToList();
        all.Insert(0, sliderInfo.StartPoint);

        var catmullTrail = PathApproximator.CatmullToPiecewiseLinear(all.ToArray());
        return catmullTrail;
    }

    #endregion

    #region Helpers

    private static (double[] segmentLengths, double[] cumulativeLengths, double totalLength) CreateCumulativeLengths(
        IReadOnlyList<Vector2> points)
    {
        var segmentCount = points.Count - 1;
        var segmentLengths = new double[segmentCount];
        var cumulativeLengths = new double[segmentCount];

        double totalLength = 0;
        for (var i = 0; i < segmentCount; i++)
        {
            var a = points[i];
            var b = points[i + 1];
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var len = Math.Sqrt(dx * dx + dy * dy);

            segmentLengths[i] = len;
            totalLength += len;
            cumulativeLengths[i] = totalLength;
        }

        return (segmentLengths, cumulativeLengths, totalLength);
    }

    private static Vector2 SamplePointAtLength(
        IReadOnlyList<Vector2> points,
        double[] segmentLengths,
        double[] cumulativeLengths,
        double targetLen)
    {
        if (targetLen <= 0)
        {
            return points[0];
        }

        var totalLen = cumulativeLengths.Length == 0 ? 0 : cumulativeLengths[cumulativeLengths.Length - 1];
        if (targetLen >= totalLen)
        {
            return points[points.Count - 1];
        }

        var index = Array.BinarySearch(cumulativeLengths, targetLen);
        index = index < 0 ? ~index : Math.Min(index + 1, cumulativeLengths.Length - 1);
        if (index >= cumulativeLengths.Length)
        {
            index = cumulativeLengths.Length - 1;
        }

        var previousSum = index == 0 ? 0 : cumulativeLengths[index - 1];
        var segLen = segmentLengths[index];
        if (segLen <= 0)
        {
            return points[index];
        }

        var t = (float)((targetLen - previousSum) / segLen);
        return Vector2.Lerp(points[index], points[index + 1], t);
    }

    private static List<Vector2[]> GetGroupedPoints(SliderInfo sliderInfo)
    {
        var controlPoints = sliderInfo.ControlPoints;
        var groupedPoints = new List<Vector2[]>();

        // 如果没有控制点，只有起点是无法构成滑条路径的
        if (controlPoints.Count == 0)
        {
            return groupedPoints;
        }

        var allPoints = new List<Vector2>(controlPoints.Count + 1) { sliderInfo.StartPoint };
        allPoints.AddRange(controlPoints);

        var currentGroup = new List<Vector2>();

        for (var i = 0; i < allPoints.Count - 1; i++)
        {
            var thisPoint = allPoints[i];
            var nextPoint = allPoints[i + 1];

            currentGroup.Add(thisPoint);

            // 如果当前点等于下一个点，说明这里是路径的分段点
            if (thisPoint.Equals(nextPoint))
            {
                // 如果当前组有效（超过1个点），则添加
                if (currentGroup.Count > 1)
                {
                    groupedPoints.Add(currentGroup.ToArray());
                }

                currentGroup.Clear();
            }
        }

        // 添加最后一个点
        currentGroup.Add(allPoints[allPoints.Count - 1]);

        if (currentGroup.Count > 1)
        {
            groupedPoints.Add(currentGroup.ToArray());
        }

        // Fallback: 如果有控制点但没有生成任何有效组（例如 A, A），则将整体作为一个组
        if (groupedPoints.Count == 0)
        {
            groupedPoints.Add(allPoints.ToArray());
        }

        return groupedPoints;
    }

    #endregion
}