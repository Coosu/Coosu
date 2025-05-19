using System;
using System.Collections.Generic;
using Coosu.Beatmap.Configurable; // For SectionInfo
using Coosu.Beatmap.Utils; // For SpanEqualityUtil

namespace Coosu.Beatmap.Internal;

public class SectionPropertyLookup
{
    public readonly IReadOnlyDictionary<string, SectionInfo> OriginalMap;
    private readonly Dictionary<int, List<string>> _hashToKeyCandidates;

    public SectionPropertyLookup(IReadOnlyDictionary<string, SectionInfo> originalMap)
    {
        OriginalMap = originalMap;
        _hashToKeyCandidates = new Dictionary<int, List<string>>();

        foreach (var kvp in originalMap)
        {
            var key = kvp.Key;
            var hashCode = SpanEqualityUtil.GetHashCode(key.AsSpan());
            if (!_hashToKeyCandidates.TryGetValue(hashCode, out var list))
            {
                list = new List<string>(1);
                _hashToKeyCandidates[hashCode] = list;
            }

            list.Add(key);
        }
    }

    public bool TryGetValue(ReadOnlySpan<char> keySpan, out SectionInfo? sectionInfo)
    {
        var keySpanHash = SpanEqualityUtil.GetHashCode(keySpan);
        if (!_hashToKeyCandidates.TryGetValue(keySpanHash, out var candidateStringKeys))
        {
            sectionInfo = null;
            return false;
        }

        foreach (var candidateKey in candidateStringKeys)
        {
#if !NETCOREAPP3_1_OR_GREATER
            if (keySpan.SequenceEqual(candidateKey.AsSpan()))
#else
            if (keySpan.Equals(candidateKey, StringComparison.Ordinal))
#endif
            {
                return OriginalMap.TryGetValue(candidateKey, out sectionInfo);
            }
        }

        sectionInfo = null;
        return false;
    }
}