using System.Collections.Generic;

namespace Coosu.Beatmap
{
    public sealed class ReadOptions
    {
        public HashSet<string> Include { get; } = new();
        public HashSet<string> Exclude { get; } = new();
        public bool? IncludeMode { get; private set; }
        public bool StoryboardIgnored { get; private set; }
        public bool SampleIgnored { get; private set; }

        public void IncludeSection(string section)
        {
            IncludeMode = true;
            Include.Add(section);
        }

        public void ExcludeSection(string section)
        {
            IncludeMode = false;
            Exclude.Add(section);
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