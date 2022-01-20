using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections
{
    public sealed class DoubleSplitConverter : ValueConverter<List<double>>
    {
        private readonly char _splitter;

        public DoubleSplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<double> ReadSection(ReadOnlySpan<char> value)
        {
            var list = new List<double>();
            foreach (var subString in value.SpanSplit(_splitter))
            {
#if NETCOREAPP3_1_OR_GREATER
                list.Add(double.Parse(subString));
#else
                list.Add(double.Parse(subString.ToString()));
#endif
            }

            return list;
        }

        public override string WriteSection(List<double> value)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < value.Count; i++)
            {
                var d = value[i];
                sb.Append(d.ToIcString());
                sb.Append(_splitter);
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }

    public sealed class Int32SplitConverter : ValueConverter<List<int>>
    {
        private readonly char _splitter;

        public Int32SplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<int> ReadSection(ReadOnlySpan<char> value)
        {
            var list = new List<int>();
            foreach (var subString in value.SpanSplit(_splitter))
            {
#if NETCOREAPP3_1_OR_GREATER
                list.Add(int.Parse(subString));
#else
                list.Add(int.Parse(subString.ToString()));
#endif
            }

            return list;
        }

        public override string WriteSection(List<int> value)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < value.Count; i++)
            {
                var d = value[i];
                sb.Append(d);
                sb.Append(_splitter);
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }

    public sealed class SplitConverter : ValueConverter<List<string>>
    {
        private readonly char _splitter;

        public SplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<string> ReadSection(ReadOnlySpan<char> value)
        {
            var list = new List<string>();
            foreach (var subString in value.SpanSplit(_splitter))
            {
                list.Add(subString.ToString());
            }

            return list;
        }

        public override string WriteSection(List<string> value)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < value.Count; i++)
            {
                var d = value[i];
                sb.Append(d);
                sb.Append(_splitter);
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}