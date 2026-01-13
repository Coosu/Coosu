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

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Sections;

#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                            DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
[SectionProperty("HitObjects")]
public sealed class HitObjectSection : Section
{
    private readonly TimingSection _timingSection;
    private readonly DifficultySection _difficulty;
    public List<RawHitObject> HitObjectList { get; set; } = new(1024);

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

    public override void Match(ReadOnlyMemory<char> memory)
    {
        var lineSpan = memory.Span;

        float x = default;
        float y = default;
        int offset = default;
        RawObjectType type = default;
        HitsoundType hitsound = default;
        ReadOnlySpan<char> others = default;

        var enumerator = lineSpan.SpanSplit(',', 6);
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex)
            {
                case 0: x = ParseHelper.ParseSingle(span); break;
                case 1: y = ParseHelper.ParseSingle(span); break;
                case 2: offset = ParseHelper.ParseInt32(span); break;
                case 3: type = (RawObjectType)ParseHelper.ParseByte(span); break;
                case 4: hitsound = (HitsoundType)ParseHelper.ParseByte(span); break;
                case 5: others = span; break; // The rest of the string after 5 splits
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
        int holdEnd = default;
        ReadOnlySpan<char> extras = default;
        var enumerator = others.SpanSplit(',');
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex) // Assuming CurrentIndex is available and 0-based
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
        int holdEnd = default;
        ReadOnlySpan<char> extras = default;

        var enumerator = others.SpanSplit(':', maxSplits: 2);
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex)
            {
                case 0: holdEnd = ParseHelper.ParseInt32(span); break;
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

        var enumerator = others.SpanSplit(',');
        while (enumerator.MoveNext())
        {
            var infoSpan = enumerator.Current;
            switch (enumerator.CurrentIndex)
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
                    pixelLength = ParseHelper.ParseDouble(infoSpan, ParseHelper.EnUsNumberFormat);
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

        var curveEnumerator = curveInfo.SpanSplit('|');
        while (curveEnumerator.MoveNext())
        {
            var point = curveEnumerator.Current;
            if (curveEnumerator.CurrentIndex < 1)
            {
                sliderType = point[0];
                continue; // curvePoints skip 1
            }

            float x = 0;
            var pointSplitEnumerator = point.SpanSplit(':');
            while (pointSplitEnumerator.MoveNext())
            {
                var s = pointSplitEnumerator.Current;
                if (pointSplitEnumerator.CurrentIndex == 0)
                {
                    x = ParseHelper.ParseSingle(s);
                }
                else
                {
                    var y = ParseHelper.ParseSingle(s);
                    points.Add(new Vector2(x, y));
                }
            }
        }

        // edge hitsounds
        HitsoundType[]? edgeHitsounds;
        ObjectSamplesetType[]? edgeSamples;
        ObjectSamplesetType[]? edgeAdditions;
        if (enumerator.CurrentIndex < 3)
        {
            edgeHitsounds = null;
            edgeSamples = null;
            edgeAdditions = null;
        }
        else
        {
            edgeHitsounds = new HitsoundType[repeat + 1];
            var edgeHitsoundEnumerator = edgeHitsoundInfo.SpanSplit('|');
            while (edgeHitsoundEnumerator.MoveNext())
            {
                var span = edgeHitsoundEnumerator.Current;
                edgeHitsounds[edgeHitsoundEnumerator.CurrentIndex] = (HitsoundType)ParseHelper.ParseByte(span);
            }

            if (enumerator.CurrentIndex < 4)
            {
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeSamples = new ObjectSamplesetType[edgeHitsounds.Length];
                edgeAdditions = new ObjectSamplesetType[edgeHitsounds.Length];
                var sampleAdditionEnumerator = sampleAdditionInfo.SpanSplit('|');
                while (sampleAdditionEnumerator.MoveNext())
                {
                    var span = sampleAdditionEnumerator.Current;
                    var currentIndex = sampleAdditionEnumerator.CurrentIndex;
                    var span2Enumerator = span.SpanSplit(':');
                    while (span2Enumerator.MoveNext())
                    {
                        var span2 = span2Enumerator.Current;
                        switch (span2Enumerator.CurrentIndex)
                        {
                            case 0:
                                var x = (ObjectSamplesetType)ParseHelper.ParseByte(span2);
                                edgeSamples[currentIndex] = x;
                                break;
                            case 1:
                                var y = (ObjectSamplesetType)ParseHelper.ParseByte(span2);
                                edgeAdditions[currentIndex] = y;
                                break;
                        }
                    }
                }
            }
        }

        // extra
        if (enumerator.CurrentIndex == 5)
        {
            hitObject.SetExtras(extraInfo);
        }

        hitObject.SliderInfo = new ExtendedSliderInfo(hitObject)
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

    public override void AppendSerializedString(TextWriter textWriter, int version)
    {
        textWriter.Write('[');
        textWriter.Write(SectionName);
        textWriter.WriteLine(']');
        for (var i = 0; i < HitObjectList.Count; i++)
        {
            var hitObject = HitObjectList[i];
            hitObject.AppendSerializedString(textWriter, version);
            textWriter.WriteLine();
        }
    }
}