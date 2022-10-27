using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections.HitObject;

public sealed class ExtendedSliderInfo : SliderInfo
{
    public ExtendedSliderInfo(RawHitObject baseObject) : base(baseObject)
    {

    }

    public ExtendedSliderInfo(SliderInfo sliderInfo, RawHitObject baseObject) : base(baseObject)
    {
        SliderType = sliderInfo.SliderType;
        ControlPoints = sliderInfo.ControlPoints;
        Repeat = sliderInfo.Repeat;
        PixelLength = sliderInfo.PixelLength;
        EdgeHitsounds = sliderInfo.EdgeHitsounds;
        EdgeSamples = sliderInfo.EdgeSamples;
        EdgeAdditions = sliderInfo.EdgeAdditions;
        StartPoint = sliderInfo.StartPoint;
        StartTime = sliderInfo.StartTime;
    }

    /// <summary>
    /// Get current beat duration for this slider
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public double CurrentBeatDuration { get; private set; }

    /// <summary>
    /// Get the total duration
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public double CurrentDuration { get; private set; }

    /// <summary>
    /// Get the slider's end time.
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public int CurrentEndTime { get; private set; }

    /// <summary>
    /// Get the first duration from slider's head to slider's tail.
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public double CurrentSingleDuration { get; private set; }

    /// <summary>
    /// Get current slider multiplier for this slider
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public double CurrentSliderMultiplier { get; private set; }

    /// <summary>
    /// Get current tick rate for this slider
    /// <para>
    /// <b>Note: </b>This is a computed value that can't be updated after timing changes until calling <see cref="UpdateComputedValues"/>
    /// </para>
    /// </summary>
    [SectionIgnore]
    public float CurrentTickRate { get; private set; }

    public void UpdateComputedValues(double lastRedFactor, double lastLineMultiple,
        double diffSliderMultiplier, float diffTickRate)
    {
        CurrentBeatDuration = lastRedFactor;
        CurrentSliderMultiplier = diffSliderMultiplier * lastLineMultiple;
        CurrentTickRate = diffTickRate;

        CurrentSingleDuration = PixelLength / (100 * CurrentSliderMultiplier) * lastRedFactor;
        CurrentEndTime = (int)(StartTime + CurrentSingleDuration * Repeat);
        CurrentDuration = CurrentEndTime - StartTime;
    }
}