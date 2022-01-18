using System;

namespace Coosu.Shared
{
    public static class StringExtensions
    {
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
            private ReadOnlySpan<char> _str;
            private readonly char _c;

            public CharSplitEnumerator(ReadOnlySpan<char> str, char c)
            {
                _str = str;
                _c = c;
                Current = default;
            }

            // Needed to be compatible with the foreach operator
            public CharSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOf(_c);
                if (index == -1) // The string is composed of only one line
                {
                    Current = _str; // The remaining string
                    _str = ReadOnlySpan<char>.Empty; 
                    return true;
                }

                Current = span.Slice(0, index);
                _str = span.Slice(index + 1);
                return true;
            }

            public ReadOnlySpan<char> Current { get; private set; }
        }
    }
}


