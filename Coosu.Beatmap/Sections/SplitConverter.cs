using Coosu.Beatmap.Configurable;
using System.Collections.Generic;
using System.Linq;

namespace Coosu.Beatmap.Sections
{
    public class DoubleSplitConverter : ValueConverter<List<double>>
    {
        private readonly string _splitter;

        public DoubleSplitConverter(string splitter)
        {
            _splitter = splitter;
        }

        public override List<double> ReadSection(string value)
        {
            return value == null
                ? new List<double>()
                : value.Split(new[] { _splitter }, System.StringSplitOptions.None)
                    .Select(double.Parse)
                    .ToList();
        }

        public override string WriteSection(List<double> value)
        {
            return value == null ? "" : string.Join(_splitter, value);
        }
    }

    public class Int32SplitConverter : ValueConverter<List<int>>
    {
        private readonly string _splitter;

        public Int32SplitConverter(string splitter)
        {
            _splitter = splitter;
        }

        public override List<int> ReadSection(string value)
        {
            return value == null
                ? new List<int>()
                : value.Split(new[] { _splitter }, System.StringSplitOptions.None)
                    .Select(int.Parse)
                    .ToList();
        }

        public override string WriteSection(List<int> value)
        {
            return value == null ? "" : string.Join(_splitter, value);
        }
    }

    public class SplitConverter : ValueConverter<List<string>>
    {
        private readonly string _splitter;

        public SplitConverter(string splitter)
        {
            _splitter = splitter;
        }

        public override List<string> ReadSection(string value)
        {
            return value == null
                ? new List<string>()
                : value.Split(new[] { _splitter }, System.StringSplitOptions.None)
                    .ToList();
        }

        public override string WriteSection(List<string> value)
        {
            return value == null ? "" : string.Join(_splitter, value);
        }
    }
}