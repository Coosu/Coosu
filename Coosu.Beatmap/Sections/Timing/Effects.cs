using System;

namespace Coosu.Beatmap.Sections.Timing;

[Flags]
public enum Effects : byte
{
    None,
    Kiai = 0b0001,
    OmitFirstBarLine = 0b1000
}