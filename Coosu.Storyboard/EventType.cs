using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Coosu.Storyboard
{
    [DebuggerDisplay("Flag = {Flag}")]
    public struct EventType : IEquatable<EventType>, IComparable<EventType>, IComparable
    {
        public string Flag { get; }
        public int Size { get; }
        public int Index { get; }

        public EventType(string flag, int size, int index)
        {
            Flag = flag;
            Size = size;
            Index = index;
        }
        
        public bool Equals(EventType other)
        {
            return Flag == other.Flag;
        }

        public override bool Equals(object obj)
        {
            return obj is EventType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Flag != null ? Flag.GetHashCode() : 0);
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

        //public static implicit operator EventType(string flag)
        //{
        //    return new EventType(flag);
        //}

        public int CompareTo(EventType other)
        {
            return string.Compare(Flag, other.Flag, StringComparison.Ordinal);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is EventType other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(EventType)}");
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

    //public enum EventType
    //{
    //    Move, Fade, Scale, Rotate, Color, MoveX, MoveY, Vector, Parameter, Loop, Trigger
    //}
}
