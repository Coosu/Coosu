using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Coosu.Storyboard
{
    [DebuggerDisplay("Flag = {Flag}")]
    public struct EventType : IEquatable<EventType>, IComparable<EventType>, IComparable
    {
        #region static members

        private static readonly Dictionary<string, EventType> DictionaryStore = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, EventType> NonCommonDictionaryStore = new(StringComparer.OrdinalIgnoreCase);

        static EventType()
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

        public static void SignType(EventType type)
        {
            if (DictionaryStore.ContainsKey(type.Flag)) return;
            DictionaryStore.Add(type.Flag, type);
            if (type.Size < 0) NonCommonDictionaryStore.Add(type.Flag, type);
        }

        public static void SignType(string flag, int length)
        {
            if (DictionaryStore.ContainsKey(flag)) return;
            DictionaryStore.Add(flag, new EventType(flag, length));
        }

        public static EventType GetValue(string flag)
        {
            return DictionaryStore.TryGetValue(flag, out var val) ? val : default;
        }

        public static bool Contains(string flag)
        {
            return DictionaryStore.ContainsKey(flag);
        }

        public static bool IsCommonEvent(string flag)
        {
            return !NonCommonDictionaryStore.ContainsKey(flag);
        }

        #endregion

        public string Flag { get; }
        public int Size { get; }

        public EventType(string flag, int size)
        {
            Flag = flag;
            Size = size;
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
