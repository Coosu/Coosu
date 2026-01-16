using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Shared;
using osu.Framework.Utils;

namespace Coosu.Beatmap;

/// <summary>
/// Contains extension methods for handling slider geometric calculations and timing point calculations.
/// This class is the core of processing osu! slider logic, responsible for converting abstract slider definitions into concrete coordinate points and timing points.
/// </summary>
public static class SliderExtensions
{
    #region Public Methods

    extension(ExtendedSliderInfo sliderInfo)
    {
        public SliderEdge[] GetEdges()
        {
            // The edge points of the slider include: start point, end point, and each repeat point.
            // For example: Repeat = 1 (one return trip), then the edge points are: Start -> End -> Start, a total of 3 points.
            // So the array size is Repeat + 1.
            var edges = new SliderEdge[sliderInfo.Repeat + 1];
            for (var i = 0; i < edges.Length; i++)
            {
                edges[i] = new SliderEdge
                (
                    offset: sliderInfo.StartTime + sliderInfo.CurrentSingleDuration * i,
                    point: i % 2 == 0 ? sliderInfo.StartPoint : sliderInfo.EndPoint,
                    edgeHitsound: sliderInfo.EdgeHitsounds?[i] ?? sliderInfo.BaseObject.Hitsound,
                    edgeSample: sliderInfo.EdgeSamples?[i] ?? sliderInfo.BaseObject.SampleSet,
                    edgeAddition: sliderInfo.EdgeAdditions?[i] ?? sliderInfo.BaseObject.AdditionSet,
                    isHitsoundDefined: sliderInfo.EdgeHitsounds == null
                );
            }

            return edges;
        }

        /// <summary>
        /// Gets the Ticks in the middle of the slider. These points not only affect scoring but also produce sound.
        /// </summary>
        public SliderTick[] GetSliderTicks()
        {
            // The Tick interval is determined by the current BPM and SV (Slider Velocity).
            var tickInterval = sliderInfo.CurrentBeatDuration / sliderInfo.CurrentTickRate;
            return ComputeTicks(sliderInfo, tickInterval);
        }

        /// <summary>
        /// Gets the sampling points during the slider ball movement (usually used for rendering or simulating cursor movement).
        /// </summary>
        public SliderTick[] GetSliderSlides()
        {
            // 60fps sampling rate, i.e., one point every 16.66ms.
            var interval = 1000 / 60d;
            return ComputeTicks(sliderInfo, interval);
        }

        public SliderTick[] ComputeTicks(double intervalMilliseconds)
        {
            return ComputeTicksByInterval(sliderInfo, intervalMilliseconds);
        }

        /// <summary>
        /// Iteratively gets the edge points. Use this if you don't want to allocate the entire array at once.
        /// </summary>
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

    /// <summary>
    /// This is the core logic for computing Ticks.
    /// Working principle: First calculate the geometric path of the slider, then "interpolate" the corresponding points on the path based on the time interval.
    /// </summary>
    private static SliderTick[] ComputeTicksByInterval(ExtendedSliderInfo sliderInfo, double fixedInterval)
    {
        // If the interval is longer than the total duration of a single slider slide, there are definitely no ticks in between.
        if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
        {
            return EmptyArray<SliderTick>.Value;
        }

        // Using ValueListBuilder and stackalloc is for extreme performance optimization.
        // Because slider calculations are very frequent, using List<Vector3> would generate a lot of GC garbage.
        var pathBuilder = new ValueListBuilder<Vector3>(stackalloc Vector3[64]);
        try
        {
            var segLens = new ValueListBuilder<double>(stackalloc double[64]);
            var cumLens = new ValueListBuilder<double>(stackalloc double[64]);
            try
            {
                // 1. Calculate path vertices. This is the most time-consuming step, which discretizes Bezier curves etc. into a series of line segments.
                sliderInfo.ComputePathVertices(ref pathBuilder, ref segLens, ref cumLens);
                var approximated = pathBuilder.AsSpan();

                if (approximated.Length < 2)
                {
                    return EmptyArray<SliderTick>.Value;
                }

                // Get total path length.
                var totalLength = cumLens.Length > 0 ? cumLens[cumLens.Length - 1] : 0;
                if (totalLength <= 0)
                {
                    return EmptyArray<SliderTick>.Value;
                }

                var ticks = new ValueListBuilder<SliderTick>(stackalloc SliderTick[64]);
                try
                {
                    var segmentIndex = 0;
                    var previousSum = 0d;

                    // 2. Iterate through each time interval point.
                    for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
                    {
                        var offset = i * fixedInterval;

                        // If this point is too close to the edge (start/end), it is usually not considered a tick.
                        var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
                        if (isOnEdges) continue;

                        // Calculate the ratio of the slider length corresponding to the current time point.
                        var ratio = offset / sliderInfo.CurrentSingleDuration;
                        var targetLen = totalLength * ratio;

                        // Find the coordinate point corresponding to this length on the discrete path.
                        // InterpolateSequential is an optimized search algorithm that takes advantage of sequentiality.
                        var tickPoint = InterpolateSequential(approximated, segLens.AsSpan(), cumLens.AsSpan(),
                            targetLen, ref segmentIndex, ref previousSum);
                        ticks.Append(new SliderTick(sliderInfo.StartTime + offset, tickPoint));
                    }

                    // 3. If the slider has repeats (Repeat > 1), we need to copy/mirror the ticks calculated in the first segment to the subsequent segments.
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

    /// <summary>
    /// Handles Ticks during slider repeats.
    /// Since the Ticks of the first segment have already been calculated, the subsequent repeat segments are actually "mirrors" or "translations" of the first segment.
    /// Directly reusing calculation results can avoid repeating expensive geometric calculations.
    /// </summary>
    private static void AppendRepeatTicks(ExtendedSliderInfo sliderInfo, ref ValueListBuilder<SliderTick> ticks)
    {
        var count = ticks.Length;
        SliderTick[]? pooled = null;
        // If there are few points, allocate space directly on the stack for temporary caching; otherwise borrow an array from the memory pool.
        Span<SliderTick> span = count <= 256
            ? stackalloc SliderTick[count]
            : (pooled = ArrayPool<SliderTick>.Shared.Rent(count)).AsSpan(0, count);

        try
        {
            // First copy the calculated ticks of the first segment into the cache.
            ticks.AsSpan().CopyTo(span);

            for (int i = 2; i <= sliderInfo.Repeat; i++)
            {
                // Every repeat, the direction is reversed.
                // For example, if the first segment is 0 -> 1, the second segment is 1 -> 0.
                // So we need to reverse these points.
                span.Reverse();
                var reverse = i % 2 == 0;
                if (reverse)
                {
                    // Even repeats (going back): time increases, but position is reversed.
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
                    // Odd repeats (going forward): consistent with the first segment direction, only time is shifted.
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
    /// Calculates the geometric path vertices of the slider. This is the most complex part of geometric processing.
    /// The ref parameter here is to reuse the caller's memory pool.
    /// </summary>
    private static void ComputePathVertices(this SliderInfo sliderInfo,
        ref ValueListBuilder<Vector3> builder,
        ref ValueListBuilder<double> segmentLengths,
        ref ValueListBuilder<double> cumulativeLengths)
    {
        var controlPoints = sliderInfo.ControlPoints;
        if (controlPoints.Count == 0) return;

        var currentType = sliderInfo.SliderType;
        var maxPoints = controlPoints.Count + 1;
        Vector3[]? pooled = null;

        // Also use stack allocation or memory pool to cache points during parsing.
        Span<Vector3> segmentBuffer = maxPoints <= 256
            ? stackalloc Vector3[maxPoints]
            : (pooled = ArrayPool<Vector3>.Shared.Rent(maxPoints)).AsSpan(0, maxPoints);

        try
        {
            int segmentCount = 0;
            segmentBuffer[segmentCount++] = sliderInfo.StartPoint;

            // osu!lazer sliders support "multi-segment paths".
            // If the Z coordinate of the control point is not 0, this is actually a Hack, indicating a switch point for the path type.
            // For example: the first half is Linear, there is a point with Z!=0 in the middle, and the second half becomes Bezier.
            for (int i = 0; i < controlPoints.Count; i++)
            {
                var cp = controlPoints[i];
                if (cp.Z != 0) // Type delimiter
                {
                    segmentBuffer[segmentCount++] = controlPoints[i + 1];
                    if (segmentCount > 1)
                    {
                        ComputePath(currentType, segmentBuffer.Slice(0, segmentCount), ref builder);
                    }

                    // Prepare for the next segment.
                    var lastPoint = segmentBuffer[segmentCount - 1];
                    segmentCount = 0;
                    segmentBuffer[segmentCount++] = lastPoint;

                    // The Z value hides the type information of the next segment.
                    currentType = (SliderType)((int)cp.Z - 1);
                    i++;
                }
                else
                {
                    segmentBuffer[segmentCount++] = cp;
                }
            }

            // Process the last segment (or the only segment).
            if (segmentCount > 1)
            {
                ComputePath(currentType, segmentBuffer.Slice(0, segmentCount), ref builder);
            }

            // --- Core logic: Trim or Extend ---
            // This is very critical: the PixelLength defined by the slider is the "authoritative" length.
            // The geometric path length generated by control points may be longer than PixelLength (needs trimming) or shorter (needs extension).

            var pixelLength = sliderInfo.PixelLength;
            if (builder.Length > 1)
            {
                double currentLength = 0;
                var count = builder.Length;
                for (int i = 0; i < count - 1; i++)
                {
                    var p1 = builder[i];
                    var p2 = builder[i + 1];
                    var dx = (double)p2.X - p1.X;
                    var dy = (double)p2.Y - p1.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    // If adding this segment exceeds the predetermined length, it needs to be trimmed.
                    if (pixelLength > 0 && currentLength + dist >= pixelLength)
                    {
                        var remaining = pixelLength - currentLength;
                        if (remaining <= 0.0001) // Tolerance
                        {
                            builder.Length = i + 1; // Just reached here
                        }
                        else
                        {
                            // Linear interpolation to find the truncation point.
                            var t = (float)(remaining / dist);
                            builder[i + 1] = Vector3.Lerp(p1, p2, t);
                            builder.Length = i + 2;

                            segmentLengths.Append(remaining);
                            cumulativeLengths.Append(pixelLength);
                        }

                        return; // Done, discard the remaining points.
                    }

                    segmentLengths.Append(dist);
                    currentLength += dist;
                    cumulativeLengths.Append(currentLength);
                }

                // If all points are exhausted and the length is still not enough for PixelLength, "extension" is needed.
                // This situation is rare in linear sliders, but may occur by user manually edit.
                // The approach is: continue to draw a straight line along the direction (tangent direction) of the last segment.
                if (pixelLength > 0 && currentLength < pixelLength)
                {
                    var pPrev = builder[builder.Length - 2];
                    var pLast = builder[builder.Length - 1];
                    var dx = (double)pLast.X - pPrev.X;
                    var dy = (double)pLast.Y - pPrev.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist > 1e-4) // Avoid division by zero
                    {
                        var remaining = pixelLength - currentLength;
                        var ratio = remaining / dist;
                        var newPoint = new Vector3(
                            (float)(pLast.X + dx * ratio),
                            (float)(pLast.Y + dy * ratio),
                            pLast.Z
                        );
                        builder.Append(newPoint);

                        segmentLengths.Append(remaining);
                        cumulativeLengths.Append(pixelLength);
                    }
                }
            }
        }
        finally
        {
            if (pooled != null) ArrayPool<Vector3>.Shared.Return(pooled);
        }
    }

    private static void ComputePath(SliderType type, scoped ReadOnlySpan<Vector3> points,
        ref ValueListBuilder<Vector3> builder)
    {
        switch (type)
        {
            case SliderType.Bezier:
            case SliderType.Linear:
            case SliderType.Bezier4:
                ComputePathVerticesBezier(points, ref builder);
                break;
            case SliderType.Perfect:
                ComputePathVerticesPerfect(points, ref builder);
                break;
#pragma warning disable CS0618 // Type or member is obsolete
            case SliderType.Catmull:
#pragma warning restore CS0618 // Type or member is obsolete
                ComputePathVerticesCatmull(points, ref builder);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Path Algorithms

    private static void ComputePathVerticesPerfect(scoped ReadOnlySpan<Vector3> points,
        ref ValueListBuilder<Vector3> builder)
    {
        // Perfect arc must be determined by 3 points (start, middle, end).
        // If the number of points is incorrect, degenerate to Bezier curve processing.
        if (points.Length != 3)
        {
            ComputePathVerticesBezier(points, ref builder);
            return;
        }

        var result = PathApproximator.CircularArcToPiecewiseLinear(points);
        if (result.Count == 0)
        {
            ComputePathVerticesBezier(points, ref builder);
            return;
        }

        foreach (var p in result) builder.Append(p);
    }

    private static void ComputePathVerticesBezier(scoped ReadOnlySpan<Vector3> allPoints,
        ref ValueListBuilder<Vector3> builder)
    {
        int totalCount = allPoints.Length;
        if (totalCount < 2) return;

        bool anySegmentAdded = false;
        int start = 0;

        // A special rule for Bezier curves:
        // If two control points overlap (coordinates are exactly the same), this is called a "Red Anchor".
        // This will cut the Bezier curve, calculating the previous and next segments separately, forming a sharp corner.
        for (var i = 0; i < totalCount - 1; i++)
        {
            var thisPoint = allPoints[i];
            var nextPoint = allPoints[i + 1];

            // If the current point equals the next point, it means this is a segment point of the path.
            if (thisPoint.Equals(nextPoint))
            {
                int length = i - start + 1;
                // If the current group is valid (more than 1 point), add it.
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

        // Add the last point.
        int lastLength = totalCount - start;
        if (lastLength > 1)
        {
            var subPoints = allPoints.Slice(start, lastLength);
            var bezierTrail = PathApproximator.BezierToPiecewiseLinear(subPoints);
            foreach (var p in bezierTrail) builder.Append(p);
            anySegmentAdded = true;
        }

        // Fallback: If there are control points but no valid groups are generated (e.g., A, A), treat the whole as one group.
        if (!anySegmentAdded)
        {
            var bezierTrail = PathApproximator.BezierToPiecewiseLinear(allPoints);
            foreach (var p in bezierTrail) builder.Append(p);
        }
    }

    private static void ComputePathVerticesCatmull(scoped ReadOnlySpan<Vector3> allPoints,
        ref ValueListBuilder<Vector3> builder)
    {
        // Catmull: an old algorithm, rarely used now.
        var catmullTrail = PathApproximator.CatmullToPiecewiseLinear(allPoints);
        foreach (var p in catmullTrail) builder.Append(p);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Sequential interpolation search.
    /// This is a targeted optimization: because we search for Tick positions in order (Offset from small to large),
    /// we don't need to use binary search every time, but can remember the last found segmentIndex and continue searching from there.
    /// This reduces the search complexity from O(N log M) to O(M), where M is the number of path segments.
    /// </summary>
    private static Vector3 InterpolateSequential(
        ReadOnlySpan<Vector3> points,
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

        // Starting from the last index, trace back to the segment containing targetLen.
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

        // Linear interpolation to calculate concrete coordinates.
        var t = (float)((targetLen - previousSum) / segLen);
        return Vector3.Lerp(points[segmentIndex], points[segmentIndex + 1], t);
    }

    #endregion
}