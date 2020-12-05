using System.IO;

namespace Coosu.Beatmap.MetaData
{
    public static class MapIdentifiableExtension
    {
        public static bool IsMapTemporary(this IMapIdentifiable map)
        {
            return Path.IsPathRooted(map.FolderName);
        }
    }
}