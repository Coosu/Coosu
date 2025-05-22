using System;
using System.Collections.Generic;

namespace Coosu.Beatmap.Configurable;

public class ReadOptions
{
    public HashSet<string> Include { get; } = new();
    public HashSet<string> Exclude { get; } = new();
    public bool? IncludeMode { get; private set; }

    public void IncludeSection(string section)
    {
        IncludeMode = true;
        Include.Add(section);
    }

    public void IncludeSections(params string[] sections)
    {
        foreach (var section in sections)
        {
            IncludeSection(section);
        }
    }

    public void ExcludeSection(string section)
    {
        IncludeMode = false;
        Exclude.Add(section);
    }

    public void ExcludeSections(params string[] sections)
    {
        foreach (var section in sections)
        {
            ExcludeSection(section);
        }
    }
}