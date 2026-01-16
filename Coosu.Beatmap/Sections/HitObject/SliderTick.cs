using System.Numerics;

namespace Coosu.Beatmap.Sections.HitObject;

public readonly struct SliderTick
{
    public SliderTick(double offset, in Vector3 point)
    {
        Offset = offset;
        Point = point;
    }

    public double Offset { get; }
    public Vector3 Point { get; }
}