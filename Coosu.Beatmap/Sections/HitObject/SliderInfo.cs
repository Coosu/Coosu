using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Sections.HitObject
{
    public class SliderInfo
    {
        public SliderType SliderType { get; set; }
        public IReadOnlyList<Vector2> ControlPoints { get; set; }
        public int Repeat { get; set; }
        public double PixelLength { get; set; }
        public HitsoundType[]? EdgeHitsounds { get; set; }
        public ObjectSamplesetType[]? EdgeSamples { get; set; }
        public ObjectSamplesetType[]? EdgeAdditions { get; set; }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint => ControlPoints[ControlPoints.Count - 1];

        public int StartTime { get; set; }

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
                string.Join("|", ControlPoints.Select(k => $"{k.X}:{k.Y}")),
                Repeat,
                PixelLength,
                edgeHitsoundStr,
                edgeSampleStr);
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
