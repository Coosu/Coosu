using System.Collections.Generic;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Serialization;

public static partial class OsuDbReaderExtensions
{
    public static IEnumerable<Collection> EnumerateCollections(this OsuDbReader reader)
    {
        Collection? collection = default;
        int beatmapHashCount = 0;

        while (!reader.IsEndOfStream && reader.Read())
        {
            //Console.WriteLine("Name = " + GetString(reader.Name) + "; " +
            //                  "Path = " + GetString(reader.Path) + "; " +
            //                  "Value = " + GetString(reader.Value) + "; " +
            //                  "NodeType = " + GetString(reader.NodeType) + "; " +
            //                  "DataType = " + GetString(reader.DataType) + "; " +
            //                  "TargetType = " + GetString(reader.TargetType));

            if (reader.NodeType == NodeType.ObjectStart)
            {
                collection = new Collection();
                //Console.WriteLine("Collection index: " + index);
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && collection != null)
            {
                yield return collection;
                collection = default;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == 7)
            {
                yield break;
            }

            if (collection == default)
            {
                continue;
            }

            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }

            FillCollectionProperty(reader, collection, ref beatmapHashCount);
        }
    }

    private static void FillCollectionProperty(OsuDbReader reader, Collection collection, ref int beatmapHashCount)
    {
        var nodeId = reader.NodeId;
        if (nodeId == 5) collection.Name = reader.GetString();
        else if (nodeId == 6) beatmapHashCount = reader.GetInt32();
        else if (nodeId == 7)
        {
            FillBeatmapHashes(collection.BeatmapHashes = new List<string>(beatmapHashCount), reader);
        }
    }

    private static void FillBeatmapHashes(List<string> beatmapHashes, OsuDbReader reader)
    {
        while (!reader.IsEndOfStream && reader.Read())
        {
            if (reader.NodeType == NodeType.ArrayEnd) break;
            var hash = reader.GetString();
            beatmapHashes.Add(hash);
        }
    }

    //private static string GetString(object? obj)
    //{
    //    if (obj == null) return "NULL";
    //    return obj.ToString();
    //}
}