using System;
using System.Collections.Generic;
using Coosu.Shared;
using StorybrewCommon.Mapset;
using StorybrewCommon.Storyboarding;

namespace StorybrewScriptTest;

internal class MyContext : GeneratorContext
{
    public override string ProjectPath { get; } = "./demo-project";
    public override string ProjectAssetPath { get; } = "./demo-project/Assert";
    public override string MapsetPath { get; } = "./demo-mapset";
    public override void AddDependency(string path)
    {
    }

    public override void AppendLog(string message)
    {
        Console.WriteLine(message);
    }

    public override Beatmap Beatmap { get; }
    public override IEnumerable<Beatmap> Beatmaps { get; } = EmptyArray<Beatmap>.Value;
    public override StoryboardLayer GetLayer(string identifier)
    {
        return new MyStoryboardLayer(identifier);
    }

    public override double AudioDuration { get; } = 114514;

    public override float[] GetFft(double time, string path = null, bool splitChannels = false)
    {
        return EmptyArray<float>.Value;
    }

    public override float GetFftFrequency(string path = null)
    {
        return 0;
    }
    //public override float[] GetFft(double time, string path = null)
    //{
    //    return EmptyArray<float>.Value;
    //}
}