# OSharp.Beatmap
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

## Using OSharp.Beatmap in your project
Currently, OSharp.Beatmap can't be independent and forced to depend on [OSharp.Common](https://github.com/Milkitic/OSharp.Common) And [OSharp.Storyboard](https://github.com/Milkitic/OSharp.Storyboard). So you should clone all these project and reference those libraries.

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
Feel free to send pull requests and raise issues.
## License
MIT License
