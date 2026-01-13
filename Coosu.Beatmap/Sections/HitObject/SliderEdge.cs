using System.Numerics;

namespace Coosu.Beatmap.Sections.HitObject;

public readonly struct SliderEdge
{
    public SliderEdge(double offset, Vector3 point,
        HitsoundType edgeHitsound, ObjectSamplesetType edgeSample, ObjectSamplesetType edgeAddition,
        bool isHitsoundDefined)
    {
        Offset = offset;
        Point = point;
        EdgeHitsound = edgeHitsound;
        EdgeSample = edgeSample;
        EdgeAddition = edgeAddition;
        IsHitsoundDefined = isHitsoundDefined;
    }

    public double Offset { get; }
    public Vector3 Point { get; }
    public HitsoundType EdgeHitsound { get; }
    public ObjectSamplesetType EdgeSample { get; }
    public ObjectSamplesetType EdgeAddition { get; }
    public bool IsHitsoundDefined { get; }
}