using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Coosu.Shared.IO;

public static class PathUtils
{
    public static string EscapeFileName(string source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var allInvalid = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());
        var sb = new StringBuilder(source);
        foreach (var c in allInvalid)
        {
            sb.Replace(c, '_');
        }

        return sb.ToString();
    }
}