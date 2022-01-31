using System;
using System.Collections.Generic;
using Coosu.Storyboard.Common;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class KeyEventExtensions
    {
        public static float GetStartsValue(this IKeyEvent e, int index)
        {
            //#if NET5_0_OR_GREATER
            //            return e.GetStartsSpan()[index];
            //#else
            return e.GetValue(index);
            //#endif
        }

        public static float GetEndsValue(this IKeyEvent e, int index)
        {
            if (e.EventType.Size < 1)
                throw new NotSupportedException("Not support the specified event size: " + e.EventType.Size);
            //#if NET5_0_OR_GREATER
            //            return e.GetEndsSpan()[index];
            //#else
            return e.GetValue(index + e.EventType.Size);
            //#endif
        }

        public static void SetStartsValue(this IKeyEvent e, int index, float value)
        {
            //#if NET5_0_OR_GREATER
            //            e.GetStartsSpan()[index] = value;
            //#else
            e.SetValue(index, value);
            //#endif
        }

        public static void SetEndsValue(this IKeyEvent e, int index, float value)
        {
            if (e.EventType.Size < 1)
                throw new NotSupportedException("Not support the specified event size: " + e.EventType.Size);
            //#if NET5_0_OR_GREATER
            //            e.GetEndsSpan()[index] = value;
            //#else
            e.SetValue(index + e.EventType.Size, value);
        }

        public static void SetRawValues(this IKeyEvent e, IEnumerable<float> startValues, IEnumerable<float> endValues)
        {
            e.SetStarts(startValues);
            e.SetEnds(endValues);
        }
    }
}
