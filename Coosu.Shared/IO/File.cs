using System;
using System.IO;
using System.Text;

namespace Coosu.Shared.IO
{
    public static class File
    {
        public static string EscapeFileName(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var allInvalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(source);
            for (var i = 0; i < allInvalid.Length; i++)
            {
                var c = allInvalid[i];
                sb.Replace(c.ToString(), "");
            }

            return sb.ToString();
        }
    }
}
