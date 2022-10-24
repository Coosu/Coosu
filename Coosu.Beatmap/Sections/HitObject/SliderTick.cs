using System.Numerics;

namespace Coosu.Beatmap.Sections.HitObject;

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