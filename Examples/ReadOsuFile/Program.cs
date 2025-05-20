using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace ReadOsuFile;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = args.FirstOrDefault() ??
                          "files/Chata & nayuta - Yuune Zekka, Ryouran no Sai (sjoy) [Replay].osu";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }

        try
        {
            Console.WriteLine($"Reading file: {filePath} ...");
            OsuFile osuFile = await OsuFile.ReadFromFileAsync(filePath);
            PrintOsuFileDetails(osuFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or parsing file: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("\nPress any key to exit...");
    }

    static void PrintOsuFileDetails(OsuFile osuFile)
    {
        Console.WriteLine("\n--- File Info ---");
        Console.WriteLine($"Format Version: {osuFile.Version}");

        if (osuFile.General != null)
        {
            Console.WriteLine("\n--- General Section ---");
            var general = osuFile.General;
            Console.WriteLine($"  AudioFilename: {general.AudioFilename ?? "(null)"}");
            Console.WriteLine($"  AudioLeadIn: {general.AudioLeadIn}");
            Console.WriteLine($"  PreviewTime: {general.PreviewTime}");
            Console.WriteLine($"  Countdown: {general.Countdown}");
            Console.WriteLine($"  SampleSet: {general.SampleSet}");
            Console.WriteLine($"  StackLeniency: {general.StackLeniency}");
            Console.WriteLine($"  Mode: {general.Mode}");
            Console.WriteLine($"  LetterboxInBreaks: {general.LetterboxInBreaks}");
            Console.WriteLine($"  UseSkinSprites: {general.UseSkinSprites}");
            Console.WriteLine($"  OverlayPosition: {general.OverlayPosition}");
            Console.WriteLine($"  SkinPreference: {general.SkinPreference ?? "(null)"}");
            Console.WriteLine($"  EpilepsyWarning: {general.EpilepsyWarning}");
            Console.WriteLine($"  CountdownOffset: {general.CountdownOffset}");
            Console.WriteLine($"  SpecialStyle: {general.SpecialStyle}");
            Console.WriteLine($"  WidescreenStoryboard: {general.WidescreenStoryboard}");
            Console.WriteLine($"  SamplesMatchPlaybackRate: {general.SamplesMatchPlaybackRate}");
            Console.WriteLine($"  CustomSamples: {general.CustomSamples}");
            Console.WriteLine($"  SampleVolume: {general.SampleVolume}");
            Console.WriteLine($"  AudioHash: {general.AudioHash ?? "(null)"}");
            Console.WriteLine($"  AlwaysShowPlayfield: {general.AlwaysShowPlayfield}");
            Console.WriteLine($"  TimelineZoom (General): {general.TimelineZoom}");

            if (osuFile.General.UndefinedPairs.Count > 0)
            {
                Console.WriteLine("\n!!Undefined pairs in General section: ");
                foreach (var pair in osuFile.General.UndefinedPairs)
                {
                    Console.WriteLine($"  {pair.Key}: {pair.Value}");
                }
            }
        }

        if (osuFile.Editor != null)
        {
            Console.WriteLine("\n--- Editor Section ---");
            var editor = osuFile.Editor;
            Console.WriteLine($"  Bookmarks: {(editor.Bookmarks != null && editor.Bookmarks.Any() ? string.Join(", ", editor.Bookmarks) : "(none)")}");
            Console.WriteLine($"  DistanceSpacing: {editor.DistanceSpacing}");
            Console.WriteLine($"  BeatDivisor: {editor.BeatDivisor}");
            Console.WriteLine($"  GridSize: {editor.GridSize}");
            Console.WriteLine($"  TimelineZoom (Editor): {editor.TimelineZoom}");

            if (osuFile.Editor.UndefinedPairs.Count > 0)
            {
                Console.WriteLine("\n!!Undefined pairs in Editor section: ");
                foreach (var pair in osuFile.Editor.UndefinedPairs)
                {
                    Console.WriteLine($"  {pair.Key}: {pair.Value}");
                }
            }
        }

        if (osuFile.Metadata != null)
        {
            Console.WriteLine("\n--- Metadata Section ---");
            var metadata = osuFile.Metadata;
            Console.WriteLine($"  Title: {metadata.Title ?? "(null)"}");
            Console.WriteLine($"  TitleUnicode: {metadata.TitleUnicode ?? "(null)"}");
            Console.WriteLine($"  Artist: {metadata.Artist ?? "(null)"}");
            Console.WriteLine($"  ArtistUnicode: {metadata.ArtistUnicode ?? "(null)"}");
            Console.WriteLine($"  Creator: {metadata.Creator ?? "(null)"}");
            Console.WriteLine($"  Version: {metadata.Version ?? "(null)"}");
            Console.WriteLine($"  Source: {metadata.Source ?? "(null)"}");
            Console.WriteLine($"  Tags: {(metadata.TagList != null && metadata.TagList.Any() ? string.Join(" ", metadata.TagList) : "(none)")}");
            Console.WriteLine($"  BeatmapID: {metadata.BeatmapId}");
            Console.WriteLine($"  BeatmapSetID: {metadata.BeatmapSetId}");

            if (osuFile.Metadata.UndefinedPairs.Count > 0)
            {
                Console.WriteLine("\n!!Undefined pairs in Metadata section: ");
                foreach (var pair in osuFile.Metadata.UndefinedPairs)
                {
                    Console.WriteLine($"  {pair.Key}: {pair.Value}");
                }
            }
        }

        if (osuFile.Difficulty != null)
        {
            Console.WriteLine("\n--- Difficulty Section ---");
            var difficulty = osuFile.Difficulty;
            Console.WriteLine($"  HPDrainRate: {difficulty.HpDrainRate}");
            Console.WriteLine($"  CircleSize: {difficulty.CircleSize}");
            Console.WriteLine($"  OverallDifficulty: {difficulty.OverallDifficulty}");
            Console.WriteLine($"  ApproachRate: {difficulty.ApproachRate}");
            Console.WriteLine($"  SliderMultiplier: {difficulty.SliderMultiplier}");
            Console.WriteLine($"  SliderTickRate: {difficulty.SliderTickRate}");

            if (osuFile.Difficulty.UndefinedPairs.Count > 0)
            {
                Console.WriteLine("\n!!Undefined pairs in Difficulty section: ");
                foreach (var pair in osuFile.Difficulty.UndefinedPairs)
                {
                    Console.WriteLine($"  {pair.Key}: {pair.Value}");
                }
            }
        }

        if (osuFile.Events != null)
        {
            Console.WriteLine("\n--- Events Section ---");
            if (osuFile.Events.BackgroundInfo != null)
            {
                Console.WriteLine($"  Background: {osuFile.Events.BackgroundInfo.Filename}, X: {osuFile.Events.BackgroundInfo.X}, Y: {osuFile.Events.BackgroundInfo.Y}");
            }
            else
            {
                Console.WriteLine("  Background: (none)");
            }

            if (osuFile.Events.VideoInfo != null)
            {
                Console.WriteLine($"  Video: {osuFile.Events.VideoInfo.Filename}, Offset: {osuFile.Events.VideoInfo.Offset}ms");
            }
            else
            {
                Console.WriteLine("  Video: (none)");
            }

            Console.WriteLine("  Break Periods:");
            if (osuFile.Events.Breaks != null && osuFile.Events.Breaks.Any())
            {
                foreach (var bp in osuFile.Events.Breaks)
                {
                    Console.WriteLine($"    Start: {bp.StartTime}, End: {bp.EndTime}");
                }
            }
            else
            {
                Console.WriteLine("    (No break periods)");
            }

            Console.WriteLine("  Storyboard (Raw Lines):");
            if (!string.IsNullOrWhiteSpace(osuFile.Events.StoryboardText))
            {
                var lines = osuFile.Events.StoryboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"    Total {lines.Length} storyboard command lines. Example (5 lines):");
                // Print first 5 lines as an example
                foreach (var line in lines.Take(5))
                {
                    Console.WriteLine($"      {line}");
                }
            }
            else
            {
                Console.WriteLine("    (No storyboard)");
            }

            Console.WriteLine("  Storyboard Sound Samples:");
            if (osuFile.Events.Samples != null && osuFile.Events.Samples.Any())
            {
                Console.WriteLine($"    Total {osuFile.Events.Samples.Count} samples:");
                foreach (var sample in osuFile.Events.Samples)
                {
                    Console.WriteLine($"      Offset: {sample.Offset}, File: '{sample.Filename}', Vol: {sample.Volume}");
                }
            }
            else
            {
                Console.WriteLine("    (No storyboard sound samples)");
            }
        }

        if (osuFile.TimingPoints != null && osuFile.TimingPoints.TimingList.Any())
        {
            Console.WriteLine("\n--- Timing Points Section ---");
            Console.WriteLine($"Total {osuFile.TimingPoints.TimingList.Count} timing points:");
            foreach (TimingPoint tp in osuFile.TimingPoints.TimingList)
            {
                Console.WriteLine($"  Offset: {tp.Offset}ms, BPM: {(tp.IsInherit ? "N/A (inherited)" : Math.Round(60000 / tp.Factor, 2))}, " +
                                  $"Factor: {tp.Factor:F2}, Rhythm: {tp.Rhythm}, SampleSet: {tp.TimingSampleset}, " +
                                  $"Volume: {tp.Volume}%, Inherited: {tp.IsInherit}, Kiai: {tp.IsKiai}");
            }
        }
        else
        {
            Console.WriteLine("\n--- Timing Points Section ---");
            Console.WriteLine("  (No timing points)");
        }

        if (osuFile.Colours != null)
        {
            Console.WriteLine("\n--- Colours Section ---");
            var combos = new[]
            {
                osuFile.Colours.Combo1, osuFile.Colours.Combo2, osuFile.Colours.Combo3, osuFile.Colours.Combo4,
                osuFile.Colours.Combo5, osuFile.Colours.Combo6, osuFile.Colours.Combo7, osuFile.Colours.Combo8
            };
            bool hasCombo = false;
            for (int i = 0; i < combos.Length; i++)
            {
                if (combos[i].HasValue)
                {
                    var color = combos[i]!.Value;
                    Console.WriteLine($"  Combo{i + 1}: R={color.X}, G={color.Y}, B={color.Z}");
                    hasCombo = true;
                }
            }
            if (!hasCombo)
            {
                Console.WriteLine("  (No Combo colours)");
            }

            if (osuFile.Colours.SliderTrackOverride.HasValue)
            {
                var color = osuFile.Colours.SliderTrackOverride.Value;
                Console.WriteLine($"  SliderTrackOverride: R={color.X}, G={color.Y}, B={color.Z}");
            }
            if (osuFile.Colours.SliderBorder.HasValue)
            {
                var color = osuFile.Colours.SliderBorder.Value;
                Console.WriteLine($"  SliderBorder: R={color.X}, G={color.Y}, B={color.Z}");
            }

            if (osuFile.Colours.UndefinedPairs.Count > 0)
            {
                Console.WriteLine("\n!!Undefined pairs in Difficulty section: ");
                foreach (var pair in osuFile.Colours.UndefinedPairs)
                {
                    Console.WriteLine($"  {pair.Key}: {pair.Value}");
                }
            }
        }

        if (osuFile.HitObjects != null && osuFile.HitObjects.HitObjectList.Any())
        {
            Console.WriteLine("\n--- Hit Objects Section ---");
            Console.WriteLine($"Total {osuFile.HitObjects.HitObjectList.Count} hit objects:");
            foreach (RawHitObject ho in osuFile.HitObjects.HitObjectList)
            {
                Console.Write($"  Offset: {ho.Offset}ms, X: {ho.X}, Y: {ho.Y}, Type: {ho.ObjectType}, NewCombo: {ho.NewComboCount}");
                if (ho.ObjectType == HitObjectType.Circle)
                {
                    // No extra properties specific to HitCircle shown here, already covered by base
                    Console.WriteLine(", Object: Circle");
                }
                else if (ho.ObjectType == HitObjectType.Slider)
                {
                    var sliderInfo = ho.SliderInfo;
                    if (sliderInfo != null)
                    {
                        Console.WriteLine($", Object: Slider, Repeats: {sliderInfo.Repeat}, Length: {sliderInfo.PixelLength}px, Type: {sliderInfo.SliderType}, Points: {sliderInfo.ControlPoints.Count}");
                    }
                    else
                    {
                        Console.WriteLine(", Object: Slider (Error: SliderInfo is null)");
                    }
                }
                else if (ho.ObjectType == HitObjectType.Spinner)
                {
                    Console.WriteLine($", Object: Spinner, EndTime: {ho.HoldEnd}ms");
                }
                else if (ho.ObjectType == HitObjectType.Hold)
                {
                    Console.WriteLine($", Object: ManiaHold, EndTime: {ho.HoldEnd}ms");
                }
                else
                {
                    Console.WriteLine(", Object: Unknown");
                }
                // Print HitSample related properties
                Console.WriteLine($"    SampleSet: {ho.SampleSet}, AdditionSet: {ho.AdditionSet}, CustomIndex: {ho.CustomIndex}, SampleVolume: {ho.SampleVolume}, Filename: '{ho.FileName ?? "(none)"}'");
            }
        }
        else
        {
            Console.WriteLine("\n--- Hit Objects Section ---");
            Console.WriteLine("  (No hit objects)");
        }
    }
}