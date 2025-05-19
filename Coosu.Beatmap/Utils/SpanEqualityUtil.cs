using System;

namespace Coosu.Beatmap.Utils;

internal static class SpanEqualityUtil
{
    public static int GetHashCode(ReadOnlySpan<char> span)
    {
        // A simple hash algorithm example, a more optimal one can be chosen if needed.
        // e.g., a variant of FNV-1a
        int hashCode = 0;
        unchecked // Overflow is fine, just wrap
        {
            // Using a common prime number for hashing
            const int HASH_PRIME = 397;
            foreach (char c in span)
            {
                hashCode = (hashCode * HASH_PRIME) ^ c;
            }
        }

        return hashCode;
    }
}