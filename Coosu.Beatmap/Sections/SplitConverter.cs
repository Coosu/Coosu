using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections
{
    public class DoubleSplitConverter : ValueConverter<List<double>>
    {
        private readonly char _splitter;

        public DoubleSplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<double> ReadSection(ReadOnlySpan<char> value)
        {
#if NETCOREAPP3_1_OR_GREATER
            var list = new List<double>();
            foreach (var subString in value.SpanSplit(_splitter))
            {
                list.Add(double.Parse(subString));
            }

            return list;
#else
            return value.ToString().Split(_splitter)
                .Select(double.Parse)
                .ToList();
#endif
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

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }

    public class Int32SplitConverter : ValueConverter<List<int>>
    {
        private readonly char _splitter;

        public Int32SplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<int> ReadSection(ReadOnlySpan<char> value)
        {
#if NETCOREAPP3_1_OR_GREATER
            var list = new List<int>();
            foreach (var subString in value.SpanSplit(_splitter))
            {
                list.Add(int.Parse(subString));
            }

            return list;
#else
            return value.ToString().Split(_splitter)
                .Select(int.Parse)
                .ToList();
#endif
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

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }

    public class SplitConverter : ValueConverter<List<string>>
    {
        private readonly char _splitter;

        public SplitConverter(char splitter)
        {
            _splitter = splitter;
        }

        public override List<string> ReadSection(ReadOnlySpan<char> value)
        {
            return value.ToString()
                .Split(new[] { _splitter }, StringSplitOptions.None)
                .ToList();
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

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}