using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coosu.Storyboard.Advanced.UI;

namespace Coosu.Storyboard.Advanced.Text
{
    public static class TextHelper
    {
        private static TimeSpan _delayTime = TimeSpan.FromMilliseconds(500);
        public static Dictionary<char, double> ProcessText(TextContext textContext)
        {
            Dictionary<char, double> dict = null!;
            var uiThread = new Thread(() =>
            {
                var textControl = new TextControl(textContext);
                var window = new WindowBase { Content /*= new DpiDecorator { Child*/ = textControl/* } */};
                
                window.Shown += (s, e) =>
                {
                    dict = textControl.SaveImageAndGetWidth();
                    Thread.Sleep(1000);
                    window.Close();
                    System.Windows.Threading.Dispatcher.ExitAllFrames();
                };
                window.Show();
                System.Windows.Threading.Dispatcher.Run();
            })
            {
                IsBackground = true
            };
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            uiThread.Join();
            return dict;
        }

        public static string ConvertToFileName(char c, string prefix, string postFix)
        {
            var u = c >= 33 && c <= 126 && !WindowsInvalidPathChars.Contains(c)
                ? (WindowsInvalidFileNameChars.Contains(c)
                    ? CharToUnicode(c)
                    : (c >= 65 && c <= 90 ? "_" + c + "u" : "_" + c)
                )
                : CharToUnicode(c);

            var fileName = prefix + u + postFix + ".png";
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

        public static readonly char[] WindowsInvalidFileNameChars =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31, ':', '*', '?', '\\', '/'
        };

        public static readonly char[] WindowsInvalidPathChars =
        {
            '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        };
    }
}