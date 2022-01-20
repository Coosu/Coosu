using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap
{
    public sealed class OsuReadOptions : ReadOptions
    {
        public bool StoryboardIgnored { get; private set; }
        public bool SampleIgnored { get; private set; }
        public bool AutoCompute { get; private set; } = true;

        public void IgnoreStoryboard()
        {
            StoryboardIgnored = true;
        }

        public void IgnoreSample()
        {
            SampleIgnored = true;
        }
    }
}