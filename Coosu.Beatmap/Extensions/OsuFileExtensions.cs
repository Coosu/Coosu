using System.IO;
using System.Threading.Tasks;
using Coosu.Shared.IO;

namespace Coosu.Beatmap.Extensions;

public static class OsuFileExtensions
{
    public static string GetOsuFilename(this OsuFile osuFile, string? difficultyName)
    {
        return PathUtils.EscapeFileName(string.Format("{0} - {1} ({2}){3}.osu",
            osuFile.Metadata.Artist,
            osuFile.Metadata.Title,
            osuFile.Metadata.Creator,
            string.IsNullOrEmpty(difficultyName) ? "" : $" [{difficultyName}]"
        ));
    }

    public static string GetOsbFilename(this OsuFile osuFile)
    {
        return PathUtils.EscapeFileName(string.Format(
            "{0} - {1} ({2}).osb"
            , osuFile.Metadata.Artist,
            osuFile.Metadata.Title,
            osuFile.Metadata.Creator
        ));
    }

    public static async Task<bool> OsuFileHasOsbStoryboard(this LocalOsuFile osuFile)
    {
        var osbFile = GetOsbFilename(osuFile);
        var folder = Path.GetDirectoryName(osuFile.OriginalPath);
        var osbPath = Path.Combine(folder, osbFile);

        return await HasOsbStoryboard(osbPath);
    }

    private static async Task<bool> HasOsbStoryboard(string osbPath)
    {
        if (!File.Exists(osbPath)) return false;

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

    public static async Task<bool> OsuFileHasStoryboard(string osuPath)
    {
        using var sr = new StreamReader(osuPath);
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
}