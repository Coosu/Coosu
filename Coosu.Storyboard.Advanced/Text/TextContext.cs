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
        public string BeatmapsetPath { get; set; }

        public string Text { get; set; }
        public CoosuTextOptions TextOptions { get; set; }
    }
}
