namespace Coosu.Beatmap.Sections.HitObject
{
    public sealed class ExtendedSliderInfo : SliderInfo
    {
        public ExtendedSliderInfo(SliderInfo sliderInfo)
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
        /// Get the slider's end time.
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public int CurrentEndTime { get; private set; }

        /// <summary>
        /// Get the first duration from slider's head to slider's tail.
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentSingleDuration { get; private set; }

        /// <summary>
        /// Get the total duration
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentDuration { get; private set; }

        /// <summary>
        /// Get current beat duration for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentBeatDuration { get; private set; }

        /// <summary>
        /// Get current slider multiplier for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentSliderMultiplier { get; private set; }

        /// <summary>
        /// Get current tick rate for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public float CurrentTickRate { get; private set; }

        public void SetVariables(double lastRedFactor, double lastLineMultiple,
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
}