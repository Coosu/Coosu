using System;
using System.Collections.Generic;

namespace Coosu.Storyboard
{
    public static class ElementTypes
    {
        public static ElementType Background { get; } = new ElementType(0);
        public static ElementType Video { get; } = new ElementType(1);
        public static ElementType Break { get; } = new ElementType(2);
        public static ElementType Color { get; } = new ElementType(3);
        public static ElementType Sprite { get; } = new ElementType(4);
        public static ElementType Sample { get; } = new ElementType(5);
        public static ElementType Animation { get; } = new ElementType(6);
    }


    public struct ElementType : IEquatable<ElementType>, IComparable<ElementType>, IComparable
    {
        public int Flag { get; }

        public ElementType(int flag)
        {
            Flag = flag;
        }

        public static ElementType Parse(string s)
        {
            var foo = ElementTypeSign.Parse(s);
            return foo == default ? (ElementType)int.Parse(s) : foo;
        }

        public bool Equals(ElementType other)
        {
            return Flag == other.Flag;
        }

        public override bool Equals(object obj)
        {
            return obj is ElementType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Flag;
        }

        public static bool operator ==(ElementType left, ElementType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ElementType left, ElementType right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ElementType other)
        {
            return Flag.CompareTo(other.Flag);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is ElementType other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ElementType)}");
        }

        public static bool operator <(ElementType left, ElementType right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ElementType left, ElementType right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ElementType left, ElementType right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ElementType left, ElementType right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static implicit operator int(ElementType type)
        {
            return type.Flag;
        }

        public static implicit operator ElementType(int flag)
        {
            return new ElementType(flag);
        }
    }

    public static class ElementTypeSign
    {
        private static Dictionary<string, int> _inner = new Dictionary<string, int>();
        private static Dictionary<int, string> _back = new Dictionary<int, string>();

        static ElementTypeSign()
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
            if (_inner.ContainsKey(name)) return;
            _inner.Add(name, num);
            _back.Add(num, name);
        }

        public static ElementType Parse(string s)
        {
            return _inner.ContainsKey(s) ? (ElementType)_inner[s] : default;
        }

        public static string GetString(ElementType type)
        {
            return _back.ContainsKey(type) ? _back[type] : null;
        }
    }

    //public enum ElementType
    //{
    //    Background = 0,
    //    Video = 1,
    //    Break = 2,
    //    Color = 3,
    //    Sprite = 4,
    //    Sample = 5,
    //    Animation = 6
    //}
}
