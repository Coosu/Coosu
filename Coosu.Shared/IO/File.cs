using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Shared.IO
{
    public static class File
    {
        public static string EscapeFileName(string source)
        {
            return source.Replace(@"\", "").Replace(@"/", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "")
                .Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
        }
    }
}
