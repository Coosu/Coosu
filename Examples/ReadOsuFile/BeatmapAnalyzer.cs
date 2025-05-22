using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.Event;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;

namespace ReadOsuFile;

public class BeatmapAnalyzer
{
    public async Task AnalyzeOsuFileAsync(string osuFilePath)
    {
        // Read the .osu file
        // You can specify options to include/exclude certain sections for performance.
        OsuFile osuFile = await OsuFile.ReadFromFileAsync(osuFilePath, options =>
        {
            options.ExcludeSections("Events", "Colours"); // Ignores [Events] and [Colours] sections
            // or
            //options.IgnoreStoryboard();
            //options.IgnoreSample();
            // or
            //options.IncludeSections("General", "Metadata", "HitObjects"); // Only parse these sections

            // Example: Ignore storyboard and sample data
            //options.IgnoreStoryboard();
            //options.IgnoreSample();
            // Example: Only include General, Metadata, and Events sections
            //options.IncludeSection("General");
            //options.IncludeSection("Metadata");
            //options.IncludeSection("Events");
        });

        // Access different sections of the beatmap:

        // [General]
        if (osuFile.General != null)
        {
            GeneralSection general = osuFile.General;
            Console.WriteLine($"Audio Filename: {general.AudioFilename}");
            Console.WriteLine($"Mode: {general.Mode}");
            Console.WriteLine($"Preview Time: {general.PreviewTime}");
        }

        // [Metadata]
        if (osuFile.Metadata != null)
        {
            MetadataSection metadata = osuFile.Metadata;
            Console.WriteLine($"Title: {metadata.TitleMeta.ToPreferredString()}"); // Shows Unicode title if available, otherwise ASCII
            Console.WriteLine($"Artist: {metadata.ArtistMeta.ToPreferredString()}");
            Console.WriteLine($"Creator: {metadata.Creator}");
            Console.WriteLine($"Version: {metadata.Version}");
        }

        // [Difficulty]
        if (osuFile.Difficulty != null)
        {
            DifficultySection difficulty = osuFile.Difficulty;
            Console.WriteLine($"HP Drain Rate: {difficulty.HpDrainRate}");
            Console.WriteLine($"Circle Size: {difficulty.CircleSize}");
            Console.WriteLine($"Overall Difficulty: {difficulty.OverallDifficulty}");
            Console.WriteLine($"Approach Rate: {difficulty.ApproachRate}");
        }

        // [Events] - Storyboard and Samples
        if (osuFile.Events != null)
        {
            EventSection events = osuFile.Events;

            BackgroundData? bgInfo = events.BackgroundInfo;
            VideoData? videoInfo = events.VideoInfo;
            List<StoryboardSampleData> samples = events.Samples;

            // Access storyboard sprites, animations, samples etc.
            // Note: For detailed storyboard manipulation, consider using Coosu.Storyboard library.
            string? storyboardText = events.StoryboardText;
        }

        // [TimingPoints]
        if (osuFile.TimingPoints != null)
        {
            TimingSection timings = osuFile.TimingPoints;
            if (timings.TimingList.Count > 0)
            {
                TimingPoint firstTimingPoint = timings.TimingList[0];
                Console.WriteLine($"First Timing Point Offset: {firstTimingPoint.Offset}, BPM: {firstTimingPoint.Bpm}, Inherited: {firstTimingPoint.IsInherit}");

                // Get timing point at a specific offset
                TimingPoint specificTimingPoint = timings.GetLine(12345); // Gets the timing point active at 12345ms
                if (specificTimingPoint != null)
                {
                    Console.WriteLine($"Timing at 12345ms: BPM: {specificTimingPoint.Bpm}");
                }

                // Get all Kiai time ranges
                RangeValue<double>[] kiaiRanges = timings.GetTimingKiais();
                foreach (RangeValue<double> kiai in kiaiRanges)
                {
                    Console.WriteLine($"Kiai: {kiai.StartTime}ms - {kiai.EndTime}ms");
                }
            }
        }

        // [Colours]
        if (osuFile.Colours != null)
        {
            ColorSection colors = osuFile.Colours;
            if (colors.Combo1.HasValue)
            {
                Console.WriteLine($"Combo1 Color: R={colors.Combo1.Value.X}, G={colors.Combo1.Value.Y}, B={colors.Combo1.Value.Z}");
            }
        }

        // [HitObjects]
        if (osuFile.HitObjects != null)
        {
            List<RawHitObject> hitObjectList = osuFile.HitObjects.HitObjectList;
            Console.WriteLine($"Total Hit Objects: {hitObjectList.Count}");
            if (hitObjectList.Count > 0)
            {
                RawHitObject firstHitObject = hitObjectList[0];
                Console.WriteLine($"First Hit Object: Time: {firstHitObject.Offset}, X: {firstHitObject.X}, Y: {firstHitObject.Y}, Type: {firstHitObject.ObjectType}");

                if (firstHitObject.ObjectType == HitObjectType.Slider)
                {
                    SliderInfo sliderInfo = firstHitObject.SliderInfo;
                    Console.WriteLine($"  Slider Type: {sliderInfo.SliderType}, Repeat: {sliderInfo.Repeat}, Length: {sliderInfo.PixelLength}");
                    // Access slider curve points, edge hitsounds, etc.
                }
            }
        }
    }
}