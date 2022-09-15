using System;
using System.Diagnostics;

namespace Coosu.Storyboard;

[DebuggerDisplay("Flag = {Flag}")]
public sealed class EventType : IEquatable<EventType>, IComparable<EventType>, IComparable
{
    public readonly string Flag;
    public readonly int Index;
    public readonly int Size;

    public EventType(string flag, int size, int index)
    {
        Flag = flag;
        Size = size;
        Index = index;
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is EventType other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(EventType)}");
    }

    public int CompareTo(EventType? other)
    {
        return string.Compare(Flag, other?.Flag, StringComparison.Ordinal);
    }

    public bool Equals(EventType? other)
    {
        return Flag == other?.Flag;
    }

    public override bool Equals(object? obj)
    {
        return obj is EventType other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Flag != null! ? Flag.GetHashCode() : 0;
    }

    public static bool operator ==(EventType left, EventType right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EventType left, EventType right)
    {
        return !left.Equals(right);
    }

    public static implicit operator string(EventType type)
    {
        return type.Flag;
    }

    public static bool operator <(EventType left, EventType right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(EventType left, EventType right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(EventType left, EventType right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(EventType left, EventType right)
    {
        return left.CompareTo(right) >= 0;
    }
}