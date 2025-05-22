# Coosu.Beatmap

[![NuGet](https://img.shields.io/nuget/v/Coosu.Beatmap.svg)](https://www.nuget.org/packages/Coosu.Beatmap/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Beatmap.svg)](https://www.nuget.org/packages/Coosu.Beatmap/)

**Coosu.Beatmap** is a powerful and efficient .NET library for parsing, manipulating, and creating osu! beatmap files (`.osu`). It provides a comprehensive API to access all sections of a beatmap, including general information, metadata, difficulty settings, timing points, hit objects, events, and colors.

## Features

*   **Full Beatmap Parsing**: Read and parse all sections of `.osu` files.
*   **Object-Oriented Model**: Access beatmap data through a clear and well-structured object model.
*   **High Performance**: Optimized for speed and low memory usage, especially for large beatmaps. (See [Performance](#performance) section for benchmarks).
*   **Data Calculation**: Includes utility functions for common calculations, such as:
    *   Retrieving timing information (BPM, kiai time, timing points at specific offsets, bar lines). (See [Timing Calculations](#timing-calculations))
    *   Analyzing slider data (edges, ticks, ball trails).
    *   **Hitsound Analysis**: Determine actual sound files to be played for each hit object and sample, considering timing sections, object-specific samples, and game mode specifics. (See [Hitsound Analysis](#hitsound-analysis))
*   **Modifiable Beatmap Data**: Modify beatmap properties and save changes back to a `.osu` file.
*   **Extensible**: Designed to be extensible for custom data handling and advanced use cases.
*   **Read/Write Support**: Supports both reading existing beatmap files and creating new ones from scratch.
*   **Selective Parsing**: Option to include or exclude specific sections during parsing for improved performance when only partial data is needed.
*   **AOT Friendly**: Designed to be AOT (Ahead-of-Time compilation) friendly, with `IsTrimmable` enabled for .NET 8.0+ targets, allowing for smaller application sizes when used in AOT-compiled projects.

## Installation

You can install Coosu.Beatmap via NuGet Package Manager:

```bash
Install-Package Coosu.Beatmap
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Beatmap
```

## Usage

Here's a basic example of how to read an osu! beatmap file and access its data:
For detailed example, see [Examples/ReadOsuFile/Program.cs](../Examples/ReadOsuFile/Program.cs).

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.Event;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;

namespace ReadOsuFile
{
    public class BeatmapAnalyzer
    {
        public async Task AnalyzeOsuFileAsync(string osuFilePath)
        {
            // Read the .osu file
            // You can specify options to include/exclude certain sections for performance.
            OsuFile osuFile = await OsuFile.ReadFromFileAsync(osuFilePath, options =>
            {
                // Example: Ignore storyboard and sample data
                // options.IgnoreStoryboard();
                // options.IgnoreSample();
                // Example: Only include General, Metadata, and Events sections
                // options.IncludeSection("General");
                // options.IncludeSection("Metadata");
                // options.IncludeSection("Events");
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
                    else if(firstHitObject.ObjectType == HitObjectType.Hold) // mania slider
                    {
                        int endTime = hitObject.HoldEnd;
                    }
                }
            }
        }
    }
}
```

### Creating a new Beatmap

```csharp
using System;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.GamePlay;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace ReadOsuFile
{
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
}
```

## Project Structure

The `Coosu.Beatmap` library is organized into several key namespaces and directories:

*   **`Coosu.Beatmap` (Root Namespace)**: Contains core classes like `OsuFile` for reading and writing beatmap files, and `OsuDirectory` for managing multiple beatmaps within a folder.
*   **`Coosu.Beatmap.Sections`**: Defines classes representing each section of an `.osu` file:
    *   `GeneralSection`: Contains general information about the beatmap (audio filename, mode, etc.).
    *   `EditorSection`: Stores editor-specific settings (bookmarks, distance spacing, etc.).
    *   `MetadataSection`: Holds metadata like title, artist, creator, and version.
    *   `DifficultySection`: Defines difficulty settings (HP, CS, OD, AR, SliderMultiplier, SliderTickRate).
    *   `EventSection`: Manages storyboard elements (sprites, animations, samples) and background/video events.
    *   `TimingSection`: Handles timing points (BPM changes, SV changes, kiai time).
    *   `ColorSection`: Stores combo colors and slider track/border colors.
    *   `HitObjectSection`: Contains all hit objects (circles, sliders, spinners).
*   **`Coosu.Beatmap.Sections.HitObject`**: Provides detailed classes for different types of hit objects (`RawHitObject`, `SliderInfo`, `SpinnerInfo`, `HoldInfo`) and their properties.
*   **`Coosu.Beatmap.Sections.Timing`**: Includes classes related to timing points like `TimingPoint`.
*   **`Coosu.Beatmap.Configurable`**: Internal components responsible for parsing and serializing `.osu` file content based on attributes and section definitions.
*   **`Coosu.Beatmap.Extensions`**: Offers extension methods for easier manipulation and calculation on beatmap objects (e.g., `TimingExtensions`, `SliderExtensions`).
*   **`Coosu.Beatmap.Utils`**: Contains utility classes for various calculations and operations, such as path approximation (`PathApproximator`) and Bézier curve generation (`BezierHelper`).
*   **`Coosu.Beatmap.Internal`**: Houses internal helper classes and extension methods used within the library.

## API Overview

The primary entry point for interacting with the library is the `OsuFile` class.

### Reading a Beatmap

```csharp
using Coosu.Beatmap;
using System.Threading.Tasks;

// ...

// Asynchronously read from a file path
OsuFile osuFile = await OsuFile.ReadFromFileAsync("path/to/your/beatmap.osu");

// Or from a Stream
// Stream stream = ...;
// OsuFile osuFileFromStream = await OsuFile.ReadFromStreamAsync(stream);

// Access sections
// var generalInfo = osuFile.General;
// var hitObjects = osuFile.HitObjects;
```

You can use `OsuReadOptions` to customize the parsing process, for example, by ignoring certain sections:

```csharp
OsuFile osuFileSelective = await OsuFile.ReadFromFileAsync(osuFilePath, options =>
{
    options.ExcludeSections("Events", "Colours"); // Ignores [Events] and [Colours] sections
    // or
    // options.IgnoreStoryboard();
    // options.IgnoreSample();
    // or
    // options.IncludeSections("General", "Metadata", "HitObjects"); // Only parse these sections
});
```

### Modifying a Beatmap

All properties of the `OsuFile` object and its sections are modifiable.

```csharp
// Change the beatmap title
osuFile.Metadata.Title = "My New Beatmap Title";
osuFile.Metadata.TitleUnicode = "我的新谱面标题"; // If Unicode is preferred

// Add a new timing point
osuFile.TimingPoints.TimingList.Add(new Coosu.Beatmap.Sections.Timing.TimingPoint
{
    Offset = 5000,
    Factor = -100, // SV change (0.5x)
    IsInherit = false // This is a green line
});

// Remove the first hit object
if (osuFile.HitObjects.HitObjectList.Count > 0)
{
    osuFile.HitObjects.HitObjectList.RemoveAt(0);
}
```

### Writing a Beatmap

```csharp
// Save changes back to the original file or a new file
await osuFile.WriteToFileAsync("path/to/your/modified_beatmap.osu");

// Or write to a Stream
// Stream outputStream = ...;
// await osuFile.WriteToStreamAsync(outputStream);
```

### Creating a Beatmap from Scratch

The `OsuFile.CreateEmpty()` method provides a blank beatmap structure that you can populate.

```csharp
OsuFile newBeatmap = OsuFile.CreateEmpty(); // Default osu file format v14
// Or specify a version: OsuFile newBeatmap = OsuFile.CreateEmpty(10);

newBeatmap.General.AudioFilename = "song.mp3";
newBeatmap.Metadata.Title = "My First Map";
// ... populate other sections and hit objects ...

newBeatmap.Save("my_new_map.osu");
```

### Working with `OsuDirectory`

`OsuDirectory` helps manage multiple `.osu` files within a game songs folder.

```csharp
using Coosu.Beatmap;
using System.Threading.Tasks;

// ...

// Load all beatmaps from a directory
// OsuDirectory osuDir = await OsuDirectory.ReadFromDirectoryAsync("path/to/osu/Songs/MySongFolder");

// Load all beatmaps including subfolders (recursive)
// Note: OsuDirectory.ReadFromDirectoryAsync will read all .osu files and also list all .wav, .mp3, .ogg files in the top directory by default.
// To ignore audio files during initialization for faster loading if not needed for hitsound analysis:
// OsuDirectory osuDir = await OsuDirectory.ReadFromDirectoryAsync("path/to/osu/Songs", true); // last parameter ignoreWaveFiles
// Or initialize later:
OsuDirectory osuDir = new OsuDirectory("path/to/osu/Songs");
await osuDir.InitializeAsync(); // This will load .osu and audio files.
// await osuDir.InitializeAsync(ignoreWaveFiles: true); // To ignore audio files.


foreach (var osuFile in osuDir.OsuFiles)
{
    Console.WriteLine($"Loaded: {osuFile.Metadata?.TitleMeta.ToPreferredString()} [{osuFile.Metadata?.Version}]");
}
```

## Advanced Calculations

Coosu.Beatmap provides several extension methods for more advanced calculations.

### Timing Calculations

The `Coosu.Beatmap.TimingExtensions` static class offers methods to work with `TimingSection` data:

```csharp
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.Timing;
using System.Linq;

// ... assuming 'osuFile' is a loaded OsuFile object
TimingSection timingSection = osuFile.TimingPoints;

// Get the timing point (red or green line) active at a specific offset
TimingPoint currentTimingPoint = timingSection.GetLine(15000); // Get timing at 15000ms
Console.WriteLine($"Effective BPM at 15000ms: {currentTimingPoint.Bpm}");

// Get the last red line (uninherited timing point) before or at a specific offset
TimingPoint redLine = timingSection.GetRedLine(15000);
Console.WriteLine($"Underlying BPM at 15000ms: {redLine.Bpm}");

// Get all Kiai time ranges
var kiaiRanges = timingSection.GetTimingKiais();
foreach (var kiai in kiaiRanges)
{
    Console.WriteLine($"Kiai active from {kiai.StartTime}ms to {kiai.EndTime}ms");
}

// Get all bar line offsets (based on red lines and their rhythms)
double[] barLines = timingSection.GetTimingBars();
// foreach (double barOffset in barLines) { /* ... */ }

// Get all timing points based on a specific beat divisor (e.g., 1/4th notes)
// This would generate timestamps for every 1/4th beat across the map according to current BPM and red lines.
double beatDivisor = 1.0 / 4.0; // For 1/4th notes
double[] quarterNotesTimings = timingSection.GetTimings(beatDivisor);
// foreach (double tickOffset in quarterNotesTimings) { /* ... */ }

// Get beat interval for specific BPM sections based on a beat divisor
// Returns a dictionary where Key is the offset of the red line and Value is the interval in ms.
var beatIntervals = timingSection.GetInterval(beatDivisor);
foreach(var kvp in beatIntervals) { Console.WriteLine($"Offset: {kvp.Key}, Interval: {kvp.Value}"); }
```

### Hitsound Analysis

The `OsuDirectory` class provides a method to analyze and list all effective hitsounds for a given beatmap within its song folder context.
This is useful for audio playback or detailed analysis of which sound files are triggered at what times.

```csharp
using Coosu.Beatmap;
using Coosu.Beatmap.Extensions.Playback; // For HitsoundNode
using System.Collections.Generic;
using System.Threading.Tasks;

// ...

// Assuming 'osuDir' is an initialized OsuDirectory object from previous example
// And 'someOsuFileInDir' is an OsuFile object belonging to that directory
OsuFile someOsuFileInDir = osuDir.OsuFiles.FirstOrDefault();
if (someOsuFileInDir != null)
{
    // Ensure sliders are computed if you haven't called it before and need accurate slider tick/edge sounds
    // osuFile.HitObjects.ComputeSlidersByCurrentSettings(); // Already called within GetHitsoundNodesAsync

    List<HitsoundNode> hitsoundNodes = await osuDir.GetHitsoundNodesAsync(someOsuFileInDir);

    foreach (HitsoundNode node in hitsoundNodes)
    {
        if (node is PlayableNode playableNode) // Actual sound to be played
        {
            Console.WriteLine($"Play: {playableNode.Filename} at {playableNode.Offset}ms, Vol: {playableNode.Volume}, Bal: {playableNode.Balance}, Priority: {playableNode.PlayablePriority}");
        }
        else if (node is ControlNode controlNode) // Control signals for looped sounds (like slider slides)
        {
            Console.WriteLine($"Control: Type={controlNode.ControlType} at {controlNode.Offset}ms, Channel={controlNode.SlideChannel}, File={controlNode.Filename}");
        }
    }
}
```

**Key aspects of Hitsound Analysis:**

*   **`HitsoundNode`**: The base class for results. Can be a `PlayableNode` (an actual sound to play) or `ControlNode` (signals for starting/stopping/modifying looped sounds like slider slides and whistles).
*   **Contextual Resolution**: Considers the beatmap's `TimingPoints` (for default samplesets and custom sample indices), `HitObjects` (their specific hitsound flags, samplesets, and custom filenames), and `Events` (for storyboarded samples).
*   **Game Mode Aware**: Handles differences in hitsound behavior for osu!standard, Taiko, and Mania (e.g., ignoring base hitsounds in Mania, ignoring balance in Taiko).
*   **File Resolution**: Uses `HitsoundFileCache` to find the best matching audio file (`.wav`, `.mp3`, `.ogg`) in the song folder, respecting custom sample indices (e.g., `normal-hitnormal2.wav`). It also determines if a sound comes from the user's skin or the map's folder.
*   **Slider Sounds**: Breaks down slider sounds into:
    *   Edge hitsounds (start, repeats, end).
    *   Slider tick sounds.
    *   Continuous slider slide and slider whistle sounds (represented by start/stop `ControlNode`s).

## Performance

Coosu.Beatmap is designed with performance in mind. It utilizes techniques like:

*   `Span<T>` and `ReadOnlySpan<T>` for efficient string and memory operations.
*   Optimized parsing routines.
*   Lazy loading where applicable.
*   Selective section parsing.

Below are some benchmark results from previous tests. Please note that these results were captured in a specific environment and may vary on different hardware or with newer .NET runtimes. For the most up-to-date figures relevant to your system, we recommend running the benchmarks yourself.

**How to Run Benchmarks:**

The benchmark projects are located in the `Benchmarks/` directory of the main Coosu repository.
1.  Navigate to the specific benchmark project directory (e.g., `Benchmarks/ParsingPerformanceTest` or `Benchmarks/WritingOsuBenchmark`).
2.  Execute the corresponding PowerShell script (e.g., `benchmark-ParsingPerformance.ps1` or `benchmark-WritingOsu.ps1`).

### Parsing Benchmark

These benchmarks measure the time taken to parse a standard `.osu` file.
(Executed using `benchmark-ParsingPerformance.ps1` in `Benchmarks/ParsingPerformanceTest`)

For detailed reports, see the [full benchmark results in the repository](../Benchmarks/ParsingPerformanceTest/BenchmarkDotNet.Artifacts/results/).

### Writing Benchmark

These benchmarks measure the time taken to write an `OsuFile` object back to a string.
(Executed using `benchmark-WritingOsu.ps1` in `Benchmarks/WritingOsuBenchmark`)

For detailed reports, see the [full benchmark results in the repository](../Benchmarks/WritingOsuBenchmark/BenchmarkDotNet.Artifacts/results/).

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1.  **Fork the repository.**
2.  **Create a new branch** for your feature or bug fix:
    ```bash
    git checkout -b feature/your-feature-name
    ```
    or
    ```bash
    git checkout -b fix/your-bug-fix-name
    ```
3.  **Make your changes.** Ensure your code adheres to the existing coding style.
4.  **Add unit tests** for any new functionality or bug fixes.
5.  **Ensure all tests pass.**
6.  **Commit your changes** with a clear and descriptive commit message.
7.  **Push your branch** to your forked repository.
8.  **Create a pull request** to the main Coosu repository.

Please provide a clear description of your changes in the pull request.

## License

Coosu.Beatmap is licensed under the **MIT License**.
You can find the license file at the root of the Coosu repository: [https://github.com/Milkitic/Coosu/blob/master/LICENSE](https://github.com/Milkitic/Coosu/blob/master/LICENSE)

---

If you encounter any bugs or have feature requests, please open an issue on the [GitHub repository](https://github.com/Milkitic/Coosu/issues).
