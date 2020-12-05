using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Animation.WPF
{
    public static class Origins
    {
        public static Origin<double> TopLeft => new Origin<double>(0, 0);
        public static Origin<double> TopCenter => new Origin<double>(0.5, 0);
        public static Origin<double> TopRight => new Origin<double>(1, 0);
        public static Origin<double> CenterLeft => new Origin<double>(0, 0.5);
        public static Origin<double> Center => new Origin<double>(0.5, 0.5);
        public static Origin<double> CenterRight => new Origin<double>(1, 0.5);
        public static Origin<double> BottomLeft => new Origin<double>(0, 1);
        public static Origin<double> BottomCenter => new Origin<double>(0.5, 1);
        public static Origin<double> BottomRight => new Origin<double>(1, 1);
    }
}
