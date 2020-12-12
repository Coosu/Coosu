using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Events.Extension
{
    public class ZoomIn : CommonEvent
    {
        public ZoomIn()
        {
        }

        public ZoomIn(EasingType easing, float startTime, float endTime, float[] start, float[] end) : base(easing, startTime, endTime, start, end)
        {
        }

        public override EventType EventType => "ZI";
    }
    public class ZoomOut : CommonEvent
    {
        public ZoomOut()
        {
        }

        public ZoomOut(EasingType easing, float startTime, float endTime, float[] start, float[] end) : base(easing, startTime, endTime, start, end)
        {
        }

        public override EventType EventType => "ZO";
    }
}
