using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.HitObject
{
    public class SliderInfo : SerializeWritableObject
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
        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write(SliderType.ToSliderFlag());
            textWriter.Write('|');
            for (var i = 0; i < ControlPoints.Count; i++)
            {
                var controlPoint = ControlPoints[i];
                textWriter.Write(controlPoint.X.ToIcString());
                textWriter.Write(':');
                textWriter.Write(controlPoint.Y.ToIcString());
                if (i < ControlPoints.Count - 1)
                    textWriter.Write('|');
            }

            textWriter.Write(',');
            textWriter.Write(Repeat);
            textWriter.Write(',');
            textWriter.Write(PixelLength.ToIcString());
            if (EdgeHitsounds == null)
                return;

            textWriter.Write(',');
            for (var i = 0; i < EdgeHitsounds.Length; i++)
            {
                var edgeHitsound = EdgeHitsounds[i];
                textWriter.Write((byte)edgeHitsound);
                if (i < EdgeHitsounds.Length - 1)
                    textWriter.Write('|');
            }

            if (EdgeSamples == null || EdgeAdditions == null)
                return;

            textWriter.Write(',');
            for (var i = 0; i < EdgeSamples.Length; i++)
            {
                var edgeSample = EdgeSamples[i];
                var edgeAddition = EdgeAdditions[i];
                textWriter.Write((byte)edgeSample);
                textWriter.Write(':');
                textWriter.Write((byte)edgeAddition);
                if (i < EdgeSamples.Length - 1)
                    textWriter.Write('|');
            }
        }
    }

    public sealed class SliderEdge
    {
        public double Offset { get; set; }
        public Vector2 Point { get; set; }
        public HitsoundType EdgeHitsound { get; set; }
        public ObjectSamplesetType EdgeSample { get; set; }
        public ObjectSamplesetType EdgeAddition { get; set; }
    }

    public readonly struct SliderTick
    {
        public SliderTick(double offset, in Vector2 point)
        {
            Offset = offset;
            Point = point;
        }

        public double Offset { get; }
        public Vector2 Point { get; }
    }
}
