# Coosu.Beatmap
A library for analyzing osu beatmap file data.

Getting Timing information is as simple as:
```csharp
Dictionary<double, double> all1_1RhythmIntervalInDifferentBpms = timings.GetInterval(1);            
TimingPoint line = timings.GetLine(12345);
RangeValue<double>[] kiaiRanges = timings.GetTimingKiais();
```
Getting Slider information is as simple as:
```csharp
SliderInfo slider = hitObject.SliderInfo;
SliderEdge[] edges = slider.Edges;
SliderTick[] ticks = slider.Ticks;
SliderTick[] trial = slider.BallTrail;
```
In addition, this library provides all original osu file information and many useful data calcuation.

You can use this library to do further works like making hitsound copier, ai modder, advanced beatmap editor, etc.

## Performance improvement

Since version v2.1.1, the library's performance was optimized. 
### Parsing benchmark
Run `benchmark-ParsingPerformance.ps1`:
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]               : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0             : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET Core 3.1        : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
  .NET Framework 4.8   : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

```
|              Method |              Runtime |      Mean |     Error |    StdDev |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-------------------- |--------------------- |----------:|----------:|----------:|----------:|---------:|---------:|----------:|
| CoosuLatest |             .NET 6.0 |  4.551 ms | 0.0893 ms | 0.0955 ms |  148.4375 |  70.3125 |        - |    907 KB |
|  OsuParsers |             .NET 6.0 |  8.350 ms | 0.1603 ms | 0.1908 ms |  718.7500 | 281.2500 |  93.7500 |  4,367 KB |
| CoosuV2_1_0 |             .NET 6.0 | 16.586 ms | 0.1052 ms | 0.0984 ms | 1625.0000 | 187.5000 |  62.5000 |  8,489 KB |
|             |                      |           |           |           |           |          |          |           |
| CoosuLatest |        .NET Core 3.1 |  3.246 ms | 0.0573 ms | 0.0765 ms |  164.0625 |  82.0313 |        - |    985 KB |
|  OsuParsers |        .NET Core 3.1 |  7.336 ms | 0.0663 ms | 0.0620 ms |  718.7500 | 281.2500 | 125.0000 |  4,367 KB |
| CoosuV2_1_0 |        .NET Core 3.1 | 16.060 ms | 0.1539 ms | 0.1285 ms | 1718.7500 | 187.5000 |  62.5000 |  8,739 KB |
|             |                      |           |           |           |           |          |          |           |
| CoosuLatest |   .NET Framework 4.8 |  8.161 ms | 0.1049 ms | 0.0981 ms |  468.7500 | 203.1250 |  31.2500 |  2,191 KB |
| CoosuV2_1_0 |   .NET Framework 4.8 | 30.774 ms | 0.3718 ms | 0.3296 ms | 2937.5000 | 187.5000 |  62.5000 | 14,428 KB |
|  OsuParsers |   .NET Framework 4.8 | 72.315 ms | 0.7524 ms | 0.6670 ms | 1571.4286 | 285.7143 |        - |  9,562 KB |

### Writing benchmark
Run `benchmark-WritingOsu.ps1`:
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]             : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0           : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET Core 3.1      : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT


```
|            Method |            Runtime |       Mean |     Error |    StdDev |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------ |------------------- |-----------:|----------:|----------:|----------:|---------:|---------:|----------:|
| CoosuLatest |           .NET 6.0 |   3.745 ms | 0.0409 ms | 0.0342 ms |  164.0625 |   7.8125 |        - |    688 KB |
| CoosuV2_1_0 |           .NET 6.0 |   5.657 ms | 0.0472 ms | 0.0442 ms |  617.1875 | 265.6250 | 148.4375 |  2,875 KB |
|  JsonDotNet |           .NET 6.0 |  15.871 ms | 0.2465 ms | 0.2305 ms |  500.0000 |        - |        - |  2,126 KB |
|  OsuParsers |           .NET 6.0 | 230.724 ms | 1.6830 ms | 1.4054 ms | 3000.0000 | 666.6667 |        - | 13,462 KB |
|             |                    |            |           |           |           |          |          |           |
| CoosuLatest |      .NET Core 3.1 |   4.930 ms | 0.0940 ms | 0.0965 ms |  164.0625 |        - |        - |    688 KB |
| CoosuV2_1_0 |      .NET Core 3.1 |   7.641 ms | 0.1206 ms | 0.1128 ms |  718.7500 | 343.7500 | 203.1250 |  3,825 KB |
|  OsuParsers |      .NET Core 3.1 |  11.649 ms | 0.2120 ms | 0.1880 ms | 3000.0000 | 984.3750 |        - | 13,638 KB |
|  JsonDotNet |      .NET Core 3.1 |  18.225 ms | 0.2487 ms | 0.2205 ms |  500.0000 |        - |        - |  2,126 KB |
|             |                    |            |           |           |           |          |          |           |
| CoosuLatest | NETFX 4.8 |   7.982 ms | 0.0842 ms | 0.0788 ms |  281.2500 |        - |        - |  1,177 KB |
| CoosuV2_1_0 | NETFX 4.8 |  11.272 ms | 0.1480 ms | 0.1385 ms | 1109.3750 | 328.1250 | 203.1250 |  5,249 KB |
|  OsuParsers | NETFX 4.8 |  15.577 ms | 0.2606 ms | 0.2438 ms | 3656.2500 | 656.2500 |        - | 15,499 KB |
|  JsonDotNet | NETFX 4.8 |  23.378 ms | 0.2087 ms | 0.1952 ms |  531.2500 |        - |        - |  2,271 KB |


## Using Coosu.Beatmap in your project
Currently, Coosu.Beatmap can't be independent and forced to depend on [Coosu.Shared](https://github.com/Milkitic/Coosu.Shared) And [Coosu.Storyboard](https://github.com/Milkitic/Coosu.Storyboard). So you should clone all these project and reference those libraries.

## Usage and examples
Just one entry class to resolve all calculation: 
```csharp
public async Task AnalyzeOsuFileAsync(string osuFilePath)
{
    OsuFile osuFile = await OsuFile.ReadFromFileAsync(osuFilePath,
        option =>
        {
            // ignore sections which you don't need to improve performance.

            //option.IncludeSection("General", "Metadata", "Events");
            //option.IgnoreSample();
            //option.IgnoreStoryboard();
        });

    // [General]
    GeneralSection general = osuFile.General;
    // original key-value pair
    Console.WriteLine(string.Join(',',
        general.AudioFilename,
        general.AudioLeadIn,
        general.Countdown,
        general.EpilepsyWarning,
        general.LetterboxInBreaks,
        general.Mode,
        general.PreviewTime,
        general.SampleSet,
        general.SkinPreference,
        general.StackLeniency,
        general.WidescreenStoryboard
    ));

    // [Difficulty]
    DifficultySection difficulty = osuFile.Difficulty;
    // original key-value pair
    Console.WriteLine(string.Join(',',
        difficulty.CircleSize,
        difficulty.SliderTickRate,
        difficulty.ApproachRate,
        difficulty.HpDrainRate,
        difficulty.OverallDifficulty,
        difficulty.SliderMultiplier
    ));

    // [Metadata]
    MetadataSection metadata = osuFile.Metadata;
    // original key-value pair
    Console.WriteLine(string.Join(',',
        metadata.Artist,
        metadata.ArtistUnicode,
        metadata.Title,
        metadata.TitleUnicode,
        metadata.BeatmapId,
        metadata.BeatmapSetId,
        metadata.Creator,
        metadata.Source,
        string.Join(' ', metadata.TagList),
        metadata.Version
    ));
    // extension
    metadata.ArtistMeta.ToPreferredString(); // return origin string if unicode is null or empty.
    metadata.TitleMeta.ToPreferredString();

    // [Editor]
    EditorSection editor = osuFile.Editor;
    // original key-value pair
    Console.WriteLine(string.Join(',',
        editor.BeatDivisor,
        string.Join(',', editor.Bookmarks),
        editor.DistanceSpacing,
        editor.GridSize,
        editor.TimelineZoom
    ));

    // [Colour]
    ColorSection color = osuFile.Colours;
    // original key-value pair
    Console.WriteLine(string.Join(',',
      color.Combo1,
      color.Combo2,
      color.Combo3,
      color.Combo4,
      color.Combo5,
      color.Combo6,
      color.Combo7,
      color.Combo8
    ));

    // [Timing]
    TimingSection timings = osuFile.TimingPoints;
    // original timing points option
    TimingPoint timingPoint = timings[0];
    Console.WriteLine(string.Join(',',
        timingPoint.Offset,
        timingPoint.Factor,
        timingPoint.Inherit,
        timingPoint.Kiai,
        timingPoint.Rhythm,
        timingPoint.TimingSampleset,
        timingPoint.Track,
        timingPoint.Volume
    ));
    // timing point extension
    Console.WriteLine($"{timingPoint.Multiple},{timingPoint.Bpm}");
    // timing calculate extension
    Dictionary<double, double> all1_1RhythmIntervalInDifferentBpms = timings.GetInterval(1);
    foreach (var kv in all1_1RhythmIntervalInDifferentBpms)
    {
        var startOffset = kv.Key;
        var bpm1_1interval = kv.Value;
        Console.WriteLine($"{startOffset},{bpm1_1interval}");
    }
    // returns current or closest previous timing point.
    // e.g: (12345) or (12300 when no timing point on 12345)
    TimingPoint line = timings.GetLine(12345);
    // just like above, but only select red line
    TimingPoint redLine = timings.GetRedLine(12345);
    // returns all kiai parts
    RangeValue<double>[] kiaiRanges = timings.GetTimingKiais();

    // [HitObjects]
    HitObjectSection hitObjects = osuFile.HitObjects;
    RawHitObject hitObject = hitObjects[0];
    // original hit objects option
    Console.WriteLine(string.Join(',',
        hitObject.Offset,
        hitObject.X,
        hitObject.Y,
        hitObject.RawType,
        hitObject.Hitsound,
        hitObject.Extras
    ));
    // object type and NC information
    HitObjectType trueType = hitObject.ObjectType;
    int ncSkip = hitObject.NewComboCount;
    // object extras
    Console.WriteLine(string.Join(',',
        hitObject.SampleSet,
        hitObject.AdditionSet,
        hitObject.CustomIndex,
        hitObject.SampleVolume
    ));
    // mania slider / spinner
    if (trueType == HitObjectType.Hold || trueType == HitObjectType.Spinner)
    {
        int endTime = hitObject.HoldEnd;
    }
    // std slider
    if (trueType == HitObjectType.Slider)
    {
        SliderInfo slider = hitObject.SliderInfo;

        SliderType sliderType = slider.SliderType; // linear, bezier ...
        Vector2<float> startPoint = slider.StartPoint; // slider.StartPoint = (hitObject.X, hitObject.Y)
        Vector2<float> endPoint = slider.EndPoint;
        decimal pixelLength = slider.PixelLength; // raw data

        int repeat = slider.Repeat; // 1 = won't repeat
        Vector2<float>[] curvePoints = slider.CurvePoints; // doesn't include start point

        double startTime = slider.StartTime;
        double endTime = slider.EndTime;

        SliderEdge[] edges = slider.Edges;
        foreach (var sliderEdge in edges)
        {
            Vector2<float> edgePosition = sliderEdge.Point;
            double edgeOffset = sliderEdge.Offset;
            ObjectSamplesetType edgeSample = sliderEdge.EdgeSample;
            ObjectSamplesetType edgeAddition = sliderEdge.EdgeAddition;
            HitsoundType edgeHitsound = sliderEdge.EdgeHitsound;
        }

        SliderTick[] ticks = slider.Ticks;
        SliderTick[] defaultTrial = slider.BallTrail;
        SliderTick[] customTrial = slider.GetDiscreteSliderTrailData(100);
        foreach (var sliderTick in ticks)
        {
            double offset = sliderTick.Offset;
            Vector2<float> position = sliderTick.Point;
        }
    }
}
```
## Development and contributing
Feel free to create pull requests and issues.

## License
MIT License
