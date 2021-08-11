using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Utils
{
    public static class StringExtensions
    {
        public static LineSplitEnumerator SpanSplit(this string str, char c)
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new LineSplitEnumerator(str.AsSpan(), c);
        }

        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;
            private readonly char _c;

            public LineSplitEnumerator(ReadOnlySpan<char> str, char c)
            {
                _str = str;
                _c = c;
                Current = default;
            }

            // Needed to be compatible with the foreach operator
            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOf(_c);
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                    Current = ReadOnlySpan<char>.Empty;
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


