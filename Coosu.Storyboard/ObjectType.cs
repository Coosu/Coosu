using System;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard
{
    public struct ObjectType : IEquatable<ObjectType>, IComparable<ObjectType>, IComparable
    {
        public int Flag { get; }

        public ObjectType(int flag)
        {
            Flag = flag;
        }

        public static ObjectType Parse(string s)
        {
            var foo = ObjectTypeRegister.Parse(s);
            return foo == default ? (ObjectType)int.Parse(s) : foo;
        }

        public bool Equals(ObjectType other)
        {
            return Flag == other.Flag;
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

        public int CompareTo(ObjectType other)
        {
            return Flag.CompareTo(other.Flag);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is ObjectType other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(ObjectType)}");
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
    }
}