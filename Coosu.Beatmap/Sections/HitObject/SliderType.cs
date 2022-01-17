using System;

namespace Coosu.Beatmap.Sections.HitObject
{
    public enum SliderType
    {
        Linear, Perfect, Bezier,
        [Obsolete("Catmull style is obsolete for latest version of osu!")]
        Catmull
    }
}
