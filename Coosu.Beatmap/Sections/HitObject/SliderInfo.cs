using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Utils;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.HitObject
{
    public class SliderInfo
    {
        private SliderEdge[]? _edges;
        private SliderTick[]? _ticks;
        private SliderTick[]? _ballTrail;
        private IReadOnlyList<List<Vector2>>? _groupedPoints;
        private IReadOnlyList<double>? _groupedBezierLengths;

        private readonly int _offset;
        private readonly double _beatDuration;
        private readonly double _sliderMultiplier;
        private readonly double _tickRate;
        private readonly double _singleElapsedTime;
        private Vector2[] _curvePoints;

        public SliderInfo(Vector2 startPoint, int offset, double beatDuration, double sliderMultiplier, double tickRate, decimal pixelLength)
        {
            StartPoint = startPoint;
            PixelLength = pixelLength;
            _offset = offset;
            _beatDuration = beatDuration;
            _sliderMultiplier = sliderMultiplier;
            _tickRate = tickRate;
            _singleElapsedTime = (double)(PixelLength / (100 * (decimal)_sliderMultiplier) * (decimal)_beatDuration);
        }

        public SliderType SliderType { get; set; }

        public Vector2[] CurvePoints
        {
            get => _curvePoints;
            set { _curvePoints = value; _groupedPoints = null; }
        }

        public IReadOnlyList<IReadOnlyList<Vector2>> GroupedPoints => _groupedPoints ??= GetGroupedPoints();
        public int Repeat { get; set; }
        public decimal PixelLength { get; set; }
        public HitsoundType[]? EdgeHitsounds { get; set; }
        public ObjectSamplesetType[]? EdgeSamples { get; set; }
        public ObjectSamplesetType[]? EdgeAdditions { get; set; }

        public double StartTime => _offset;
        public double EndTime => Edges[Edges.Length - 1].Offset;

        public Vector2 StartPoint { get; }
        public Vector2 EndPoint => CurvePoints.Last();

        public SliderEdge[] Edges
        {
            get
            {
                if (_edges == null)
                {
                    var edges = new SliderEdge[Repeat + 1];

                    for (var i = 0; i < edges.Length; i++)
                    {
                        edges[i] = new SliderEdge
                        {
                            Offset = _offset + _singleElapsedTime * i,
                            Point = i % 2 == 0 ? StartPoint : EndPoint,
                            EdgeHitsound = EdgeHitsounds?[i] ?? HitsoundType.Normal,
                            EdgeSample = EdgeSamples?[i] ?? ObjectSamplesetType.Auto,
                            EdgeAddition = EdgeAdditions?[i] ?? ObjectSamplesetType.Auto
                        };
                    }

                    _edges = edges;
                }

                return _edges;
            }
        }

        public SliderTick[] Ticks
        {
            get
            {
                if (_ticks == null)
                {
                    var tickInterval = _beatDuration / _tickRate;
                    _ticks = GetDiscreteSliderTrailData(tickInterval);
                }

                return _ticks;
            }
        }

        public SliderTick[] BallTrail
        {
            get
            {
                if (_ballTrail == null)
                {
                    // 60fps
                    var interval = 1000 / 60d;
                    _ballTrail = GetDiscreteSliderTrailData(interval);
                }

                return _ballTrail;
            }
        }

        private IReadOnlyList<double> GroupedBezierLengths => _groupedBezierLengths ??= GetGroupedBezierLengths(GroupedPoints);

        public SliderTick[] GetDiscreteSliderTrailData(double intervalMilliseconds)
        {
            SliderTick[] ticks;
            switch (SliderType)
            {
                case SliderType.Bezier:
                case SliderType.Linear:
                    ticks = GetBezierDiscreteBallData(intervalMilliseconds);
                    break;
                case SliderType.Perfect:
                    ticks = GetPerfectDiscreteBallData(intervalMilliseconds);
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
        public IReadOnlyList<Vector2> ComputeApproximatedData()
        {
            IReadOnlyList<Vector2> ticks;
            switch (SliderType)
            {
                case SliderType.Bezier:
                case SliderType.Linear:
                    ticks = ComputeBezierApproximatedData();
                    break;
                case SliderType.Perfect:
                    ticks = ComputePerfectApproximatedData();
                    break;
                default:
                    ticks = EmptyArray<Vector2>.Value;
                    break;
            }

            return ticks;
        }

        public override string ToString()
        {
            var sampleList = new List<(ObjectSamplesetType, ObjectSamplesetType)>();
            string edgeSampleStr;
            string edgeHitsoundStr;
            if (EdgeSamples != null)
            {
                for (var i = 0; i < EdgeSamples.Length; i++)
                {
                    var objectSamplesetType = EdgeSamples[i];
                    var objectAdditionType = EdgeAdditions[i];
                    sampleList.Add((objectSamplesetType, objectAdditionType));
                }

                edgeSampleStr = "," + string.Join("|", sampleList.Select(k => $"{(int)k.Item1}:{(int)k.Item2}"));
            }
            else
            {
                edgeSampleStr = "";
            }

            if (EdgeHitsounds != null)
            {
                edgeHitsoundStr = "," + string.Join("|", EdgeHitsounds.Select(k => $"{(int)k}"));
            }
            else
            {
                edgeHitsoundStr = "";
            }

            return string.Format("{0}|{1},{2},{3}{4}{5}",
                SliderType.ParseToCode(),
                string.Join("|", CurvePoints.Select(k => $"{k.X}:{k.Y}")),
                Repeat,
                PixelLength,
                edgeHitsoundStr,
                edgeSampleStr);
        }

        private (int index, double lenInPart) CalculateWhichPart(double relativeLen)
        {
            double sum = 0;
            for (var i = 0; i < GroupedBezierLengths.Count; i++)
            {
                var len = GroupedBezierLengths[i];
                sum += len;
                if (relativeLen < sum) return (i, len - (sum - relativeLen));
            }

            return (-1, -1);
        }

        // todo: not cut by rhythm
        // todo: i forget math
        private SliderTick[] GetPerfectDiscreteBallData(double fixedInterval)
        {
            if (Math.Round(fixedInterval - _singleElapsedTime) >= 0)
            {
                return EmptyArray<SliderTick>.Value;
            }

            Vector2 p1;
            Vector2 p2;
            Vector2 p3;
            try
            {
                p1 = StartPoint;
                p2 = CurvePoints[0];
                p3 = CurvePoints[1];
            }
            catch (IndexOutOfRangeException)
            {
                this.SliderType = SliderType.Linear;
                return GetBezierDiscreteBallData(fixedInterval);
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

            for (int i = 1; i * fixedInterval < _singleElapsedTime; i++)
            {
                var offset = i * fixedInterval; // 当前tick的相对时间
                if (Edges.Any(k => Math.Abs(k.Offset - _offset - offset) < 0.01))
                    continue;
                var ratio = offset / _singleElapsedTime; // 相对整个滑条的时间比例，=距离比例
                var relativeRad = (radEnd - radStart) * ratio; // 至滑条头的距离
                var offsetRad = radStart + relativeRad;
                var x = circle.p.X + circle.r * Math.Cos(offsetRad);
                var y = circle.p.Y + circle.r * Math.Sin(offsetRad);

                ticks.Add(new SliderTick(_offset + offset, new Vector2((float)x, (float)y)));
            }

            if (Repeat > 1)
            {
                var firstSingleCopy = ticks.ToArray();
                for (int i = 2; i <= Repeat; i++)
                {
                    var reverse = i % 2 == 0;
                    if (reverse)
                    {
                        ticks.AddRange(firstSingleCopy.Reverse().Select(k =>
                            new SliderTick((_singleElapsedTime - (k.Offset - _offset)) + (i - 1) * _singleElapsedTime + _offset,
                                k.Point)));
                    }
                    else
                    {
                        ticks.AddRange(firstSingleCopy.Select(k =>
                            new SliderTick(k.Offset + (i - 1) * _singleElapsedTime, k.Point)));
                    }
                }
            }

            return ticks.ToArray();
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

        // todo: not cut by rhythm
        private SliderTick[] GetBezierDiscreteBallData(double fixedInterval)
        {
            if (Math.Round(fixedInterval - _singleElapsedTime) >= 0)
            {
                return EmptyArray<SliderTick>.Value;
            }

            var totalLength = GroupedBezierLengths.Sum();
            var ticks = new List<SliderTick>();

            for (int i = 1; i * fixedInterval < _singleElapsedTime; i++)
            {
                var offset = i * fixedInterval; // 当前tick的相对时间
                if (Edges.Any(k => Math.Abs(k.Offset - _offset - offset) < 0.01))
                    continue;

                var ratio = offset / _singleElapsedTime; // 相对整个滑条的时间比例，=距离比例
                var relativeLen = totalLength * ratio; // 至滑条头的距离

                var (index, lenInPart) = CalculateWhichPart(relativeLen); // can be optimized
                var len = GroupedBezierLengths[index];
                var tickPoint = BezierHelper.Compute(GroupedPoints[index], (float)(lenInPart / len));
                ticks.Add(new SliderTick(_offset + offset, tickPoint));
            }

            if (Repeat > 1)
            {
                var firstSingleCopy = ticks.ToArray();
                for (int i = 2; i <= Repeat; i++)
                {
                    var reverse = i % 2 == 0;
                    if (reverse)
                    {
                        ticks.AddRange(firstSingleCopy.Reverse().Select(k =>
                            new SliderTick((_singleElapsedTime - (k.Offset - _offset)) + (i - 1) * _singleElapsedTime + _offset,
                                k.Point)));
                    }
                    else
                    {
                        ticks.AddRange(firstSingleCopy.Select(k =>
                            new SliderTick(k.Offset + (i - 1) * _singleElapsedTime, k.Point)));
                    }
                }
            }

            return ticks.ToArray();
        }

        private IReadOnlyList<Vector2> ComputePerfectApproximatedData()
        {
            Vector2 p1;
            Vector2 p2;
            Vector2 p3;
            try
            {
                p1 = StartPoint;
                p2 = CurvePoints[0];
                p3 = CurvePoints[1];
            }
            catch (IndexOutOfRangeException)
            {
                return ComputeBezierApproximatedData();
            }

            return PathApproximator.ApproximateCircularArc(new[] { p1, p2, p3 });
        }

        private IReadOnlyList<Vector2> ComputeBezierApproximatedData()
        {
            var points = new List<Vector2>();
            for (var i = 0; i < GroupedPoints.Count; i++)
            {
                var groupedPoint = GroupedPoints[i];
                var bezierTrail = PathApproximator.ApproximateBezier(groupedPoint);

                points.AddRange(bezierTrail.Select(k => new Vector2(k.X, k.Y)));
            }

            return points;
        }

        private static IReadOnlyList<double> GetGroupedBezierLengths(IReadOnlyList<IReadOnlyList<Vector2>> groupedBezierPoints)
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

        private List<List<Vector2>> GetGroupedPoints()
        {
            IReadOnlyList<Vector2> rawPoints = CurvePoints;

            var groupedPoints = new List<List<Vector2>>();
            var currentGroup = new List<Vector2>();

            Vector2? nextPoint = default;
            for (var i = -1; i < rawPoints.Count - 1; i++)
            {
                var thisPoint = i == -1 ? StartPoint : rawPoints[i];
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
    }

    public struct SliderEdge
    {
        public double Offset { get; set; }
        public Vector2 Point { get; set; }
        public HitsoundType EdgeHitsound { get; set; }
        public ObjectSamplesetType EdgeSample { get; set; }
        public ObjectSamplesetType EdgeAddition { get; set; }
    }

    public struct SliderTick
    {
        public SliderTick(double offset, Vector2 point)
        {
            Offset = offset;
            Point = point;
        }

        public double Offset { get; set; }
        public Vector2 Point { get; set; }
    }
}
