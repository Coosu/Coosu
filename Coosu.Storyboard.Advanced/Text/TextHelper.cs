using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Coosu.Storyboard.Advanced.UI;

namespace Coosu.Storyboard.Advanced.Text
{
    public static class TextHelper
    {
        private static TimeSpan _delayTime = TimeSpan.FromMilliseconds(500);
        public static Dictionary<char, double> ProcessText(TextContext textContext)
        {
            UiThreadHelper.EnsureUiThreadAlive();
            Dictionary<char, double> dict = null!;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var textControl = new TextControl(textContext);
                var window = new WindowBase { Content = new DpiDecorator { Child = textControl } };

                window.Show();
                dict = textControl.SaveImageAndGetWidth();
                window.Close();
            });

            return dict;
        }

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