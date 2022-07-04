using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Coosu.Database.Serialization;

namespace Coosu.Database.Internal;

internal static class StructureHelperPool
{
    internal static readonly Type TypeOsuDb = typeof(OsuDb);
    internal static readonly Type TypeCollectionDb = typeof(CollectionDb);

    private static readonly ConcurrentDictionary<Type, StructureHelper> StructureHelpers = new();

    public static StructureHelper GetHelperByType(Type type)
    {
        return StructureHelpers.GetOrAdd(type, t => new StructureHelper(t));
    }
}