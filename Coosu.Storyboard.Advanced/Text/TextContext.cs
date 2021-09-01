using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Advanced.Text
{
    public class TextContext
    {
        public string CachePath { get; set; }
        public string BeatmapsetDir { get; set; }

        public IList<char> Text { get; set; }
        public CoosuTextOptions TextOptions { get; set; }
        public SpriteGroup SpriteGroup { get; set; }
        public double StartTime { get; set; }
        public LayerType Layer { get; set; }
    }
}
