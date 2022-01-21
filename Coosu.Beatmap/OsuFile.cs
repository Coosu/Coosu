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
        private const string VerFlag = "osu file format v";
        public int Version { get; set; }
        public GeneralSection General { get; set; }
        public EditorSection Editor { get; set; }
        public MetadataSection Metadata { get; set; }
        public DifficultySection Difficulty { get; set; }
        public EventSection Events { get; set; }
        public TimingSection TimingPoints { get; set; }
        public ColorSection Colours { get; set; }
        public HitObjectSection HitObjects { get; set; }

        public static async Task<LocalOsuFile> ReadFromFileAsync(string path, Action<OsuReadOptions>? readOptionFactory = null)
        {
#if NETFRAMEWORK && NET462_OR_GREATER
            var targetPath = System.IO.Path.IsPathRooted(path)
                ? (path?.StartsWith(@"\\?\") == true
                    ? path
                    : @"\\?\" + path)
                : path;
#else
            var targetPath = path;
#endif
            var options = new OsuReadOptions();
            readOptionFactory?.Invoke(options);
            var localOsuFile = await Task
                .Run(() =>
                {
                    using var sr = new StreamReader(targetPath);
                    return ConfigConvert.DeserializeObject<LocalOsuFile>(sr, options);
                })
                .ConfigureAwait(false);
            localOsuFile.OriginPath = targetPath;
            return localOsuFile;
        }

        public override string ToString() => Path;

        public void WriteOsuFile(string path, string? newDiffName = null)
        {
            using var sw = new StreamWriter(path);
            sw.Write(VerFlag);
            sw.WriteLine(Version);
            sw.WriteLine();

            General.AppendSerializedString(sw);
            sw.WriteLine();

            Editor.AppendSerializedString(sw);
            sw.WriteLine();

            Metadata.AppendSerializedString(sw, newDiffName);
            sw.WriteLine();

            Difficulty.AppendSerializedString(sw);
            sw.WriteLine();

            Events.AppendSerializedString(sw);
            sw.WriteLine();

            TimingPoints.AppendSerializedString(sw);
            sw.WriteLine(Environment.NewLine);

            Colours.AppendSerializedString(sw);
            sw.WriteLine();

            HitObjects.AppendSerializedString(sw);
        }

        public override void OnDeserialized()
        {
            if (((OsuReadOptions)Options).AutoCompute)
                this.HitObjects.ComputeSlidersByCurrentSettings();
        }

        public override void HandleCustom(string line)
        {
            if (Version != 0) return;
            if (line.StartsWith(VerFlag))
            {
                var str = line.Substring(VerFlag.Length);
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
            emptyFile.Events.Samples = new List<StoryboardSampleData>();
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
