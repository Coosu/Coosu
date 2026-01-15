using System;
using System.Collections.Generic;
using System.Numerics;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Utils;

namespace CoosuUnitTest.Beatmap;

internal static class SliderDiscreteSamplingShared
{
    internal static List<Vector2[]> GetGroupedPoints(SliderInfo sliderInfo)
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

    internal static IReadOnlyList<double> GetGroupedBezierLengths(IReadOnlyList<IReadOnlyList<Vector2>> groupedBezierPoints)
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

    internal static (int index, double lenInPart) CalculateWhichPart(IReadOnlyList<double> groupedBezierLengths, double relativeLen)
    {
        double sum = 0;
        for (var i = 0; i < groupedBezierLengths.Count; i++)
        {
            var len = groupedBezierLengths[i];
            sum += len;
            if (relativeLen < sum) return (i, len - (sum - relativeLen));
        }

        return (-1, -1);
    }

    internal static (Vector2 p, double r) GetCircle(Vector2 p1, Vector2 p2, Vector2 p3)
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

    internal static (double[] segmentLengths, double[] cumulativeLengths, double totalLength) CreateCumulativeLengths(IReadOnlyList<Vector2> points)
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

    internal static Vector2 SamplePointAtLength(
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

        var prevSum = index == 0 ? 0 : cumulativeLengths[index - 1];
        var segLen = segmentLengths[index];
        if (segLen <= 0)
        {
            return points[index];
        }

        var t = (float)((targetLen - prevSum) / segLen);
        return Vector2.Lerp(points[index], points[index + 1], t);
    }

    internal static List<Vector2> CreateHighResolutionBezierPolyline(SliderInfo sliderInfo, int resolution)
    {
        var groupedPoints = GetGroupedPoints(sliderInfo);
        var points = new List<Vector2>(groupedPoints.Count * (resolution + 1));
        for (var i = 0; i < groupedPoints.Count; i++)
        {
            var segment = groupedPoints[i];
            var trail = BezierHelper.GetBezierTrail(segment, resolution);

            if (trail.Count == 0)
            {
                continue;
            }

            if (points.Count != 0)
            {
                for (var j = 1; j < trail.Count; j++)
                {
                    points.Add(trail[j]);
                }
            }
            else
            {
                points.AddRange(trail);
            }
        }

        return points;
    }
}