using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Event;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

[SectionProperty("Events")]
public sealed class EventSection : Section
{
    private const string SectionBgVideo = "//Background and Video events";
    private const string SectionBreak = "//Break Periods";
    private const string SectionStoryboard = "//Storyboard";
    private const string SectionSbSamples = "//Storyboard Sound Samples";

    private readonly StringBuilder _sbInfo = new();
    private readonly Dictionary<string, StringBuilder> _unknownSection = new();
    private string? _currentSection;
    private readonly OsuReadOptions _options;

    public EventSection(Config osuFile)
    {
        _options = (OsuReadOptions)osuFile.Options;
    }

    public BackgroundData? BackgroundInfo { get; set; }
    public VideoData? VideoInfo { get; set; }
    public List<StoryboardSampleData> Samples { get; set; } = new();
    public List<RangeValue<int>> Breaks { get; set; } = new();

    /// <summary>
    /// Raw storyboard text
    /// <para>
    /// For detailed analyzing, use <see href="https://www.nuget.org/packages/Coosu.Storyboard/"/> package for parsing storyboard elements.
    /// </para>
    /// </summary>
    public string? StoryboardText { get; set; }

    public override void Match(string line)
    {
        if (line.StartsWith("//", StringComparison.Ordinal))
        {
            var section = line.Trim();
            switch (section)
            {
                case SectionBgVideo:
                    _currentSection = SectionBgVideo;
                    break;
                case SectionBreak:
                    _currentSection = SectionBreak;
                    break;
                case SectionSbSamples:
                    if (!_options.StoryboardIgnored)
                    {
                        StoryboardText = _sbInfo.ToString().TrimEnd('\r', '\n');
                        _sbInfo.Clear();
                    }

                    _currentSection = SectionSbSamples;
                    break;
                default:
                    if (section.StartsWith(SectionStoryboard, StringComparison.Ordinal))
                    {
                        _currentSection = SectionStoryboard;
                        if (!_options.StoryboardIgnored) _sbInfo.AppendLine(line);
                    }
                    else
                    {
                        _currentSection = section;
                        _unknownSection.Add(section, new StringBuilder());
                    }
                    break;
            }
        }
        else
        {
            switch (_currentSection)
            {
                case SectionBgVideo:
                    // https://osu.ppy.sh/help/wiki/osu!_File_Formats/Osu_(file_format)#videos
                    if (line.StartsWith("Video,", StringComparison.Ordinal) ||
                        line.StartsWith("1,", StringComparison.Ordinal))
                    {
                        double offset = default;
                        string filename = "";

                        int i = -1;
                        foreach (var span in line.SpanSplit(','))
                        {
                            i++;
                            switch (i)
                            {
                                case 1: offset = ParseHelper.ParseDouble(span); break;
                                case 2: filename = span.Trim('"').ToString(); break;
                            }
                        }

                        VideoInfo = new VideoData { Offset = offset, Filename = filename };
                    }
                    else if (line.StartsWith("_") || line.StartsWith(" "))
                    {
                        break;
                    }
                    else
                    {
                        double x = 0;
                        double y = 0;
                        string filename = "";

                        int i = -1;
                        foreach (var span in line.SpanSplit(','))
                        {
                            i++;
                            switch (i)
                            {
                                case 2: filename = span.Trim('"').ToString(); break;
                                case 3: x = ParseHelper.ParseDouble(span); break;
                                case 4: y = ParseHelper.ParseDouble(span); break;
                            }
                        }

                        BackgroundInfo = new BackgroundData { Filename = filename, X = x, Y = y };
                    }
                    break;
                case SectionBreak:
                    {
                        int startTime = default;
                        int endTime = default;

                        int i = -1;
                        foreach (var span in line.SpanSplit(','))
                        {
                            i++;
                            switch (i)
                            {
                                case 1: startTime = ParseHelper.ParseInt32(span); break;
                                case 2: endTime = ParseHelper.ParseInt32(span); break;
                            }
                        }

                        Breaks.Add(new RangeValue<int>(startTime, endTime));
                    }
                    break;
                case SectionSbSamples:
                    if (!_options.SampleIgnored && (line.StartsWith("Sample,", StringComparison.Ordinal) ||
                                                    line.StartsWith("5,", StringComparison.Ordinal)))
                    {
                        int offset = default;
                        byte magicalInt = default;
                        string filename = "";
                        byte volume = default;

                        int i = -1;
                        foreach (var span in line.SpanSplit(','))
                        {
                            i++;
                            switch (i)
                            {
                                case 1: offset = ParseHelper.ParseInt32(span); break;
                                case 2: magicalInt = ParseHelper.ParseByte(span); break;
                                case 3: filename = span.Trim('"').ToString(); break;
                                case 4: volume = ParseHelper.ParseByte(span); break;
                            }
                        }

                        Samples.Add(new StoryboardSampleData
                        {
                            Offset = offset,
                            MagicalInt = magicalInt,
                            Filename = filename,
                            Volume = volume,
                        });
                    }
                    break;
                case SectionStoryboard:
                    if (!_options.StoryboardIgnored) _sbInfo.AppendLine(line);
                    break;
                default:
                    if (_currentSection != null) _unknownSection[_currentSection].AppendLine(line);
                    break;
            }
        }
    }

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write('[');
        textWriter.Write(SectionName);
        textWriter.WriteLine(']');

        textWriter.WriteLine(SectionBgVideo);
        VideoInfo?.AppendSerializedString(textWriter);
        BackgroundInfo?.AppendSerializedString(textWriter);
        textWriter.WriteLine(SectionBreak);
        foreach (var range in Breaks)
        {
            textWriter.Write("2,");
            textWriter.Write(range.StartTime);
            textWriter.Write(',');
            textWriter.WriteLine(range.EndTime);
        }

        if (StoryboardText != null) textWriter.WriteLine(StoryboardText);
        textWriter.WriteLine(SectionSbSamples);
        var validSampleList = Samples.Where(k => k.Volume > 0);
        foreach (var sampleData in validSampleList)
        {
            sampleData.AppendSerializedString(textWriter);
            textWriter.WriteLine();
        }

        foreach (var pair in _unknownSection)
        {
            textWriter.WriteLine(pair.Key);
            textWriter.WriteLine(pair.Value.ToString().TrimEnd('\r', '\n'));
        }
    }
}