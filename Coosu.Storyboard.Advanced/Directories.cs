using System.IO;

namespace Coosu.Storyboard.Advanced
{
    public static class Directories
    {
        public static string BaseDir { get; } = "SB";
        public static string CoosuBaseDir { get; } = Path.Combine(BaseDir, "coosu_auto_generated");
        public static string CoosuTextDir { get; } = Path.Combine(CoosuBaseDir, "Texts");
    }
}
