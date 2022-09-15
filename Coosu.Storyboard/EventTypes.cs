using System.Collections.Generic;

namespace Coosu.Storyboard;

public static class EventTypes
{
    private static readonly Dictionary<string, EventType> DictionaryStore = new(/*StringComparer.OrdinalIgnoreCase*/);
    private static readonly Dictionary<int, EventType> DictionaryStoreIndex = new();
    private static readonly Dictionary<string, EventType> NonBasicDictionaryStore = new(/*StringComparer.OrdinalIgnoreCase*/);

    static EventTypes()
    {
        SignType(EventTypes.Fade);
        SignType(EventTypes.Move);
        SignType(EventTypes.MoveX);
        SignType(EventTypes.MoveY);
        SignType(EventTypes.Scale);
        SignType(EventTypes.Vector);
        SignType(EventTypes.Rotate);
        SignType(EventTypes.Color);
        SignType(EventTypes.Parameter);
        SignType(EventTypes.Loop);
        SignType(EventTypes.Trigger);
    }

    public static EventType Loop { get; } = new("L", -1, 0);
    public static EventType Move { get; } = new("M", 2, 1);
    public static EventType Fade { get; } = new("F", 1, 2);
    public static EventType Scale { get; } = new("S", 1, 3);
    public static EventType Rotate { get; } = new("R", 1, 4);
    public static EventType Color { get; } = new("C", 3, 5);
    public static EventType MoveX { get; } = new("MX", 1, 6);
    public static EventType MoveY { get; } = new("MY", 1, 7);
    public static EventType Vector { get; } = new("V", 2, 8);
    public static EventType Parameter { get; } = new("P", 0, 9);
    public static EventType Trigger { get; } = new("T", -1, 10);

    public static void SignType(EventType type)
    {
        if (DictionaryStore.ContainsKey(type.Flag)) return;
        DictionaryStore.Add(type.Flag, type);
        DictionaryStore.Add(type.Index.ToString(), type);
        DictionaryStoreIndex.Add(type.Index, type);
        if (type.Size < 0)
        {
            NonBasicDictionaryStore.Add(type.Index.ToString(), type);
            NonBasicDictionaryStore.Add(type.Flag, type);
        }
    }

    public static void SignType(string flag, int size, int index)
    {
        if (DictionaryStore.ContainsKey(flag)) return;
        var type = new EventType(flag, size, index);
        DictionaryStoreIndex.Add(index, type);
        DictionaryStore.Add(index.ToString(), type);
        DictionaryStore.Add(flag, type);
    }

    public static EventType? GetValue(string flag)
    {
        return DictionaryStore.TryGetValue(flag, out var val) ? val : default;
    }

    public static EventType? GetValue(int index)
    {
        return DictionaryStoreIndex.TryGetValue(index, out var val) ? val : default;
    }

    public static bool Contains(string flag)
    {
        return DictionaryStore.ContainsKey(flag);
    }

    public static bool IsBasicEvent(string flag)
    {
        return !NonBasicDictionaryStore.ContainsKey(flag);
    }
}