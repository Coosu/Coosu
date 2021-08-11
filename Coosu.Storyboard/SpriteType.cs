using System;

namespace Coosu.Storyboard
{
    public struct SpriteType : IEquatable<SpriteType>, IComparable<SpriteType>, IComparable
    {
        public int Flag { get; }

        public SpriteType(int flag)
        {
            Flag = flag;
        }

        public static SpriteType Parse(string s)
        {
            var foo = SpriteTypeManager.Parse(s);
            return foo == default ? (SpriteType) int.Parse(s) : foo;
        }

        public bool Equals(SpriteType other)
        {
            return Flag == other.Flag;
        }

        public override bool Equals(object obj)
        {
            return obj is SpriteType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Flag;
        }

        public static bool operator ==(SpriteType left, SpriteType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpriteType left, SpriteType right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(SpriteType other)
        {
            return Flag.CompareTo(other.Flag);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is SpriteType other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(SpriteType)}");
        }

        public static bool operator <(SpriteType left, SpriteType right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(SpriteType left, SpriteType right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(SpriteType left, SpriteType right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(SpriteType left, SpriteType right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static implicit operator int(SpriteType type)
        {
            return type.Flag;
        }

        public static implicit operator SpriteType(int flag)
        {
            return new SpriteType(flag);
        }
    }
}