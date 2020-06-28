using System;

namespace OSharp.Beatmap
{
    public static class StringUtil
    {
        public static string[] SpanSplit(this string str, string split)
        {
            return str.Split(new[] { split }, StringSplitOptions.None);
            //var list = new List<string>();
            //var span = str.AsSpan();
            //var splitSpan = split.AsSpan();

            //while (true)
            //{
            //    var n = span.IndexOf(splitSpan);
            //    if (n > -1)
            //    {
            //        list.Add(span.Slice(0, n).ToString());
            //        span = span.Slice(n + 1);
            //    }
            //    else
            //    {
            //        list.Add(span.ToString());
            //        break;
            //    }
            //}

            //return list.ToArray();
        }

        public static int SpanIndexOf(this string str, string subStr)
        {
            return str?.IndexOf(subStr) ?? -1;
            //var span = str.AsSpan();
            //var subSpan = subStr.AsSpan();

            //var n = span.IndexOf(subSpan);
            //return n;
        }
    }
}