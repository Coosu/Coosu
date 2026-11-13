using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Event;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Shared.Numerics;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Sections;

#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                            DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
[SectionProperty("Events")]
public sealed class EventSection : Section
{
    private enum EventType
    {
        Unknown,
        Background,
        Video,
        Break,
        Sprite,
        Sample,
        Animation
    }

    private const string SectionBgVideo = "//Background and Video events";
    private const string SectionBreak = "//Break Periods";
    private const string SectionStoryboard = "//Storyboard";
    private const string SectionSbSamples = "//Storyboard Sound Samples";

    private readonly StringBuilder _sbInfo = new();
    private string? _storyboardText;
    private readonly Dictionary<string, StringBuilder> _unknownSection = new();
    private string? _currentSection;
    private readonly OsuReadOptions _options;

    public EventSection(OsuFile osuFile)
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
    public string? StoryboardText
    {
        get
        {
            if (_storyboardText != null || _sbInfo.Length == 0) return _storyboardText;
            var length = _sbInfo.Length;
            while (length > 0 && (_sbInfo[length - 1] == '\r' || _sbInfo[length - 1] == '\n')) length--;
            return _storyboardText = _sbInfo.ToString(0, length);
        }
        set
        {
            _storyboardText = value;
            _sbInfo.Clear();
        }
    }

    public override void Match(ReadOnlyMemory<char> memory)
    {
        var lineSpan = memory.Span;
        if (lineSpan.IsEmpty) return;

        // Commands dominate large storyboards. They need no event-type scan or lookup.
        if (lineSpan[0] == ' ' || lineSpan[0] == '_' || lineSpan[0] == '\t')
        {
            if (!_options.StoryboardIgnored) AppendStoryboardLine(lineSpan);
            return;
        }

        if (lineSpan.StartsWith("//".AsSpan()))
        {
            var section = lineSpan.Trim();
            switch (section)
            {
                case SectionBgVideo:
                    _currentSection = SectionBgVideo;
                    break;
                case SectionBreak:
                    _currentSection = SectionBreak;
                    break;
                case SectionSbSamples:
                    _currentSection = SectionSbSamples;
                    break;
                default:
                    if (section.StartsWith(SectionStoryboard.AsSpan()))
                    {
                        _currentSection = SectionStoryboard;
                        if (!_options.StoryboardIgnored) AppendStoryboardLine(lineSpan);
                    }
                    else
                    {
                        _currentSection = section.ToString();
                        if (!_unknownSection.ContainsKey(_currentSection))
                            _unknownSection.Add(_currentSection, new StringBuilder());
                    }
                    break;
            }
        }
        else
        {
            // Comment headings are optional and can be out of order. In particular,
            // a Sprite can follow break periods before the next storyboard heading.
            var eventType = GetEventType(lineSpan);
            switch (eventType)
            {
                case EventType.Background:
                case EventType.Video:
                    // https://osu.ppy.sh/help/wiki/osu!_File_Formats/Osu_(file_format)#videos
                    if (eventType == EventType.Video)
                    {
                        double offset = default;
                        string filename = "";

                        var enumerator = lineSpan.SpanSplit(',');
                        while (enumerator.MoveNext())
                        {
                            var span = enumerator.Current;
                            switch (enumerator.CurrentIndex)
                            {
                                case 1: offset = ParseHelper.ParseDouble(span); break;
                                case 2: filename = span.Trim('"').ToString(); break;
                            }
                        }

                        VideoInfo = new VideoData { Offset = offset, Filename = filename };
                    }
                    else
                    {
                        double x = 0;
                        double y = 0;
                        string filename = "";

                        var enumerator = lineSpan.SpanSplit(',');
                        while (enumerator.MoveNext())
                        {
                            var span = enumerator.Current;
                            switch (enumerator.CurrentIndex)
                            {
                                case 2: filename = span.Trim('"').ToString(); break;
                                case 3: x = ParseHelper.ParseDouble(span); break;
                                case 4: y = ParseHelper.ParseDouble(span); break;
                            }
                        }

                        BackgroundInfo = new BackgroundData { Filename = filename, X = x, Y = y };
                    }
                    break;
                case EventType.Break:
                    {
                        int startTime = default;
                        int endTime = default;

                        var enumerator = lineSpan.SpanSplit(',');
                        while (enumerator.MoveNext())
                        {
                            var span = enumerator.Current;
                            switch (enumerator.CurrentIndex)
                            {
                                case 1: startTime = ParseHelper.ParseInt32(span); break;
                                case 2: endTime = ParseHelper.ParseInt32(span); break;
                            }
                        }

                        Breaks.Add(new RangeValue<int>(startTime, endTime));
                    }
                    break;
                case EventType.Sample:
                    if (!_options.SampleIgnored)
                    {
                        int offset = default;
                        byte magicalInt = default;
                        string filename = "";
                        byte volume = default;

                        var enumerator = lineSpan.SpanSplit(',');
                        while (enumerator.MoveNext())
                        {
                            var span = enumerator.Current;
                            switch (enumerator.CurrentIndex)
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
                case EventType.Sprite:
                case EventType.Animation:
                    _currentSection = SectionStoryboard;
                    if (!_options.StoryboardIgnored) AppendStoryboardLine(lineSpan);
                    break;
                default:
                    if (_currentSection != null &&
                        _unknownSection.TryGetValue(_currentSection, out var unknownSection))
                    {
                        unknownSection.AppendLine(lineSpan.ToString());
                    }
                    else
                    {
                        if (!_options.StoryboardIgnored) AppendStoryboardLine(lineSpan);
                    }
                    break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static EventType GetEventType(ReadOnlySpan<char> line)
    {
        if (line.Length > 1 && line[1] == ',')
        {
            return line[0] switch
            {
                '0' => EventType.Background,
                '1' => EventType.Video,
                '2' => EventType.Break,
                '4' => EventType.Sprite,
                '5' => EventType.Sample,
                '6' => EventType.Animation,
                _ => EventType.Unknown
            };
        }

        switch (line[0])
        {
            case 'S':
                if (line.StartsWith("Sample,".AsSpan())) return EventType.Sample;
                if (line.StartsWith("Sprite,".AsSpan())) return EventType.Sprite;
                break;
            case 'B':
                if (line.StartsWith("Background,".AsSpan())) return EventType.Background;
                if (line.StartsWith("Break,".AsSpan())) return EventType.Break;
                break;
            case 'V':
                if (line.StartsWith("Video,".AsSpan())) return EventType.Video;
                break;
            case 'A':
                if (line.StartsWith("Animation,".AsSpan())) return EventType.Animation;
                break;
        }

        return EventType.Unknown;
    }

    private void AppendStoryboardLine(ReadOnlySpan<char> line)
    {
        if (_sbInfo.Length == 0 && !string.IsNullOrEmpty(_storyboardText)) _sbInfo.AppendLine(_storyboardText);
        _storyboardText = null;
#if NET6_0_OR_GREATER
        _sbInfo.Append(line);
        _sbInfo.AppendLine();
#else
        _sbInfo.AppendLine(line.ToString());
#endif
    }

    public override void AppendSerializedString(TextWriter textWriter, int version)
    {
        textWriter.Write('[');
        textWriter.Write(SectionName);
        textWriter.WriteLine(']');

        textWriter.WriteLine(SectionBgVideo);
        VideoInfo?.AppendSerializedString(textWriter, version);
        BackgroundInfo?.AppendSerializedString(textWriter, version);
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
        foreach (var sampleData in Samples)
        {
            if (sampleData.Volume == 0) continue;
            sampleData.AppendSerializedString(textWriter, version);
            textWriter.WriteLine();
        }

        foreach (var pair in _unknownSection)
        {
            textWriter.WriteLine(pair.Key);
            textWriter.WriteLine(pair.Value.ToString().TrimEnd('\r', '\n'));
        }
    }
}
