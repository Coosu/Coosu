using System.Collections.Generic;
using System.Numerics;

namespace Coosu.Storyboard.Storybrew
{
    public class CacheObj
    {
        public Dictionary<string, FontTypeObj> FontIdentifier { get; set; } = new();
    }

    public record FontTypeObj
    {
        public Dictionary<char, Vector2> SizeMapping { get; set; }
        public string Base { get; set; }
        public string Stroke { get; set; }
        public string Shadow { get; set; }
    }
}