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
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

[SectionProperty("HitObjects")]
public sealed class HitObjectSection : Section
{
    private readonly TimingSection _timingSection;
    private readonly DifficultySection _difficulty;
    private readonly SpanSplitArgs _sliderArgs = new();
    private readonly SpanSplitArgs _holdArgs = new();
    public List<RawHitObject> HitObjectList { get; set; } = new();

    [SectionIgnore]
    public double MinTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Min(t => t.Offset);
    [SectionIgnore]
    public double MaxTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Max(t => t.Offset);

    public HitObjectSection(OsuFile osuFile)
    {
        _timingSection = osuFile.TimingPoints ??= new TimingSection(osuFile);
        _difficulty = osuFile.Difficulty ??= new DifficultySection();
    }

    public void ComputeSlidersByCurrentSettings()
    {
        _timingSection.TimingList.Sort(TimingPointComparer.Instance);
        HitObjectList = HitObjectList
            .OrderBy(k => k, HitObjectOffsetComparer.Instance)
            .ToList(); // Use LINQ for stable sort

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

            if (_timingSection.TimingList.Count > i + 1)
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
        _sliderArgs.Canceled = false;
        foreach (var span in line.SpanSplit(',', _sliderArgs))
        {
            i++;
            switch (i)
            {
                case 0: x = ParseHelper.ParseInt32(span); break;
                case 1: y = ParseHelper.ParseInt32(span); break;
                case 2: offset = ParseHelper.ParseInt32(span); break;
                case 3: type = (RawObjectType)ParseHelper.ParseByte(span); break;
                case 4: hitsound = (HitsoundType)ParseHelper.ParseByte(span); _sliderArgs.Canceled = true; break;
                default: others = span; break;
            }
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
        int i = -1;
        int holdEnd = default;
        ReadOnlySpan<char> extras = default;
        foreach (var span in others.SpanSplit(','))
        {
            i++;
            switch (i)
            {
                case 0: holdEnd = ParseHelper.ParseInt32(span); break;
                case 1: extras = span; break;
            }
        }

        hitObject.HoldEnd = holdEnd;
        hitObject.SetExtras(extras);
    }

    private void ToHold(RawHitObject hitObject, ReadOnlySpan<char> others)
    {
        _holdArgs.Canceled = false;
        int i = -1;
        int holdEnd = default;
        ReadOnlySpan<char> extras = default;
        foreach (var span in others.SpanSplit(':', _holdArgs))
        {
            i++;
            switch (i)
            {
                case 0: holdEnd = ParseHelper.ParseInt32(span); _holdArgs.Canceled = true; break;
                case 1: extras = span; break;
            }
        }

        hitObject.HoldEnd = holdEnd;
        hitObject.SetExtras(extras);
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
                    repeat = ParseHelper.ParseInt32(infoSpan);
                    break;
                case 2:
                    // length
                    pixelLength = ParseHelper.ParseDouble(infoSpan, ValueConvert.EnUsNumberFormatInfo);
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
                    x = ParseHelper.ParseInt32(s);
                }
                else
                {
                    var y = ParseHelper.ParseInt32(s);
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
                edgeHitsounds[g] = (HitsoundType)ParseHelper.ParseByte(span);
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
                                var x = (ObjectSamplesetType)ParseHelper.ParseByte(span2);
                                edgeSamples[i] = x;
                                break;
                            case 1:
                                var y = (ObjectSamplesetType)ParseHelper.ParseByte(span2);
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
            textWriter.WriteLine();
        }
    }
}