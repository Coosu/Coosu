using System;

namespace Coosu.Shared;

public static class StringExtensions
{
    public static CharSplitEnumerator SpanSplit(this string str, char c, SpanSplitArgs? e = null)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(str.AsSpan(), c, e);
    }

    public static CharSplitEnumerator SpanSplit(this ReadOnlySpan<char> span, char c, SpanSplitArgs? e = null)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new CharSplitEnumerator(span, c, e);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct CharSplitEnumerator
    {
        private ReadOnlySpan<char> _span;
        private readonly char _c;
        private readonly SpanSplitArgs? _e;

        public CharSplitEnumerator(ReadOnlySpan<char> span, char c, SpanSplitArgs? e)
        {
            _span = span;
            _c = c;
            _e = e;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        public CharSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _span;
            if (span.Length == 0) // Reach the end of the string
                return false;
            if (_e is { Canceled: true })
            {
                Current = _span; // The remaining string
                _span = ReadOnlySpan<char>.Empty;
                return true;
            }

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