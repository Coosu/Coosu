using System.Collections.Generic;

namespace Coosu.Storyboard.Management
{
    public class GroupComparer : IComparer<VirtualLayer>
    {
        public int Compare(VirtualLayer x, VirtualLayer y)
        {
            if (x == null && y == null)
                return 0;
            if (y == null)
                return 1;
            if (x == null)
                return -1;
            if (x.ZDistance > y.ZDistance)
                return 1;
            if (x.ZDistance < y.ZDistance)
                return -1;
            return 0;
        }
    }
}