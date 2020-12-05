using System;
using System.Collections.Generic;

namespace Coosu.Beatmap.Configurable
{
    internal class AttributeComparer : IEqualityComparer<Attribute>
    {
        public bool Equals(Attribute x, Attribute y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.GetType() == y.GetType();
        }

        public int GetHashCode(Attribute obj)
        {
            return obj.GetHashCode();
        }
    }
}