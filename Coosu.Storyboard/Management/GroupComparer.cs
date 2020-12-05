using System.Collections.Generic;

namespace Coosu.Storyboard.Management
{
    public class GroupComparer : IComparer<ElementGroup>
    {
        public int Compare(ElementGroup x, ElementGroup y)
        {
            if (x == null && y == null)
                return 0;
            if (y == null)
                return 1;
            if (x == null)
                return -1;
            if (x.Index > y.Index)
                return 1;
            if (x.Index < y.Index)
                return -1;
            return 0;
        }
    }
}