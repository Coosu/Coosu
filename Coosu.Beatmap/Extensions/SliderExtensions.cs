﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Utils;
using Coosu.Shared;

// ReSharper disable once CheckNamespace
namespace Coosu.Beatmap
{
    public static class SliderExtensions
    {
        public static SliderEdge[] GetEdges(this SliderInfo sliderInfo)
        {
            var edges = new SliderEdge[sliderInfo.Repeat + 1];

            for (var i = 0; i < edges.Length; i++)
            {
                edges[i] = new SliderEdge
                {
                    Offset = sliderInfo.StartTime + sliderInfo.CurrentSingleDuration * i,
                    Point = i % 2 == 0 ? sliderInfo.StartPoint : sliderInfo.EndPoint,
                    EdgeHitsound = sliderInfo.EdgeHitsounds?[i] ?? HitsoundType.Normal,
                    EdgeSample = sliderInfo.EdgeSamples?[i] ?? ObjectSamplesetType.Auto,
                    EdgeAddition = sliderInfo.EdgeAdditions?[i] ?? ObjectSamplesetType.Auto
                };
            }

            return edges;
        }

        public static SliderTick[] GetSliderTicks(this SliderInfo sliderInfo)
        {
            var tickInterval = sliderInfo.CurrentBeatDuration / sliderInfo.CurrentTickRate;
            return ComputeDiscreteData(sliderInfo, tickInterval);
        }

        public static SliderTick[] GetSliderSlides(this SliderInfo sliderInfo)
        {
            // 60fps
            var interval = 1000 / 60d;
            return ComputeDiscreteData(sliderInfo, interval);
        }

        // todo: not cut by rhythm
        public static SliderTick[] ComputeDiscreteData(this SliderInfo sliderInfo, double intervalMilliseconds)
        {
            SliderTick[] ticks;
            switch (sliderInfo.SliderType)
            {
                case SliderType.Bezier:
                case SliderType.Linear:
                    ticks = ComputeBezierDiscreteData(sliderInfo, intervalMilliseconds);
                    break;
                case SliderType.Perfect:
                    ticks = ComputePerfectDiscreteData(sliderInfo, intervalMilliseconds);
                    break;
                default:
                    ticks = EmptyArray<SliderTick>.Value;
                    break;
            }

            return ticks.ToArray();
        }

        /// <summary>
        /// osu!lazer implementation to compute approximated data.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<Vector2> ComputeApproximatedData(this SliderInfo sliderInfo)
        {
            IReadOnlyList<Vector2> ticks;
            switch (sliderInfo.SliderType)
            {
                case SliderType.Bezier:
                case SliderType.Linear:
                    ticks = ComputeBezierApproximatedData(sliderInfo);
                    break;
                case SliderType.Perfect:
                    ticks = ComputePerfectApproximatedData(sliderInfo);
                    break;
#pragma warning disable CS0618 // 类型或成员已过时
                case SliderType.Catmull:
#pragma warning restore CS0618 // 类型或成员已过时
                    ticks = ComputeCatmullApproximatedData(sliderInfo);
                    break;
                default:
                    ticks = EmptyArray<Vector2>.Value;
                    break;
            }

            return ticks;
        }

        // todo: not cut by rhythm
        // todo: i forget math
        private static SliderTick[] ComputePerfectDiscreteData(SliderInfo sliderInfo, double fixedInterval)
        {
            if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
            {
                return EmptyArray<SliderTick>.Value;
            }

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
                sliderInfo.SliderType = SliderType.Linear;
                return ComputeBezierDiscreteData(sliderInfo, fixedInterval);
            }

            var circle = GetCircle(p1, p2, p3);

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
                var offset = i * fixedInterval; // 当前tick的相对时间
                var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
                if (isOnEdges) continue;
                var ratio = offset / sliderInfo.CurrentSingleDuration; // 相对整个滑条的时间比例，=距离比例
                var relativeRad = (radEnd - radStart) * ratio; // 至滑条头的距离
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

        private static SliderTick[] ComputeBezierDiscreteData(SliderInfo sliderInfo, double fixedInterval)
        {
            if (Math.Round(fixedInterval - sliderInfo.CurrentSingleDuration) >= 0)
            {
                return EmptyArray<SliderTick>.Value;
            }

            var groupedPoints = GetGroupedPoints(sliderInfo);
            var groupedBezierLengths = GetGroupedBezierLengths(groupedPoints);
            var totalLength = groupedBezierLengths.Sum();
            var ticks = new List<SliderTick>();

            for (int i = 1; i * fixedInterval < sliderInfo.CurrentSingleDuration; i++)
            {
                var offset = i * fixedInterval; // 当前tick的相对时间
                var isOnEdges = Math.Abs(offset % sliderInfo.CurrentSingleDuration) < 0.5;
                if (isOnEdges) continue;

                var ratio = offset / sliderInfo.CurrentSingleDuration; // 相对整个滑条的时间比例，=距离比例
                var relativeLen = totalLength * ratio; // 至滑条头的距离

                var (index, lenInPart) = CalculateWhichPart(sliderInfo, relativeLen); // can be optimized
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
                        // span's enumerator is ok
                        foreach (var baseTick in span)
                        {
                            // (sliderInfo.CurrentSingleDuration - (k.Offset - sliderInfo.StartTime)) + (i - 1) * sliderInfo.CurrentSingleDuration + sliderInfo.StartTime,
                            var tick = new SliderTick(
                                i * sliderInfo.CurrentSingleDuration - baseTick.Offset + sliderInfo.StartTime * 2,
                                baseTick.Point);
                            ticks.Add(tick);
                        }
                    }
                    else
                    {
                        // span's enumerator is ok
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

        private static IReadOnlyList<Vector2> ComputePerfectApproximatedData(SliderInfo sliderInfo)
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
                return ComputeBezierApproximatedData(sliderInfo);
            }

            return PathApproximator.ApproximateCircularArc(new[] { p1, p2, p3 });
        }

        private static IReadOnlyList<Vector2> ComputeBezierApproximatedData(SliderInfo sliderInfo)
        {
            var points = new List<Vector2>();
            var groupedPoints = GetGroupedPoints(sliderInfo);
            for (var i = 0; i < groupedPoints.Count; i++)
            {
                var groupedPoint = groupedPoints[i];
                var bezierTrail = PathApproximator.ApproximateBezier(groupedPoint);

                points.AddRange(bezierTrail.Select(k => new Vector2(k.X, k.Y)));
            }

            return points;
        }

        private static IReadOnlyList<Vector2> ComputeCatmullApproximatedData(SliderInfo sliderInfo)
        {
            var all = sliderInfo.ControlPoints.ToList();
            all.Insert(0, sliderInfo.StartPoint);

            var catmullTrail = PathApproximator.ApproximateCatmull(all);
            return catmullTrail;
        }

        private static (Vector2 p, double r) GetCircle(Vector2 p1, Vector2 p2, Vector2 p3)
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

        private static (int index, double lenInPart) CalculateWhichPart(SliderInfo sliderInfo, double relativeLen)
        {
            double sum = 0;
            var groupedBezierLengths = GetGroupedBezierLengths(GetGroupedPoints(sliderInfo));
            for (var i = 0; i < groupedBezierLengths.Count; i++)
            {
                var len = groupedBezierLengths[i];
                sum += len;
                if (relativeLen < sum) return (i, len - (sum - relativeLen));
            }

            return (-1, -1);
        }

        private static List<List<Vector2>> GetGroupedPoints(SliderInfo sliderInfo)
        {
            IReadOnlyList<Vector2> rawPoints = sliderInfo.ControlPoints;

            var groupedPoints = new List<List<Vector2>>();
            var currentGroup = new List<Vector2>();

            Vector2? nextPoint = default;
            for (var i = -1; i < rawPoints.Count - 1; i++)
            {
                var thisPoint = i == -1 ? sliderInfo.StartPoint : rawPoints[i];
                nextPoint = rawPoints[i + 1];
                currentGroup.Add(thisPoint);
                if (thisPoint.Equals(nextPoint))
                {
                    groupedPoints.Add(currentGroup);
                    currentGroup = new List<Vector2>();
                }
            }

            if (nextPoint != null)
                currentGroup.Add(nextPoint.Value);
            groupedPoints.Add(currentGroup);

            if (groupedPoints.Count == 0 && rawPoints.Count != 0)
            {
                currentGroup.AddRange(rawPoints);
            }

            return groupedPoints;
        }

        private static IReadOnlyList<double> GetGroupedBezierLengths(
            IReadOnlyList<IReadOnlyList<Vector2>> groupedBezierPoints)
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
    }
}