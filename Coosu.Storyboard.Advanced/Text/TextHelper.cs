using System;
using System.Globalization;
using System.Text;

namespace Coosu.Storyboard.Advanced.Text
{
    public class TextHelper
    {
        public static string ConvertToFileName(char c, string prefix, string postFix)
        {
            var fileName = prefix + CharToUnicode(c) + postFix + ".png";
            return fileName;
        }

        public static string ConvertToFileName(string name, string prefix, string postFix)
        {
            var fileName = prefix + StringToUnicode(name) + postFix + ".png";
            return fileName;
        }

        public static string CharToUnicode(char c)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(c.ToString());
            string str = bytes[1].ToString("x2") + bytes[0].ToString("x2");
            return str;
        }

        public static string StringToUnicode(string srcText)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(srcText);

            StringBuilder sb = new();
            for (var i = 0; i < bytes.Length; i += 2)
            {
                var b0 = bytes[i];
                var b1 = bytes[i + 1];
                sb.Append(b1.ToString("X2", CultureInfo.InvariantCulture));
                sb.Append(b0.ToString("X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
    }
}