﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Coosu.Beatmap.Configurable;
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
[SectionProperty("TimingPoints")]
public sealed class TimingSection : Section
{
    private readonly OsuFile _osuFile;

    public TimingSection(OsuFile osuFile)
    {
        _osuFile = osuFile;
        _osuFile.General ??= new GeneralSection();
    }

    public List<TimingPoint> TimingList { get; set; } = new(1024);
    [SectionIgnore]
    public double MinTime => TimingList.Count == 0 ? 0 : TimingList.Min(t => t.Offset);
    [SectionIgnore]
    public double MaxTime => TimingList.Count == 0 ? 0 : TimingList.Max(t => t.Offset);

    public override void Match(ReadOnlyMemory<char> memory)
    {
        var lineSpan = memory.Span;

        double offset = default;
        double factor = default;
        int rhythm = default;
        TimingSamplesetType timingSampleset = _osuFile.General!.SampleSet;
        int track = default;
        int volume = _osuFile.General.SampleVolume;
        int inherit = default;
        int effects = default;

        var enumerator = lineSpan.SpanSplit(',');
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex)
            {
                case 0: offset = ParseHelper.ParseDouble(span, ParseHelper.EnUsNumberFormat); break;
                case 1: factor = ParseHelper.ParseDouble(span, ParseHelper.EnUsNumberFormat); break;
                case 2: rhythm = ParseHelper.ParseInt32(span); break;
                case 3:
                    timingSampleset = ParseHelper.ParseInt32(span) switch
                    {
                        1 => TimingSamplesetType.Normal,
                        2 => TimingSamplesetType.Soft,
                        3 => TimingSamplesetType.Drum,
                        _ => timingSampleset
                    };
                    break;
                case 4: track = ParseHelper.ParseInt32(span); break;
                case 5: volume = ParseHelper.ParseInt32(span); break;
                case 6: inherit = ParseHelper.ParseInt32(span); break;
                case 7: effects = ParseHelper.ParseInt32(span); break;
            }
        }

        TimingList.Add(new TimingPoint
        {
            Offset = offset,
            Factor = factor,
            Rhythm = rhythm < 1 ? 4 : rhythm,
            TimingSampleset = timingSampleset,
            Track = (ushort)Math.Max(track, 0),
            Volume = (byte)Clamp(volume, 0, 100),
            IsInherit = inherit != 1,
            Effects = (Effects)(effects & 0b1001),
        });
    }

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write('[');
        textWriter.Write(SectionName);
        textWriter.WriteLine(']');
        for (var i = 0; i < TimingList.Count; i++)
        {
            var timingPoint = TimingList[i];
            timingPoint.AppendSerializedString(textWriter);
            textWriter.WriteLine();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Clamp(int value, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException($"min ({min}) cannot be greater than max ({max})");
        }

        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }

        return value;
    }
}