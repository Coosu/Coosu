using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Event;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Management;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Events")]
    public class EventSection : Section
    {
        public BackgroundData BackgroundInfo { get; set; }
        public VideoData VideoInfo { get; set; }
        public List<StoryboardSampleData> SampleInfo { get; set; } = new List<StoryboardSampleData>();
        public List<RangeValue<int>> Breaks { get; set; } = new List<RangeValue<int>>();
        public ElementGroup ElementGroup { get; set; }

        public EventSection(OsuFile osuFile)
        {
            _options = osuFile.Options;
        }

        private readonly StringBuilder _sbInfo = new StringBuilder();
        private readonly Dictionary<string, StringBuilder> _unknownSection = new Dictionary<string, StringBuilder>();
        private string _currentSection;
        private ReadOptions _options;

        private const string SectionBgVideo = "//Background and Video events";
        private const string SectionBreak = "//Break Periods";
        private const string SectionStoryboard = "//Storyboard";
        private const string SectionSbSamples = "//Storyboard Sound Samples";

        public override void Match(string line)
        {
            if (line.StartsWith("//"))
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
                        if (!_options.StoryboardIgnored) ElementGroup = ElementGroup.ParseFromText(_sbInfo.ToString().Trim('\r', '\n'));
                        _currentSection = SectionSbSamples;
                        break;
                    default:
                        if (section.StartsWith(SectionStoryboard))
                        {
                            _currentSection = SectionStoryboard;
                            _sbInfo.AppendLine(line);
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
                                Unknown1 = infos[0],
                                Unknown2 = infos[1],
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
                                SampleInfo.Add(new StoryboardSampleData
                                {
                                    Offset = int.Parse(infos[1]),
                                    MagicalInt = int.Parse(infos[2]),
                                    Filename = infos[3].Trim('"'),
                                    Volume = infos.Length > 4 ? int.Parse(infos[4]) : 0,
                                });
                            }
                        break;
                    case SectionStoryboard:
                        if (!_options.StoryboardIgnored) _sbInfo.AppendLine(line);
                        break;
                    default:
                        _unknownSection[_currentSection].AppendLine(line);
                        break;
                }
            }
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.WriteLine($"[{SectionName}]");
            textWriter.WriteLine(SectionBgVideo);
            textWriter.WriteLine(VideoInfo); //optimize
            textWriter.WriteLine(BackgroundInfo); //optimize
            textWriter.WriteLine(SectionBreak);
            foreach (var range in Breaks)
            {
                textWriter.WriteLine($"2,{range.StartTime},{range.EndTime}");
            }

            textWriter.WriteLine(_sbInfo.ToString().TrimEnd('\r', '\n'));
            textWriter.WriteLine(SectionSbSamples);
            var validSampleList = SampleInfo.Where(k => k.Volume > 0);
            foreach (var sampleData in validSampleList)
            {
                textWriter.WriteLine(sampleData); //optimize
            }

            foreach (var pair in _unknownSection)
            {
                textWriter.WriteLine($"{pair.Key}\r\n" +
                                     $"{pair.Value.ToString().TrimEnd('\r', '\n')}");
            }
        }
    }
}
