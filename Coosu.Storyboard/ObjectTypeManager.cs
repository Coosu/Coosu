using System.Collections.Generic;

namespace Coosu.Storyboard
{
    public static class ObjectTypeManager
    {
        private static readonly Dictionary<string, ObjectType> DictionaryStore = new();
        private static readonly Dictionary<ObjectType, string> BackDictionaryStore = new();

        static ObjectTypeManager()
        {
            SignType(0, "Background");
            SignType(1, "Video");
            SignType(2, "Break");
            SignType(3, "Color");
            SignType(4, "Sprite");
            SignType(5, "Sample");
            SignType(6, "Animation");
        }

        public static void SignType(int num, string name)
        {
            if (DictionaryStore.ContainsKey(name)) return;
            DictionaryStore.Add(name, num);
            BackDictionaryStore.Add(num, name);
        }

        public static ObjectType Parse(string s)
        {
            return DictionaryStore.ContainsKey(s) ? DictionaryStore[s] : default;
        }

        public static string? GetString(ObjectType type)
        {
            return BackDictionaryStore.ContainsKey(type) ? BackDictionaryStore[type] : null;
        }
    }
}