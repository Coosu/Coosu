using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections.HitObject;

public class SliderInfo : SerializeWritableObject
{
    public SliderInfo(RawHitObject baseObject)
    {
        BaseObject = baseObject;
    }

    public SliderType SliderType { get; set; }
    public IReadOnlyList<Vector2> ControlPoints { get; set; } = EmptyArray<Vector2>.Value;
    public int Repeat { get; set; }
    public double PixelLength { get; set; }
    public HitsoundType[]? EdgeHitsounds { get; set; }
    public ObjectSamplesetType[]? EdgeSamples { get; set; }
    public ObjectSamplesetType[]? EdgeAdditions { get; set; }

    public Vector2 StartPoint { get; set; }
    public Vector2 EndPoint => ControlPoints[ControlPoints.Count - 1];

    public int StartTime { get; set; }

    public RawHitObject BaseObject { get; }

    public override void AppendSerializedString(TextWriter textWriter, int version)
    {
        textWriter.Write(SliderType.ToSliderFlag());
        textWriter.Write('|');
        for (var i = 0; i < ControlPoints.Count; i++)
        {
            var controlPoint = ControlPoints[i];
            textWriter.Write(controlPoint.X.ToString(ParseHelper.EnUsNumberFormat));
            textWriter.Write(':');
            textWriter.Write(controlPoint.Y.ToString(ParseHelper.EnUsNumberFormat));
            if (i < ControlPoints.Count - 1)
            {
                textWriter.Write('|');
            }
        }

        textWriter.Write(',');
        textWriter.Write(Repeat);
        textWriter.Write(',');
        textWriter.Write(PixelLength.ToString(ParseHelper.EnUsNumberFormat));
        textWriter.Write(',');
        for (var i = 0; i < Repeat + 1; i++)
        {
            var edgeHitsound = EdgeHitsounds?[i] ?? BaseObject.Hitsound;
            textWriter.Write((byte)edgeHitsound);
            if (i < Repeat)
            {
                textWriter.Write('|');
            }
        }

        textWriter.Write(',');
        for (var i = 0; i < Repeat + 1; i++)
        {
            var edgeSample = EdgeSamples?[i] ?? BaseObject.SampleSet;
            var edgeAddition = EdgeAdditions?[i] ?? BaseObject.AdditionSet;
            textWriter.Write((byte)edgeSample);
            textWriter.Write(':');
            textWriter.Write((byte)edgeAddition);
            if (i < Repeat)
            {
                textWriter.Write('|');
            }
        }
    }
}