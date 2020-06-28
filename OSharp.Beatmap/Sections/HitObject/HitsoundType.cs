﻿using System;

namespace OSharp.Beatmap.Sections.HitObject
{
    [Flags]
    public enum HitsoundType : short
    {
        Normal = 0x1,
        Whistle = 0x2,
        Finish = 0x4,
        Clap = 0x8,
        Tick = 0x10,
        Slide = 0x20,
        SlideWhistle = 0x40,
        Custom = 0x80
    }
}
