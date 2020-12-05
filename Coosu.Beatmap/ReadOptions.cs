using System.Collections.Generic;

namespace Coosu.Beatmap
{
    public class ReadOptions
    {
        internal List<string> Include { get; } = new List<string>();
        internal List<string> Exclude { get; } = new List<string>();
        internal bool? IncludeMode { get; private set; } = null;
        internal bool StoryboardIgnored { get; private set; } = false;
        internal bool SampleIgnored { get; private set; } = false;

        public void IncludeSection(params string[] sections)
        {
            IncludeMode = true;
            Include.AddRange(sections);
        }

        public void ExcludeSection(params string[] sections)
        {
            IncludeMode = false;
            Exclude.AddRange(sections);
        }

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