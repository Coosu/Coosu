using System.IO;
using System.Text;
using System.Threading.Tasks;
using Coosu.Shared.IO;

namespace Coosu.Beatmap.Extensions;

public static class OsuFileExtensions
{
    public static string GetOsuFilename(this OsuFile osuFile, string? difficultyName)
    {
        var sb = new StringBuilder();
        AppendGeneral(osuFile, sb);

        var version = difficultyName ?? osuFile.Metadata.Version;

        if (!string.IsNullOrEmpty(version))
        {
            sb.Append(" [");
            sb.Append(version);
            sb.Append("]");
        }

        sb.Append(".osu");
        return PathUtils.EscapeFileName(sb.ToString());
    }

    public static string GetOsbFilename(this OsuFile osuFile)
    {
        var sb = new StringBuilder();
        AppendGeneral(osuFile, sb);
        sb.Append(".osb");

        return PathUtils.EscapeFileName(sb.ToString());
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

    private static void AppendGeneral(OsuFile osuFile, StringBuilder sb)
    {
        if (!string.IsNullOrEmpty(osuFile.Metadata.Artist))
        {
            sb.Append(osuFile.Metadata.Artist);
            sb.Append(" - ");
            sb.Append(osuFile.Metadata.Title);
        }
        else
        {
            sb.Append(Path.GetFileNameWithoutExtension(osuFile.General.AudioFilename));
        }

        if (!string.IsNullOrEmpty(osuFile.Metadata.Creator))
        {
            sb.Append(" (");
            sb.Append(osuFile.Metadata.Creator);
            sb.Append(")");
        }
    }
}