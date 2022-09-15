using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Coosu.Storyboard;

[DebuggerDisplay("Name = {DebuggerDisplay}")]
public sealed class ObjectType : IEquatable<ObjectType>, IComparable<ObjectType>, IComparable
{
    public readonly int Flag;

    public ObjectType(int flag)
    {
        Flag = flag;
    }

    private string? DebuggerDisplay => ObjectType.GetString(this);

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is ObjectType other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(ObjectType)}");
    }

    public int CompareTo(ObjectType other)
    {
        return Flag.CompareTo(other.Flag);
    }

    public bool Equals(ObjectType? other)
    {
        return Flag == other?.Flag;
    }

    public override bool Equals(object obj)
    {
        return obj is ObjectType other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Flag;
    }

    public static bool operator ==(ObjectType left, ObjectType right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ObjectType left, ObjectType right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(ObjectType left, ObjectType right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(ObjectType left, ObjectType right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(ObjectType left, ObjectType right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(ObjectType left, ObjectType right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static implicit operator int(ObjectType type)
    {
        return type.Flag;
    }

    public static implicit operator ObjectType(int flag)
    {
        return new ObjectType(flag);
    }

    #region static members

    private static readonly Dictionary<string, ObjectType> DictionaryStore = new(/*StringComparer.OrdinalIgnoreCase*/);
    private static readonly Dictionary<ObjectType, string> BackDictionaryStore = new();

    static ObjectType()
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
        DictionaryStore.Add(num.ToString(), num);
        BackDictionaryStore.Add(num, name);
    }

    public static ObjectType Parse(string s)
    {
        return DictionaryStore.ContainsKey(s) ? DictionaryStore[s] : int.Parse(s);
    }

    public static string? GetString(ObjectType type)
    {
        return BackDictionaryStore.ContainsKey(type) ? BackDictionaryStore[type] : null;
    }

    public static bool Contains(string name)
    {
        return DictionaryStore.ContainsKey(name);
    }

    #endregion
}