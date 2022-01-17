using System.Linq;

namespace Coosu.Beatmap
{
    public struct MetaString
    {
        public string Origin { get; }
        public string Unicode { get; }
        private readonly bool _preferUnicode;

        public bool IsWestern => !(_preferUnicode && Unicode != Origin && !string.IsNullOrEmpty(Unicode));

        public MetaString(string origin, string unicode) :
            this(origin, unicode, true)
        {
        }

        public MetaString(string origin, string unicode, bool preferUnicode)
        {
            Origin = origin == null ? null : new string(origin.Where(k => k <= 126 || k >= 32).ToArray());
            Unicode = unicode;
            _preferUnicode = preferUnicode;
        }

        public string ToUnicodeString()
        {
            return GetUnicode(Origin, Unicode);
        }

        public string ToOriginalString()
        {
            return GetOriginal(Origin, Unicode);
        }

        public string ToPreferredString() => _preferUnicode ? string.IsNullOrEmpty(Unicode) ? Origin : Unicode : Origin;

        public override string ToString() => string.IsNullOrEmpty(Unicode) ? Origin : Unicode;


        public static string GetUnicode(string origin, string unicode)
        {
            return string.IsNullOrEmpty(unicode)
                ? (string.IsNullOrEmpty(origin) ? default : origin)
                : unicode;
        }

        public static string GetOriginal(string origin, string unicode)
        {
            return string.IsNullOrEmpty(origin)
                ? (string.IsNullOrEmpty(unicode) ? default : unicode)
                : origin;
        }
    }
}
