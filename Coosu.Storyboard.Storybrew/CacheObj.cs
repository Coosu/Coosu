using System.Collections.Generic;

namespace Coosu.Storyboard.Advanced
{
    public class CacheObj
    {
        public Dictionary<string, FontTypeObj> FontIdentifier { get; set; } = new();
    }

    public record FontTypeObj
    {
        public string Base { get; set; }
        public string Stroke { get; set; }
        public string Shadow { get; set; }
    }
}