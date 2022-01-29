using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Event;
using Coosu.Shared.Mathematics;

namespace Coosu.Beatmap.Sections
{
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
                            StoryboardText = _sbInfo.ToString();
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
                        // todo: split issue
                        if (line.StartsWith("Video,") || line.StartsWith("1,"))
                        {
                            var infos = line.Split(',');
                            VideoInfo = new VideoData { Offset = double.Parse(infos[1]), Filename = infos[2].Trim('"') };
                        }
                        else
                        {
                            var infos = line.Split(',');
                            double x = 0, y = 0;
                            if (infos.Length > 3)
                            {
                                x = double.Parse(infos[3]);
                                y = double.Parse(infos[4]);
                            }

                            BackgroundInfo = new BackgroundData
                            {
                                Filename = infos[2].Trim('"'),
                                X = x,
                                Y = y
                            };
                        }
                        break;
                    case SectionBreak:
                        {
                            var infos = line.Split(',');
                            Breaks.Add(new RangeValue<int>(int.Parse(infos[1]), int.Parse(infos[2])));
                        }
                        break;
                    case SectionSbSamples:
                        if (!_options.SampleIgnored)
                            if (line.StartsWith("Sample,") || line.StartsWith("5,"))
                            {
                                var infos = line.Split(',');
                                Samples.Add(new StoryboardSampleData
                                {
                                    Offset = int.Parse(infos[1]),
                                    MagicalInt = byte.Parse(infos[2]),
                                    Filename = infos[3].Trim('"'),
                                    Volume = infos.Length > 4 ? byte.Parse(infos[4]) : default,
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
            textWriter.WriteLine($"[{SectionName}]");
            textWriter.WriteLine(SectionBgVideo);
            VideoInfo?.AppendSerializedString(textWriter);
            BackgroundInfo?.AppendSerializedString(textWriter);
            textWriter.WriteLine(SectionBreak);
            foreach (var range in Breaks)
            {
                textWriter.WriteLine($"2,{range.StartTime},{range.EndTime}");
            }

            textWriter.WriteLine(StoryboardText?.TrimEnd('\r', '\n'));
            textWriter.WriteLine(SectionSbSamples);
            var validSampleList = Samples.Where(k => k.Volume > 0);
            foreach (var sampleData in validSampleList)
            {
                sampleData.AppendSerializedString(textWriter);
            }

            foreach (var pair in _unknownSection)
            {
                textWriter.WriteLine($"{pair.Key}\r\n" +
                                     $"{pair.Value.ToString().TrimEnd('\r', '\n')}");
            }
        }
    }
}
