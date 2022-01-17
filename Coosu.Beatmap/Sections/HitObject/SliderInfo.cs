using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.HitObject
{
    public class SliderInfo
    {
        public SliderType SliderType { get; set; }
        public Vector2[] CurvePoints { get; set; }
        public int Repeat { get; set; }
        public decimal PixelLength { get; set; }
        public HitsoundType[] EdgeHitsounds { get; set; }
        public ObjectSamplesetType[] EdgeSamples { get; set; }
        public ObjectSamplesetType[] EdgeAdditions { get; set; }

        //extension
        public Vector2 StartPoint { get; }
        public Vector2 EndPoint => CurvePoints.Last();

        public double StartTime => _offset;
        public double EndTime => Edges[Edges.Length - 1].Offset;

        private double _singleElapsedTime;
        private SliderEdge[] _edges;
        private SliderTick[] _ticks;
        private SliderTick[] _ballTrail;
        private List<List<Vector2>>? _rawBezierData;
        private List<double>? _rawBezierLengthData;

        private readonly int _offset;
        private readonly double _beatDuration;
        private readonly double _sliderMultiplier;
        private readonly double _tickRate;

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

        private (int index, double lenInPart) CalculateWhichPart(double relativeLen)
        {
            double sum = 0;
            for (var i = 0; i < RawBezierLengthData.Count; i++)
            {
                var len = RawBezierLengthData[i];
                sum += len;
                if (relativeLen < sum) return (i, len - (sum - relativeLen));
            }

            return (-1, -1);
        }

        // todo: not cut by rhythm
        // todo: i forget math
        private SliderTick[] GetPerfectDiscreteBallData(double interval)
        {
            if (Math.Round(interval - _singleElapsedTime) >= 0)
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
                return GetBezierDiscreteBallData(interval);
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

            for (int i = 1; i * interval < _singleElapsedTime; i++)
            {
                var offset = i * interval; // 当前tick的相对时间
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
        private SliderTick[] GetBezierDiscreteBallData(double interval)
        {
            if (Math.Round(interval - _singleElapsedTime) >= 0)
            {
                return EmptyArray<SliderTick>.Value;
            }

            var totalLength = RawBezierLengthData.Sum();
            var ticks = new List<SliderTick>();

            for (int i = 1; i * interval < _singleElapsedTime; i++)
            {
                var offset = i * interval; // 当前tick的相对时间
                if (Edges.Any(k => Math.Abs(k.Offset - _offset - offset) < 0.01))
                    continue;

                var ratio = offset / _singleElapsedTime; // 相对整个滑条的时间比例，=距离比例
                var relativeLen = totalLength * ratio; // 至滑条头的距离

                var (index, lenInPart) = CalculateWhichPart(relativeLen); // can be optimized
                var len = RawBezierLengthData[index];
                var tickPoint = BezierHelper.Compute(RawGroupedBezierData[index], (float)(lenInPart / len));
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

        private List<List<Vector2>> RawGroupedBezierData => _rawBezierData ??= GetGroupedBezier();

        private List<double> RawBezierLengthData => _rawBezierLengthData ??= GetBezierLengths(RawGroupedBezierData);

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

        private static List<double> GetBezierLengths(List<List<Vector2>> value)
        {
            var list = new List<double>();
            foreach (var controlPoints in value)
            {
                var points = BezierHelper.GetBezierTrail(controlPoints, 20);
                double dis = 0;
                if (points.Count <= 1)
                {
                }
                else
                {
                    for (int j = 0; j < points.Count - 1; j++)
                    {
                        dis += Math.Pow(
                            Math.Pow(points[j].X - points[j + 1].X, 2) +
                            Math.Pow(points[j].Y - points[j + 1].Y, 2),
                            0.5);
                    }
                }

                list.Add(dis);
            }

            return list;
        }

        private List<List<Vector2>> GetGroupedBezier()
        {
            var copiedCurvePoints = CurvePoints.ToList();
            copiedCurvePoints.Insert(0, StartPoint);

            var list = new List<List<Vector2>>();
            var current = new List<Vector2>();
            list.Add(current);
            for (int i = 0; i < copiedCurvePoints.Count; i++)
            {
                var @this = copiedCurvePoints[i];
                current.Add(@this);
                if (i == copiedCurvePoints.Count - 1)
                {
                    break;
                }

                var next = copiedCurvePoints[i + 1];

                if (Math.Abs(@this.X - next.X) < 0.01 && Math.Abs(@this.Y - next.Y) < 0.01)
                {
                    current = new List<Vector2>();
                    list.Add(current);
                }
            }

            return list;
        }
    }

    public class ReversableList<T>
    {
        private (int index, T element)? _current;
        private readonly List<T> _list;
        //private readonly bool _initialReverse;
        private bool _currentReverse = false;
        public ReversableList(List<T> list/*, bool initialReverse = false*/)
        {
            _list = list;
            //_initialReverse = initialReverse;
        }

        public (int index, T element) GetNext()
        {
            if (_current == null)
            {
                _current = (0, _list[0]);
                _currentReverse = false;

                return _current.Value;
            }
            else
            {
                if (!_currentReverse && _current.Value.index == _list.Count - 1)
                {
                    _currentReverse = false;
                    var index = _list.Count - 1;
                    _current = (index, _list[index]);
                }
                else if (_currentReverse && _current.Value.index == 0)
                {
                    _currentReverse = true;
                    _current = (0, _list[0]);
                }
                else
                {
                    var index = _currentReverse ? _current.Value.index - 1 : _current.Value.index + 1;
                    _current = (index, _list[index]);
                }

                return _current.Value;
            }
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
