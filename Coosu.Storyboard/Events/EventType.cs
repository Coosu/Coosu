using System;

namespace Coosu.Storyboard.Events
{
    public static class EventTypes
    {
        public static EventType Fade { get; } = new EventType("F");
        public static EventType Move { get; } = new EventType("M");
        public static EventType MoveX { get; } = new EventType("MX");
        public static EventType MoveY { get; } = new EventType("MY");
        public static EventType Scale { get; } = new EventType("S");
        public static EventType Vector { get; } = new EventType("V");
        public static EventType Rotate { get; } = new EventType("R");
        public static EventType Color { get; } = new EventType("C");
        public static EventType Parameter { get; } = new EventType("P");
        public static EventType Loop { get; } = new EventType("L");
        public static EventType Trigger { get; } = new EventType("T");
    }

    public struct EventType : IEquatable<EventType>, IComparable<EventType>, IComparable
    {
        public string Flag { get; }

        public EventType(string flag)
        {
            Flag = flag;
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

        public static implicit operator EventType(string flag)
        {
            return new EventType(flag);
        }

        public int CompareTo(EventType other)
        {
            return string.Compare(Flag, other.Flag, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
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

    public static class EventEnumExtension
    {
        public static string ToShortString(this EventType eventType)
        {
            return eventType;
            //switch (eventType)
            //{
            //    case EventTypes.Fade:
            //        return "F";
            //    case EventTypes.Move:
            //        return "M";
            //    case EventTypes.MoveX:
            //        return "MX";
            //    case EventTypes.MoveY:
            //        return "MY";
            //    case EventTypes.Scale:
            //        return "S";
            //    case EventTypes.Vector:
            //        return "V";
            //    case EventTypes.Rotate:
            //        return "R";
            //    case EventTypes.Color:
            //        return "C";
            //    case EventTypes.Parameter:
            //        return "P";
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            //}
        }

        public static EventType ToCommandType(this string shortHand)
        {
            return shortHand;
            //switch (shortHand)
            //{
            //    case "F": return EventTypes.Fade;
            //    case "M": return EventTypes.Move;
            //    case "MX": return EventTypes.MoveX;
            //    case "MY": return EventTypes.MoveY;
            //    case "S": return EventTypes.Scale;
            //    case "V": return EventTypes.Vector;
            //    case "R": return EventTypes.Rotate;
            //    case "C": return EventTypes.Color;
            //    case "P": return EventTypes.Parameter;
            //    case "L": return EventTypes.Loop;
            //    case "T": return EventTypes.Trigger;
            //}
            //return EventTypes.Fade;
        }
    }
}
