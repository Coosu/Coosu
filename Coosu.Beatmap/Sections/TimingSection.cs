using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

[SectionProperty("TimingPoints")]
public sealed class TimingSection : Section
{
    private readonly OsuFile _osuFile;

    public TimingSection(OsuFile osuFile)
    {
        _osuFile = osuFile;
        _osuFile.General ??= new GeneralSection();
    }

    public List<TimingPoint> TimingList { get; set; } = new();
    [SectionIgnore]
    public double MinTime => TimingList.Count == 0 ? 0 : TimingList.Min(t => t.Offset);
    [SectionIgnore]
    public double MaxTime => TimingList.Count == 0 ? 0 : TimingList.Max(t => t.Offset);

    public override void Match(string line)
    {
        double offset = default;
        double factor = default;
        byte rhythm = default;
        var timingSampleset = _osuFile.General!.SampleSet;
        ushort track = default;
        byte volume = (byte)_osuFile.General.SampleVolume;
        bool inherit = default;
        Effects effects = default;

        int i = -1;
        foreach (var span in line.SpanSplit(','))
        {
            i++;
            switch (i)
            {
                case 0: offset = ParseHelper.ParseDouble(span, ParseHelper.EnUsNumberFormat); break;
                case 1: factor = ParseHelper.ParseDouble(span, ParseHelper.EnUsNumberFormat); break;
                case 2: rhythm = ParseHelper.ParseByte(span); break;
                case 3:
                    var b = ParseHelper.ParseByte(span);
                    timingSampleset = (TimingSamplesetType)(b == 0 ? 0 : b - 1); break;
                case 4: track = ParseHelper.ParseUInt16(span); break;
                case 5: volume = ParseHelper.ParseByte(span); break;
                case 6: inherit = ParseHelper.ParseByte(span) == 0; break;
                case 7: effects = (Effects)ParseHelper.ParseByte(span); break;
            }
        }

        TimingList.Add(new TimingPoint
        {
            Offset = offset,
            Factor = factor,
            Rhythm = rhythm,
            TimingSampleset = timingSampleset,
            Track = track,
            Volume = volume,
            IsInherit = inherit,
            Effects = effects,
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
}