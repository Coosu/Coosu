using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coosu.Shared.IO;

public static class PathUtils
{
    public static string EscapeFileName(string source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var allInvalid = new HashSet<char>(Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()));
        Span<char> span = stackalloc char[192];
        var sb = new ValueStringBuilder(span);
        foreach (var c in source.Where(c => !allInvalid.Contains(c)))
        {
            sb.Append(c);
        }

        return sb.ToString();
    }
}