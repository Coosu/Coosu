using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Sections.HitObject
{
    public sealed class SliderInfo
    {
        public SliderInfo(Vector2 startPoint, int offset, double beatDuration, double sliderMultiplier, float tickRate, float pixelLength)
        {
            StartPoint = startPoint;
            PixelLength = pixelLength;
            StartTime = offset;
            CurrentBeatDuration = beatDuration;
            CurrentSliderMultiplier = sliderMultiplier;
            CurrentTickRate = tickRate;
            CurrentSingleDuration = PixelLength / (100 * CurrentSliderMultiplier) * CurrentBeatDuration;
            CurrentEndTime = (int)(StartTime + CurrentSingleDuration * Repeat);
            CurrentDuration = CurrentEndTime - StartTime;
        }

        public SliderType SliderType { get; set; }
        public Vector2[] ControlPoints { get; set; }
        public int Repeat { get; set; }
        public float PixelLength { get; set; }
        public HitsoundType[]? EdgeHitsounds { get; set; }
        public ObjectSamplesetType[]? EdgeSamples { get; set; }
        public ObjectSamplesetType[]? EdgeAdditions { get; set; }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint => ControlPoints.Last();

        public int StartTime { get; set; }

        /// <summary>
        /// Get the slider's end time.
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public int CurrentEndTime { get; internal set; }

        /// <summary>
        /// Get the first duration from slider's head to slider's tail.
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentSingleDuration { get; internal set; }

        /// <summary>
        /// Get the total duration
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentDuration { get; internal set; }

        /// <summary>
        /// Get current beat duration for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentBeatDuration { get; internal set; }

        /// <summary>
        /// Get current slider multiplier for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public double CurrentSliderMultiplier { get; internal set; }

        /// <summary>
        /// Get current tick rate for this slider
        /// <para>
        /// <b>Please Note this is a computed value that can't be updated after timing changes.</b>
        /// </para>
        /// </summary>
        public float CurrentTickRate { get; internal set; }

        public override string ToString()
        {
            var sampleList = new List<(ObjectSamplesetType, ObjectSamplesetType)>();
            string edgeSampleStr;
            string edgeHitsoundStr;
            if (EdgeSamples != null)
            {
                for (var i = 0; i < EdgeSamples.Length; i++)
                {
                    var objectSamplesetType = EdgeSamples[i];
                    var objectAdditionType = EdgeAdditions[i];
                    sampleList.Add((objectSamplesetType, objectAdditionType));
                }

                edgeSampleStr = "," + string.Join("|", sampleList.Select(k => $"{(int)k.Item1}:{(int)k.Item2}"));
            }
            else
            {
                edgeSampleStr = "";
            }

            if (EdgeHitsounds != null)
            {
                edgeHitsoundStr = "," + string.Join("|", EdgeHitsounds.Select(k => $"{(int)k}"));
            }
            else
            {
                edgeHitsoundStr = "";
            }

            return string.Format("{0}|{1},{2},{3}{4}{5}",
                SliderType.ParseToCode(),
                string.Join("|", ControlPoints.Select(k => $"{k.X}:{k.Y}")),
                Repeat,
                PixelLength,
                edgeHitsoundStr,
                edgeSampleStr);
        }
    }

    public struct SliderEdge
    {
        public double Offset { get; set; }
        public Vector2 Point { get; set; }
        public HitsoundType EdgeHitsound { get; set; }
        public ObjectSamplesetType EdgeSample { get; set; }
        public ObjectSamplesetType EdgeAddition { get; set; }
    }

    public struct SliderTick
    {
        public SliderTick(double offset, Vector2 point)
        {
            Offset = offset;
            Point = point;
        }

        public double Offset { get; set; }
        public Vector2 Point { get; set; }
    }
}
