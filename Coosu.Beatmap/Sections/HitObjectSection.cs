using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("HitObjects")]
    public sealed class HitObjectSection : Section
    {
        private readonly TimingSection _timingSection;
        private readonly DifficultySection _difficulty;
        private readonly SpanSplitArgs _e = new();
        public List<RawHitObject> HitObjectList { get; set; } = new();

        public double MinTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Min(t => t.Offset);
        public double MaxTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Max(t => t.Offset);

        public HitObjectSection(OsuFile osuFile)
        {
            _timingSection = osuFile.TimingPoints;
            _difficulty = osuFile.Difficulty;
        }

        public RawHitObject this[int index] => HitObjectList[index];

        public void ComputeSlidersByCurrentSettings()
        {
            _timingSection.TimingList.Sort(new TimingPointComparer());
            HitObjectList.Sort(new HitObjectOffsetComparer());

            var currentIndex = 0;
            double? nextTiming = default;
            TimingPoint? currentLine = default;
            TimingPoint? currentRedLine = default;
            UpdateTiming(ref currentIndex, ref nextTiming, ref currentLine, ref currentRedLine, true);

            for (var i = 0; i < HitObjectList.Count; i++)
            {
                var hitObject = HitObjectList[i];
                if (hitObject.SliderInfo == null) continue;

                while (nextTiming != null && hitObject.Offset + 0.5 >= nextTiming)
                {
                    UpdateTiming(ref currentIndex, ref nextTiming, ref currentLine, ref currentRedLine);
                }

                hitObject.SliderInfo.UpdateComputedValues(
                    currentRedLine?.Factor ?? 0,
                    currentLine?.Multiple ?? 0,
                    _difficulty.SliderMultiplier,
                    _difficulty.SliderTickRate
                );
            }

            void UpdateTiming(ref int i, ref double? nextT, ref TimingPoint? current, ref TimingPoint? currentRed,
                bool isInitial = false)
            {
                if (_timingSection.TimingList.Count == 0) return;

                current = _timingSection.TimingList[i];
                if (!current.IsInherit)
                    currentRed = current;
                else if (isInitial)
                    currentRed = _timingSection.TimingList.First(t => !t.IsInherit);

                if (_timingSection.TimingList.Count > i)
                    nextT = _timingSection.TimingList[i + 1].Offset;
                else
                    nextT = null;
                i++;
            }
        }
        
        public override void Match(string line)
        {
            int x = default;
            int y = default;
            int offset = default;
            RawObjectType type = default;
            HitsoundType hitsound = default;
            ReadOnlySpan<char> others = default;

            int i = -1;
            _e.Canceled = false;
            foreach (var span in line.SpanSplit(',', _e))
            {
                i++;
#if NETCOREAPP3_1_OR_GREATER
                switch (i)
                {
                    case 0: x = int.Parse(span); break;
                    case 1: y = int.Parse(span); break;
                    case 2: offset = int.Parse(span); break;
                    case 3: type = (RawObjectType)int.Parse(span); break;
                    case 4: hitsound = (HitsoundType)int.Parse(span); _e.Canceled = true; break;
                    default: others = span; break;
                }
#else
                switch (i)
                {
                    case 0: x = int.Parse(span.ToString()); break;
                    case 1: y = int.Parse(span.ToString()); break;
                    case 2: offset = int.Parse(span.ToString()); break;
                    case 3: type = (RawObjectType)int.Parse(span.ToString()); break;
                    case 4: hitsound = (HitsoundType)int.Parse(span.ToString()); _e.Canceled = true; break;
                    default: others = span; break;
                }
#endif
            }

            var hitObject = new RawHitObject
            {
                X = x,
                Y = y,
                Offset = offset,
                RawType = type,
                Hitsound = hitsound
            };

            switch (hitObject.ObjectType)
            {
                case HitObjectType.Circle:
                    ToCircle(hitObject, others);
                    break;
                case HitObjectType.Slider:
                    ToSlider(hitObject, others);
                    break;
                case HitObjectType.Spinner:
                    ToSpinner(hitObject, others);
                    break;
                case HitObjectType.Hold:
                    ToHold(hitObject, others);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HitObjectList.Add(hitObject);
        }

        private void ToCircle(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            // extra
            hitObject.SetExtras(others);
        }

        private void ToSpinner(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            var s = others.ToString();
            var infos = s.Split(',');
            var holdEnd = infos[0];
            hitObject.HoldEnd = int.Parse(holdEnd);
            if (infos.Length > 1)
            {
                var extra = infos[1];
                hitObject.SetExtras(extra.AsSpan());
            }
        }

        private void ToHold(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            var s = others.ToString();
            var index = s.IndexOf(':');

            var holdEnd = s.Substring(0, index);
            var extra = s.Substring(index + 1);
            hitObject.HoldEnd = int.Parse(holdEnd);
            hitObject.SetExtras(extra.AsSpan());
        }

        private void ToSlider(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            ReadOnlySpan<char> curveInfo = default;
            int repeat = default;
            double pixelLength = default;
            ReadOnlySpan<char> edgeHitsoundInfo = default;
            ReadOnlySpan<char> sampleAdditionInfo = default;
            ReadOnlySpan<char> extraInfo = default;

            int index = -1;
            foreach (var infoSpan in others.SpanSplit(','))
            {
                index++;
                switch (index)
                {
                    case 0:
                        curveInfo = infoSpan;
                        break;
                    case 1:
                        // repeat
#if NETCOREAPP3_1_OR_GREATER
                        repeat = int.Parse(infoSpan);
#else
                        repeat = int.Parse(infoSpan.ToString());
#endif
                        break;
                    case 2:
                        // length
#if NETCOREAPP3_1_OR_GREATER
                        pixelLength = double.Parse(infoSpan);
#else
                        pixelLength = double.Parse(infoSpan.ToString());
#endif
                        break;
                    case 3:
                        edgeHitsoundInfo = infoSpan;
                        break;
                    case 4:
                        sampleAdditionInfo = infoSpan;
                        break;
                    case 5:
                        extraInfo = infoSpan;
                        break;
                }
            }

            // slider curve
            char sliderType = default;
            var points = curveInfo.Length > 100
                ? curveInfo.Length > 200
                    ? new List<Vector2>(50)
                    : new List<Vector2>(30)
                : new List<Vector2>();

            int j = -1;
            foreach (var point in curveInfo.SpanSplit('|'))
            {
                j++;
                if (j < 1)
                {
                    sliderType = point[0];
                    continue; // curvePoints skip 1
                }

                int x = 0;
                int i = -1;
                foreach (var s in point.SpanSplit(':'))
                {
                    i++;
                    if (i == 0)
                    {
#if NETCOREAPP3_1_OR_GREATER
                        x = int.Parse(s);
#else
                        x = int.Parse(s.ToString());
#endif
                    }
                    else
                    {
#if NETCOREAPP3_1_OR_GREATER
                        var y = int.Parse(s);
#else
                        var y = int.Parse(s.ToString());
#endif
                        points.Add(new Vector2(x, y));
                    }
                }
            }

            // edge hitsounds
            HitsoundType[]? edgeHitsounds;
            ObjectSamplesetType[]? edgeSamples;
            ObjectSamplesetType[]? edgeAdditions;
            if (index < 3)
            {
                edgeHitsounds = null;
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeHitsounds = new HitsoundType[repeat + 1];
                int g = -1;
                foreach (var span in edgeHitsoundInfo.SpanSplit('|'))
                {
                    g++;
#if NETCOREAPP3_1_OR_GREATER
                    edgeHitsounds[g] = (HitsoundType)int.Parse(span);
#else
                    edgeHitsounds[g] = (HitsoundType)int.Parse(span.ToString());
#endif
                }

                if (index < 4)
                {
                    edgeSamples = null;
                    edgeAdditions = null;
                }
                else
                {
                    edgeSamples = new ObjectSamplesetType[edgeHitsounds.Length];
                    edgeAdditions = new ObjectSamplesetType[edgeHitsounds.Length];
                    int i = -1;
                    foreach (var span in sampleAdditionInfo.SpanSplit('|'))
                    {
                        i++;
                        int k = -1;
                        foreach (var span2 in span.SpanSplit(':'))
                        {
                            k++;
                            switch (k)
                            {
                                case 0:
#if NETCOREAPP3_1_OR_GREATER
                                    var x = (ObjectSamplesetType)int.Parse(span2);
#else
                                    var x = (ObjectSamplesetType)int.Parse(span2.ToString());
#endif
                                    edgeSamples[i] = x;
                                    break;
                                case 1:
#if NETCOREAPP3_1_OR_GREATER
                                    var y = (ObjectSamplesetType)int.Parse(span2);
#else
                                    var y = (ObjectSamplesetType)int.Parse(span2.ToString());
#endif
                                    edgeAdditions[i] = y;
                                    break;
                            }
                        }
                    }
                }
            }

            // extra
            if (index == 5)
            {
                hitObject.SetExtras(extraInfo);
            }

            hitObject.SliderInfo = new ExtendedSliderInfo
            {
                StartPoint = new Vector2(hitObject.X, hitObject.Y),
                StartTime = hitObject.Offset,
                PixelLength = pixelLength,
                ControlPoints = points,
                EdgeAdditions = edgeAdditions,
                EdgeHitsounds = edgeHitsounds,
                EdgeSamples = edgeSamples,
                Repeat = repeat,
                SliderType = sliderType.SliderFlagToEnum()
            };
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write('[');
            textWriter.Write(SectionName);
            textWriter.WriteLine(']');
            for (var i = 0; i < HitObjectList.Count; i++)
            {
                var hitObject = HitObjectList[i];
                hitObject.AppendSerializedString(textWriter);
            }
        }
    }
}
