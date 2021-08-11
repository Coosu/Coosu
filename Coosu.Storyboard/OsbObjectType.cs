using System;
using Coosu.Storyboard.Management;

namespace Coosu.Storyboard
{
    public struct OsbObjectType : IEquatable<OsbObjectType>, IComparable<OsbObjectType>, IComparable
    {
        public int Flag { get; }

        public OsbObjectType(int flag)
        {
            Flag = flag;
        }

        public static OsbObjectType Parse(string s)
        {
            var foo = ObjectTypeManager.Parse(s);
            return foo == default ? (OsbObjectType) int.Parse(s) : foo;
        }

        public bool Equals(OsbObjectType other)
        {
            return Flag == other.Flag;
        }

        public override bool Equals(object obj)
        {
            return obj is OsbObjectType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Flag;
        }

        public static bool operator ==(OsbObjectType left, OsbObjectType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OsbObjectType left, OsbObjectType right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(OsbObjectType other)
        {
            return Flag.CompareTo(other.Flag);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is OsbObjectType other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(OsbObjectType)}");
        }

        public static bool operator <(OsbObjectType left, OsbObjectType right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(OsbObjectType left, OsbObjectType right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(OsbObjectType left, OsbObjectType right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(OsbObjectType left, OsbObjectType right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static implicit operator int(OsbObjectType type)
        {
            return type.Flag;
        }

        public static implicit operator OsbObjectType(int flag)
        {
            return new OsbObjectType(flag);
        }
    }
}