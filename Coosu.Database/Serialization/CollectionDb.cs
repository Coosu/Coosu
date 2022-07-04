using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Database.Annotations;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Serialization;

public class CollectionDb
{
    public int OsuVersion { get; set; }
    internal int CollectionCount => Collections?.Count ?? 0;

    [StructureArray(typeof(Collection), nameof(CollectionCount),
        SubDataType = DataType.Object)]
    public List<Collection> Collections { get; set; } = new();

    public static CollectionDb ReadFromFile(string path)
    {
        return ReadFromStream(File.OpenRead(path));
    }

    public static CollectionDb ReadFromStream(Stream stream)
    {
        var collectionDb = new CollectionDb();
        using var reader = new OsuDbReader(stream, StructureHelperPool.TypeCollectionDb);

        int itemIndex = -1;
        int collectionCount = 0;
        while (!reader.IsEndOfStream && reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;

            if (nodeType is NodeType.ArrayStart or NodeType.KeyValue)
            {
                itemIndex++;
            }
            else
            {
                continue;
            }

            if (itemIndex == 0) collectionDb.OsuVersion = reader.GetInt32();
            else if (itemIndex == 1) collectionCount = reader.GetInt32();
            else if (itemIndex == 2)
            {
                collectionDb.Collections.Capacity = collectionCount;
                collectionDb.Collections.AddRange(reader.EnumerateCollections());
            }
        }

        return collectionDb;
    }
}