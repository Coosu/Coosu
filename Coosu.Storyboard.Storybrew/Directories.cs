using System.IO;

namespace Coosu.Storyboard.Storybrew;

public static class Directories
{
    public static string BaseDir { get; } = "SB";
    public static string CoosuBaseDir { get; } = Path.Combine(BaseDir, "coosu.autogen");
    public static string CoosuTextDir { get; } = Path.Combine(CoosuBaseDir, "Texts");
}