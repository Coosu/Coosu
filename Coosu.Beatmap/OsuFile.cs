using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.Event;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap
{
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
#if NETFRAMEWORK
            var targetPath = System.IO.Path.IsPathRooted(path)
                ? (path?.StartsWith(@"\\?\") == true
                    ? path
                    : @"\\?\" + path)
                : path;
#else
            var targetPath = path;
#endif
            using var sr = new StreamReader(targetPath);
            var localOsuFile = await Task
                .Run(() => ConfigConvert.DeserializeObject<LocalOsuFile>(sr, readOptionFactory))
                .ConfigureAwait(false);
            localOsuFile.OriginPath = targetPath;
            return localOsuFile;
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

        public override void HandleCustom(string line)
        {
            if (Version != 0) return;
            const string verFlag = "osu file format v";
            if (line.StartsWith(verFlag))
            {
                var str = line.Substring(verFlag.Length);
                if (!int.TryParse(str, out var verNum))
                    throw new BadOsuFormatException("Unknown osu file format: " + str);
                if (verNum < 5)
                    throw new VersionNotSupportedException(verNum);
                Version = verNum;
            }
            else
            {
                throw new BadOsuFormatException("Invalid header declaration: " + line);
            }
        }

        public static async Task<bool> OsbFileHasStoryboard(string osbPath)
        {
            using var sr = new StreamReader(osbPath);
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

        public static async Task<bool> FileHasStoryboard(string mapPath)
        {
            using var sr = new StreamReader(mapPath);
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

        private string Path => GetPath(Metadata.Version);

        public string GetPath(string newDiffName)
        {
            return Shared.IO.PathUtils.EscapeFileName(string.Format("{0} - {1} ({2}){3}.osu",
                Metadata.Artist,
                Metadata.Title,
                Metadata.Creator,
                newDiffName != "" ? $" [{newDiffName}]" : ""));
        }
    }
}
