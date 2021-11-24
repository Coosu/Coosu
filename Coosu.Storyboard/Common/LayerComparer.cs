using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public class LayerComparer : IComparer<Layer>
    {
        public int Compare(Layer? x, Layer? y)
        {
            if (x == null && y == null)
                return 0;
            if (y == null)
                return 1;
            if (x == null)
                return -1;
            if (x.Camera2.DefaultZ > y.Camera2.DefaultZ)
                return 1;
            if (x.Camera2.DefaultZ < y.Camera2.DefaultZ)
                return -1;
            return 0;
        }
    }
}