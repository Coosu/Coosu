using System;
using System.Text;

namespace Coosu.Beatmap;

public readonly struct MetaString
{
    private readonly bool _preferUnicode;

    public MetaString(string origin, string? unicode, bool preferUnicode = true)
    {
        if (origin == null) throw new ArgumentNullException(nameof(origin));
        Origin = GetAsciiStr(origin);
        Unicode = unicode;
        _preferUnicode = preferUnicode;
    }

    public string Origin { get; }
    public string? Unicode { get; }
    public bool IsWestern => !(_preferUnicode && Unicode != Origin && !string.IsNullOrEmpty(Unicode));

    public string ToUnicodeString()
    {
        return GetUnicode(Origin, Unicode);
    }

    public string ToOriginalString()
    {
        return GetOriginal(Origin, Unicode);
    }

    public string ToPreferredString() => _preferUnicode ? string.IsNullOrEmpty(Unicode) ? Origin : Unicode! : Origin;

    public override string ToString() => string.IsNullOrEmpty(Unicode) ? Origin : Unicode!;


    public static string GetUnicode(string origin, string? unicode)
    {
        return string.IsNullOrEmpty(unicode)
            ? (string.IsNullOrEmpty(origin) ? "" : origin)
            : unicode!;
    }

    public static string GetOriginal(string origin, string? unicode)
    {
        return string.IsNullOrEmpty(origin)
            ? (string.IsNullOrEmpty(unicode) ? "" : unicode!)
            : origin;
    }

    public static string GetAsciiStr(string? value)
    {
        if (value == null) return "";

        var sb = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (c >= 32 && c <= 126)
                sb.Append(c);
        }

        return sb.ToString();
    }
}