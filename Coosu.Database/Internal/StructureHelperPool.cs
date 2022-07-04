using System;
using System.Collections.Concurrent;
using Coosu.Database.Serialization;

namespace Coosu.Database.Internal;

internal static class StructureHelperPool
{
    internal static readonly Type TypeOsuDb = typeof(OsuDb);
    internal static readonly Type TypeCollectionDb = typeof(CollectionDb);
    internal static readonly Type TypeScoresDb = typeof(ScoresDb);

    private static readonly ConcurrentDictionary<Type, StructureHelper> StructureHelpers = new();

    public static StructureHelper GetHelperByType(Type type)
    {
        return StructureHelpers.GetOrAdd(type, t => new StructureHelper(t));
    }
}