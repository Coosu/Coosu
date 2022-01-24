using System;

namespace SplitBenchmark;

public static class StringExtensions
{
    public static CharSplitRangeEnumerator SpanSplitRange(this string str, char c)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitRangeEnumerator(str.AsSpan(), c);
    }

    public static CharSplitRangeEnumerator SpanSplitRange(this ReadOnlySpan<char> span, char c)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitRangeEnumerator(span, c);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct CharSplitRangeEnumerator
    {
        private ReadOnlySpan<char> _span;
        private int _index;
        private int _length;
        private readonly char _c;

        public CharSplitRangeEnumerator(ReadOnlySpan<char> span, char c)
        {
            _span = span;
            _c = c;
            Current = default;
            _index = -1;
            _length = 0;
        }

        // Needed to be compatible with the foreach operator
        public CharSplitRangeEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _span;
            if (span.Length == 0) // Reach the end of the string
                return false;

            var index = span.IndexOf(_c);
            _index++;

            if (index == -1) // The string is composed of only one line
            {
                var leftLength = span.Length;
                Current = (_index, (_length, leftLength));// The remaining string
                _span = ReadOnlySpan<char>.Empty;
                return true;
            }


            var length = index - _length;
            Current = (_index, (_length, length));
            _span = span.Slice(index + 1);
            _length += length + 1;
            return true;

        }

        public (int index, (int startIndex, int length) range) Current { get; private set; }
    }

    public static CharSplitEnumerator SpanSplit(this string str, char c)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(str.AsSpan(), c);
    }

    public static CharSplitEnumerator SpanSplit(this ReadOnlySpan<char> span, char c)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(span, c);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct CharSplitEnumerator
    {
        private ReadOnlySpan<char> _span;
        private readonly char _c;

        public CharSplitEnumerator(ReadOnlySpan<char> span, char c)
        {
            _span = span;
            _c = c;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        public CharSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _span;
            if (span.Length == 0) // Reach the end of the string
                return false;

            var index = span.IndexOf(_c);
            if (index == -1) // The string is composed of only one line
            {
                Current = _span; // The remaining string
                _span = ReadOnlySpan<char>.Empty;
                return true;
            }

            Current = span.Slice(0, index);
            _span = span.Slice(index + 1);
            return true;

        }

        public ReadOnlySpan<char> Current { get; private set; }
    }
}