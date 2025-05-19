using System;

namespace Coosu.Beatmap.Internal;

public static class StringExtensions
{
    public static CharSplitEnumerator SpanSplit(this string str, char c, int maxSplits)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(str.AsSpan(), c, maxSplits);
    }

    public static CharSplitEnumerator SpanSplit(this ReadOnlySpan<char> span, char c, int maxSplits)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(span, c, maxSplits);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct CharSplitEnumerator
    {
        private ReadOnlySpan<char> _span;
        private int _currentSplitCount;

        private readonly char _c;
        private readonly int _maxSplits;

        public CharSplitEnumerator(ReadOnlySpan<char> span, char c, int maxSplits)
        {
            _span = span;
            _c = c;
            _maxSplits = maxSplits;
            _currentSplitCount = 0;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        public CharSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _span;
            if (span.Length == 0) // Reach the end of the string
                return false;

            _currentSplitCount++;
            if (_maxSplits > 0 && _currentSplitCount >= _maxSplits)
            {
                Current = _span; // The remaining string
                _span = ReadOnlySpan<char>.Empty;
                return true;
            }

            var index = span.IndexOf(_c);
            if (index == -1) // The string is composed of only one line or it's the last segment
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
        public int CurrentIndex => _currentSplitCount - 1;
    }
}