using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Shared;
using osu.Framework.Utils;

namespace Coosu.Beatmap;

public static class SliderExtensions
{
    #region Public Methods

    extension(ExtendedSliderInfo sliderInfo)
    {
        public SliderEdge[] GetEdges()
        {
            return EnumerateEdges(sliderInfo).ToArray();
        }

        public SliderTick[] GetSliderTicks()
        {
            var tickInterval = sliderInfo.CurrentBeatDuration / sliderInfo.CurrentTickRate;
            return ComputeTicks(sliderInfo, tickInterval);
        }

        public SliderTick[] GetSliderSlides()
        {
            // 60fps
            var interval = 1000 / 60d;
            return ComputeTicks(sliderInfo, interval);
        }

        // todo: not cut by rhythm
        public SliderTick[] ComputeTicks(double intervalMilliseconds)
        {
            return ComputeTicksByInterval(sliderInfo, intervalMilliseconds);
        }

        public IEnumerable<SliderEdge> EnumerateEdges()
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

        var pathBuilder = new ValueListBuilder<Vector2>(stackalloc Vector2[64]);
        try
        {
            sliderInfo.ComputePathVertices(ref pathBuilder);
            var approximated = pathBuilder.AsSpan();

            if (approximated.Length < 2)
            {
                return EmptyArray<SliderTick>.Value;
            }

            var segLens = new ValueListBuilder<double>(stackalloc double[64]);
            var cumLens = new ValueListBuilder<double>(stackalloc double[64]);
            try
            {
                CreateCumulativeLengths(approximated, ref segLens, ref cumLens, out var totalLength);
                if (totalLength <= 0)
                {
                    return EmptyArray<SliderTick>.Value;
                }

                var ticks = new ValueListBuilder<SliderTick>(stackalloc SliderTick[64]);
                try
                {
                    var segmentIndex = 0;
                    var previousSum = 0d;
                    for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
                    {
                        var offset = i * fixedInterval;
                        var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
                        if (isOnEdges) continue;

                        var ratio = offset / sliderInfo.CurrentSingleDuration;
                        var targetLen = totalLength * ratio;
                        var tickPoint = InterpolateSequential(approximated, segLens.AsSpan(), cumLens.AsSpan(),
                            targetLen, ref segmentIndex, ref previousSum);
                        ticks.Append(new SliderTick(sliderInfo.StartTime + offset, tickPoint));
                    }

                    if (sliderInfo.Repeat > 1)
                    {
                        AppendRepeatTicks(sliderInfo, ref ticks);
                    }

                    return ticks.AsSpan().ToArray();
                }
                finally
                {
                    ticks.Dispose();
                }
            }
            finally
            {
                segLens.Dispose();
                cumLens.Dispose();
            }
        }
        finally
        {
            pathBuilder.Dispose();
        }
    }

    private static void AppendRepeatTicks(ExtendedSliderInfo sliderInfo, ref ValueListBuilder<SliderTick> ticks)
    {
        var count = ticks.Length;
        SliderTick[]? pooled = null;
        Span<SliderTick> span = count <= 256
            ? stackalloc SliderTick[count]
            : (pooled = ArrayPool<SliderTick>.Shared.Rent(count)).AsSpan(0, count);

        try
        {
            ticks.AsSpan().CopyTo(span);

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
                        ticks.Append(tick);
                    }
                }
                else
                {
                    foreach (var baseTick in span)
                    {
                        var tick = new SliderTick(baseTick.Offset + (i - 1) * sliderInfo.CurrentSingleDuration,
                            baseTick.Point);
                        ticks.Append(tick);
                    }
                }
            }
        }
        finally
        {
            if (pooled != null) ArrayPool<SliderTick>.Shared.Return(pooled);
        }
    }

    /// <summary>
    /// osu!lazer implementation to compute approximated data.
    /// </summary>
    /// <returns></returns>
    private static void ComputePathVertices(this SliderInfo sliderInfo, ref ValueListBuilder<Vector2> builder)
    {
        switch (sliderInfo.SliderType)
        {
            case SliderType.Bezier:
            case SliderType.Linear:
                ComputePathVerticesBezier(sliderInfo, ref builder);
                break;
            case SliderType.Perfect:
                ComputePathVerticesPerfect(sliderInfo, ref builder);
                break;
#pragma warning disable CS0618 // 类型或成员已过时
            case SliderType.Catmull:
#pragma warning restore CS0618 // 类型或成员已过时
                ComputePathVerticesCatmull(sliderInfo, ref builder);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Path Algorithms

    private static void ComputePathVerticesPerfect(SliderInfo sliderInfo, ref ValueListBuilder<Vector2> builder)
    {
        if (sliderInfo.ControlPoints.Count < 2)
        {
            ComputePathVerticesBezier(sliderInfo, ref builder);
            return;
        }

        Span<Vector2> points = stackalloc Vector2[3];
        points[0] = sliderInfo.StartPoint;
        points[1] = sliderInfo.ControlPoints[0];
        points[2] = sliderInfo.ControlPoints[1];

        var result = PathApproximator.CircularArcToPiecewiseLinear(points);
        foreach (var p in result) builder.Append(p);
    }

    private static void ComputePathVerticesBezier(SliderInfo sliderInfo, ref ValueListBuilder<Vector2> builder)
    {
        var controlPoints = sliderInfo.ControlPoints;
        if (controlPoints.Count == 0) return;

        int totalCount = controlPoints.Count + 1;
        Vector2[]? pooled = null;
        Span<Vector2> allPoints = totalCount <= 256
            ? stackalloc Vector2[totalCount]
            : (pooled = ArrayPool<Vector2>.Shared.Rent(totalCount)).AsSpan(0, totalCount);

        try
        {
            allPoints[0] = sliderInfo.StartPoint;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                allPoints[i + 1] = controlPoints[i];
            }

            bool anySegmentAdded = false;
            int start = 0;

            for (var i = 0; i < totalCount - 1; i++)
            {
                var thisPoint = allPoints[i];
                var nextPoint = allPoints[i + 1];

                // 如果当前点等于下一个点，说明这里是路径的分段点
                if (thisPoint.Equals(nextPoint))
                {
                    int length = i - start + 1;
                    // 如果当前组有效（超过1个点），则添加
                    if (length > 1)
                    {
                        var subPoints = allPoints.Slice(start, length);
                        var bezierTrail = PathApproximator.BezierToPiecewiseLinear(subPoints);
                        foreach (var p in bezierTrail) builder.Append(p);
                        anySegmentAdded = true;
                    }

                    start = i + 1;
                }
            }

            // 添加最后一个点
            int lastLength = totalCount - start;
            if (lastLength > 1)
            {
                var subPoints = allPoints.Slice(start, lastLength);
                var bezierTrail = PathApproximator.BezierToPiecewiseLinear(subPoints);
                foreach (var p in bezierTrail) builder.Append(p);
                anySegmentAdded = true;
            }

            // Fallback: 如果有控制点但没有生成任何有效组（例如 A, A），则将整体作为一个组
            if (!anySegmentAdded)
            {
                var bezierTrail = PathApproximator.BezierToPiecewiseLinear(allPoints);
                foreach (var p in bezierTrail) builder.Append(p);
            }
        }
        finally
        {
            if (pooled != null) ArrayPool<Vector2>.Shared.Return(pooled);
        }
    }

    private static void ComputePathVerticesCatmull(SliderInfo sliderInfo, ref ValueListBuilder<Vector2> builder)
    {
        var controlPoints = sliderInfo.ControlPoints;
        int totalCount = controlPoints.Count + 1;
        Vector2[]? pooled = null;
        Span<Vector2> allPoints = totalCount <= 256
            ? stackalloc Vector2[totalCount]
            : (pooled = ArrayPool<Vector2>.Shared.Rent(totalCount)).AsSpan(0, totalCount);

        try
        {
            allPoints[0] = sliderInfo.StartPoint;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                allPoints[i + 1] = controlPoints[i];
            }

            var catmullTrail = PathApproximator.CatmullToPiecewiseLinear(allPoints);
            foreach (var p in catmullTrail) builder.Append(p);
        }
        finally
        {
            if (pooled != null) ArrayPool<Vector2>.Shared.Return(pooled);
        }
    }

    #endregion

    #region Helpers

    private static void CreateCumulativeLengths(
        ReadOnlySpan<Vector2> points,
        ref ValueListBuilder<double> segmentLengths,
        ref ValueListBuilder<double> cumulativeLengths,
        out double totalLength)
    {
        var segmentCount = points.Length - 1;

        totalLength = 0;
        for (var i = 0; i < segmentCount; i++)
        {
            var a = points[i];
            var b = points[i + 1];
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var len = Math.Sqrt(dx * dx + dy * dy);

            segmentLengths.Append(len);
            totalLength += len;
            cumulativeLengths.Append(totalLength);
        }
    }

    private static Vector2 InterpolateSequential(
        ReadOnlySpan<Vector2> points,
        ReadOnlySpan<double> segmentLengths,
        ReadOnlySpan<double> cumulativeLengths,
        double targetLen,
        ref int segmentIndex,
        ref double previousSum)
    {
        if (targetLen <= 0)
        {
            segmentIndex = 0;
            previousSum = 0;
            return points[0];
        }

        var totalLen = cumulativeLengths.Length == 0 ? 0 : cumulativeLengths[cumulativeLengths.Length - 1];
        if (targetLen >= totalLen)
        {
            segmentIndex = Math.Max(0, cumulativeLengths.Length - 1);
            previousSum = segmentIndex == 0 ? 0 : cumulativeLengths[segmentIndex - 1];
            return points[points.Length - 1];
        }

        if (segmentIndex < 0) segmentIndex = 0;
        if (segmentIndex >= cumulativeLengths.Length) segmentIndex = cumulativeLengths.Length - 1;

        var currentCum = cumulativeLengths[segmentIndex];
        while (segmentIndex < cumulativeLengths.Length - 1 && targetLen >= currentCum)
        {
            previousSum = currentCum;
            segmentIndex++;
            currentCum = cumulativeLengths[segmentIndex];
        }

        var segLen = segmentLengths[segmentIndex];
        if (segLen <= 0)
        {
            return points[segmentIndex];
        }

        var t = (float)((targetLen - previousSum) / segLen);
        return Vector2.Lerp(points[segmentIndex], points[segmentIndex + 1], t);
    }

    #endregion
}