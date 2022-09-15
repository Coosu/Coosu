using System.Collections.Generic;
using Coosu.Shared.Numerics;

namespace Coosu.Storyboard.Storybrew;

public class CacheObj
{
    public Dictionary<string, FontTypeObj> FontIdentifier { get; set; } = new();
}

public record FontTypeObj
{
    public string? Base { get; set; }
    public string? Shadow { get; set; }
    public Dictionary<char, Vector2D>? SizeMapping { get; set; }
    public string? Stroke { get; set; }
}