﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.Event;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Numerics;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap;

public class OsuFile : Config
{
    private const string VerFlag = "osu file format v";

#if NET6_0_OR_GREATER
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                       DynamicallyAccessedMemberTypes.PublicProperties |
                       DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(GeneralSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                       DynamicallyAccessedMemberTypes.PublicProperties |
                       DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(EditorSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                       DynamicallyAccessedMemberTypes.PublicProperties |
                       DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(MetadataSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                       DynamicallyAccessedMemberTypes.PublicProperties |
                       DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(DifficultySection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                       DynamicallyAccessedMemberTypes.PublicProperties |
                       DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(ColorSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(EventSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(HitObjectSection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(TimingSection))]
#endif

    protected OsuFile()
    {
    }

    public int Version { get; set; }
    public GeneralSection? General { get; set; }
    public EditorSection? Editor { get; set; }
    public MetadataSection? Metadata { get; set; }
    public DifficultySection? Difficulty { get; set; }
    public EventSection? Events { get; set; }
    public TimingSection? TimingPoints { get; set; }
    public ColorSection? Colours { get; set; }
    public HitObjectSection? HitObjects { get; set; }

    private string Filename
    {
        get
        {
            Metadata ??= new MetadataSection();
            return this.GetOsuFilename(Metadata.Version);
        }
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
        };
        emptyFile.TimingPoints = new TimingSection(emptyFile);
        emptyFile.Events = new EventSection(emptyFile);
        emptyFile.HitObjects = new HitObjectSection(emptyFile);
        emptyFile.TimingPoints.TimingList = new List<TimingPoint>();
        emptyFile.Events.Samples = new List<StoryboardSampleData>();
        return emptyFile;
    }

    public static OsuFile ReadFromStream(Stream stream, Action<OsuReadOptions>? configReadOption = null)
    {
        var options = new OsuReadOptions();
        configReadOption?.Invoke(options);
        using var sr = new StreamReader(stream);
        OsuFile osuFile;
        if (stream is FileStream fs)
        {
            LocalOsuFile localOsuFile;
            osuFile = localOsuFile = ConfigConvert.DeserializeObject<LocalOsuFile>(sr, options);
            localOsuFile.OriginalPath = Path.GetFullPath(fs.Name);
        }
        else
        {
            osuFile = ConfigConvert.DeserializeObject<OsuFile>(sr, options);
        }

        return osuFile;
    }

    public static LocalOsuFile ReadFromFile(string path, Action<OsuReadOptions>? configReadOption = null)
    {
        using var fs = new FileStream(GetActualPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
        return (LocalOsuFile)ReadFromStream(fs, configReadOption);
    }

    /// <summary>
    /// Note this is not async stream, but async parsing
    /// </summary>
    /// <param name="path"></param>
    /// <param name="configReadOption"></param>
    /// <returns></returns>
    public static async Task<LocalOsuFile> ReadFromFileAsync(string path,
        Action<OsuReadOptions>? configReadOption = null)
    {
        using var fs = new FileStream(GetActualPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
        return await Task.Run(() => (LocalOsuFile)ReadFromStream(fs, configReadOption));
    }

    public string SaveToDirectory(string directory, string? difficultyName = null)
    {
        Metadata ??= new MetadataSection();
        var fileName = this.GetOsuFilename(difficultyName ?? Metadata.Version);
        var path = Path.Combine(directory, fileName);
        WriteToPath(path, difficultyName);
        return path;
    }

    public void Save(string path)
    {
        WriteToPath(path);
    }

    public override string ToString() => Filename;

    public override void OnDeserialized()
    {
        var readOptions = (OsuReadOptions)Options;
        if (!readOptions.AutoCompute) return;
        var hitObjectSection = HitObjects;
        if (hitObjectSection != null)
            hitObjectSection.ComputeSlidersByCurrentSettings();
        if (TimingPoints != null)
            TimingPoints.TimingList = TimingPoints.TimingList.OrderBy(k => k.Offset).ToList();
    }

    public override void HandleCustom(ReadOnlyMemory<char> memory)
    {
        if (Version != 0) return;
        var line = memory.Span;
        if (line.StartsWith(VerFlag.AsSpan()))
        {
            var verSpan = line.Slice(VerFlag.Length);
            if (!ParseHelper.TryParseInt32(verSpan, out var verNum))
                throw new BadOsuFormatException("Unknown osu file format: " + verSpan.ToString());
            if (verNum is < 3 or > 14)
                throw new VersionNotSupportedException(verNum);
            Version = verNum;
        }
        else
        {
            throw new BadOsuFormatException("Invalid header declaration: " + line.ToString());
        }
    }

    private static string GetActualPath(string path)
    {
#if NETFRAMEWORK && NET462_OR_GREATER
        var targetPath = System.IO.Path.IsPathRooted(path)
            ? (path?.StartsWith(@"\\?\", StringComparison.Ordinal) == true
                ? path
                : @"\\?\" + path)
            : path;
#else
        var targetPath = path;
#endif
        return targetPath;
    }

    private void WriteToPath(string path, string? overrideDifficulty = null)
    {
        using var sw = new StreamWriter(path);
        sw.Write(VerFlag);
        sw.WriteLine(Version);
        sw.WriteLine();

        General ??= new GeneralSection();
        General.AppendSerializedString(sw);
        sw.WriteLine();

        Editor ??= new EditorSection();
        Editor.AppendSerializedString(sw);
        sw.WriteLine();

        Metadata ??= new MetadataSection();
        Metadata.AppendSerializedString(sw, overrideDifficulty);
        sw.WriteLine();

        Difficulty ??= new DifficultySection();
        Difficulty.AppendSerializedString(sw);
        sw.WriteLine();

        Events ??= new EventSection(this);
        Events.AppendSerializedString(sw);
        sw.WriteLine();

        if (TimingPoints != null)
        {
            TimingPoints.AppendSerializedString(sw);
            sw.WriteLine(Environment.NewLine);
        }

        if (Colours != null)
        {
            Colours.AppendSerializedString(sw);
            sw.WriteLine();
        }

        if (HitObjects != null)
        {
            HitObjects.AppendSerializedString(sw);
        }
    }
}