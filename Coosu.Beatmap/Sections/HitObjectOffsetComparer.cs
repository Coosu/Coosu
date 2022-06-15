using System.Collections.Generic;
using Coosu.Beatmap.Sections.HitObject;

namespace Coosu.Beatmap.Sections;

public class HitObjectOffsetComparer : IComparer<RawHitObject>
{
    private HitObjectOffsetComparer()
    {
    }

    public static IComparer<RawHitObject> Instance { get; } = new HitObjectOffsetComparer();

    public int Compare(RawHitObject? x, RawHitObject? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return x.Offset.CompareTo(y.Offset);
    }
}