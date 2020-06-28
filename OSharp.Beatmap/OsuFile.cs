using OSharp.Beatmap.Configurable;
using OSharp.Beatmap.Sections;
using OSharp.Beatmap.Sections.Event;
using OSharp.Beatmap.Sections.Timing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OSharp.Beatmap
{
    public class LocalOsuFile : OsuFile
    {
        public string OriginPath { get; internal set; }
        public bool ReadSuccess { get; set; } = true;
        public Exception ReadException { get; set; }
    }

    public class OsuFile : Config
    {
        public int Version { get; set; }
        public GeneralSection General { get; set; }
        public EditorSection Editor { get; set; }
        public MetadataSection Metadata { get; set; }
        public DifficultySection Difficulty { get; set; }
        public EventSection Events { get; set; }
        public TimingSection TimingPoints { get; set; }
        public ColorSection Colours { get; set; }
        public HitObjectSection HitObjects { get; set; }

        public static async Task<LocalOsuFile> ReadFromFileAsync(string path, Action<ReadOptions> readOptionFactory = null)
        {
            return await Task.Run(() =>
            {
                var targetPath = (path?.StartsWith(@"\\?\") == true) ? path : @"\\?\" + path;
                try
                {
                    using (var sr = new StreamReader(targetPath))
                    {
                        var localOsuFile = ConfigConvert.DeserializeObject<LocalOsuFile>(sr, readOptionFactory);
                        localOsuFile.OriginPath = targetPath;
                        return localOsuFile;
                    }
                }
                catch (Exception ex)
                {
                    return new LocalOsuFile { ReadSuccess = false, ReadException = ex, OriginPath = targetPath };
                }
            }).ConfigureAwait(false);
        }

        public override string ToString() => Path;

        //todo: not optimized
        public void WriteOsuFile(string path, string newDiffName = null)
        {
            File.WriteAllText(path,
                string.Format("osu file format v{0}\r\n\r\n{1}{2}{3}{4}{5}{6}{7}{8}", Version,
                    General?.ToSerializedString(),
                    Editor?.ToSerializedString(),
                    Metadata?.ToSerializedString(newDiffName),
                    Difficulty?.ToSerializedString(),
                    Events?.ToSerializedString(),
                    TimingPoints?.ToSerializedString(),
                    Colours?.ToSerializedString(),
                    HitObjects?.ToSerializedString()));
        }

        internal override void HandleCustom(string line)
        {
            const string verFlag = "osu file format v";

            if (line.StartsWith(verFlag))
            {
                var str = line.Replace(verFlag, "");
                if (!int.TryParse(str, out var verNum))
                    throw new BadOsuFormatException("未知的osu版本: " + str);
                if (verNum < 5)
                    throw new VersionNotSupportedException(verNum);
                Version = verNum;
            }
            else
            {
                throw new BadOsuFormatException("存在问题头声明: " + line);
            }
        }

        public static async Task<bool> OsbFileHasStoryboard(string osbPath)
        {
            using (var sr = new StreamReader(osbPath))
            {
                var line = await sr.ReadLineAsync();

                bool inSbSection = false;
                bool hasInSbSection = false;

                while (!sr.EndOfStream)
                {
                    if (line.StartsWith("//"))
                    {
                        if (line.StartsWith("//Storyboard Layer"))
                        {
                            inSbSection = true;
                            hasInSbSection = true;
                        }
                        else if (hasInSbSection)
                        {
                            break;
                        }
                    }
                    else if (inSbSection)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            return true;
                    }

                    line = await sr.ReadLineAsync();
                }

                return false;
            }
        }

        public static async Task<bool> FileHasStoryboard(string mapPath)
        {
            using (var sr = new StreamReader(mapPath))
            {
                var line = await sr.ReadLineAsync();
                bool hasEvent = false;
                bool inEventsSection = false;
                bool inSbSection = false;
                bool hasInSbSection = false;

                while (!sr.EndOfStream)
                {
                    if (line == null) break;

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (line == "[Events]")
                        {
                            inEventsSection = true;
                            hasEvent = true;

                        }
                        else if (hasEvent)
                        {
                            break;
                        }
                    }
                    else if (inEventsSection)
                    {
                        if (line.StartsWith("//"))
                        {
                            if (line.StartsWith("//Storyboard Layer"))
                            {
                                inSbSection = true;
                                hasInSbSection = true;
                            }
                            else if (hasInSbSection)
                            {
                                break;
                            }
                        }
                        else if (inSbSection)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                                return true;
                        }
                    }

                    line = await sr.ReadLineAsync();
                }
            }

            return false;
        }

        public static OsuFile CreateEmpty()
        {
            var emptyFile = new OsuFile
            {
                Version = 14,
                General = new GeneralSection(),
                Colours = new ColorSection(),
                Difficulty = new DifficultySection(),
                Editor = new EditorSection(),
                Metadata = new MetadataSection(),
                TimingPoints = new TimingSection()
            };
            emptyFile.Events = new EventSection(emptyFile);
            emptyFile.HitObjects = new HitObjectSection(emptyFile);
            emptyFile.TimingPoints.TimingList = new List<TimingPoint>();
            emptyFile.Events.SampleInfo = new List<StoryboardSampleData>();
            return emptyFile;
        }

        protected OsuFile() { }

        private string Path => Shared.IO.File.EscapeFileName(string.Format("{0} - {1} ({2}){3}.osu",
            Metadata.Artist,
            Metadata.Title,
            Metadata.Creator,
            Metadata.Version != "" ? $" [{Metadata.Version}]" : ""));

        public string GetPath(string newDiffName)
        {
            return Shared.IO.File.EscapeFileName(string.Format("{0} - {1} ({2}){3}.osu",
                Metadata.Artist,
                Metadata.Title,
                Metadata.Creator,
                Metadata.Version != "" ? $" [{newDiffName}]" : ""));
        }
    }
}
