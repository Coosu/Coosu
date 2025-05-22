using System;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.GamePlay;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace ReadOsuFile;

public class BeatmapCreator
{
    public async Task CreateNewBeatmapAsync(string outputPath)
    {
        OsuFile osuFile = OsuFile.CreateEmpty(); // Creates a beatmap with osu! file format v14 and default sections

        // Populate General section
        osuFile.General.AudioFilename = "audio.mp3";
        osuFile.General.Mode = GameMode.Circle;
        osuFile.General.PreviewTime = 15000;

        // Populate Metadata section
        osuFile.Metadata.Title = "My Awesome Beatmap";
        osuFile.Metadata.Artist = "Amazing Artist";
        osuFile.Metadata.Creator = "You";
        osuFile.Metadata.Version = "Normal";

        // Populate Difficulty section
        osuFile.Difficulty.HpDrainRate = 5;
        osuFile.Difficulty.CircleSize = 4;
        osuFile.Difficulty.OverallDifficulty = 6;
        osuFile.Difficulty.ApproachRate = 8;

        // Add a timing point (120 BPM, 4/4 signature)
        osuFile.TimingPoints.TimingList.Add(new TimingPoint
        {
            Offset = 0,
            Factor = 60000.0 / 120, // Milliseconds per beat for 120 BPM
            Rhythm = 4,
            TimingSampleset = TimingSamplesetType.Normal,
            Volume = 70,
            Effects = Effects.Kiai
        });

        // Add a hit circle
        osuFile.HitObjects.HitObjectList.Add(new RawHitObject
        {
            X = 256,
            Y = 192,
            Offset = 1000,
            RawType = RawObjectType.Circle,
            Hitsound = HitsoundType.Normal,
            // Addition: null, // No additions for a simple circle
            // Extras: new HitObjectExtras // Or provide default extras
            // {
            //    SampleSet = SampleSet.Normal,
            //    AdditionSampleSet = SampleSet.Normal,
            //    CustomIndex = 0,
            //    Volume = 0, // Use timing point volume
            //    HitsoundFile = null
            // }
        });

        // Save the beatmap to a file
        osuFile.Save(outputPath);
        Console.WriteLine($"New beatmap created at: {outputPath}");
    }
}