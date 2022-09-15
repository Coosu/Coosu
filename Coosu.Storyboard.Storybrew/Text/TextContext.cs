using System.Collections.Generic;

namespace Coosu.Storyboard.Storybrew.Text;

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