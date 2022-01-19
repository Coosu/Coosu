using System;
using System.Collections.Generic;

namespace Coosu.Beatmap.Configurable
{
    public class AttributeTypeComparer : IEqualityComparer<Attribute>
    {
        public bool Equals(Attribute? x, Attribute? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            var @equals = x.GetType() == y.GetType();
            return @equals;
        }

        public int GetHashCode(Attribute obj)
        {
            return obj.GetHashCode();
        }
    }
}