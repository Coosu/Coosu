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
            return e.GetValue(index);
        }

        public static float GetEndsValue(this IKeyEvent e, int index)
        {
            if (e.EventType.Size < 1)
                throw new NotSupportedException("Not support the specified event size: " + e.EventType.Size);
            return e.GetValue(index + e.EventType.Size);
        }

        public static void SetStartsValue(this IKeyEvent e, int index, float value)
        {
            e.SetValue(index, value);
        }

        public static void SetEndsValue(this IKeyEvent e, int index, float value)
        {
            if (e.EventType.Size < 1)
                throw new NotSupportedException("Not support the specified event size: " + e.EventType.Size);
            e.SetValue(index + e.EventType.Size, value);
        }

        public static void SetRawValues(this IKeyEvent e, IEnumerable<float> startValues, IEnumerable<float> endValues)
        {
            e.SetStartsValues(startValues);
            e.SetEndsValues(endValues);
        }
    }
}
