using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;

// ReSharper disable once CheckNamespace
namespace Coosu.Beatmap
{
    public static class TimingExtensions
    {
        /// <summary>
        /// 获取当前bpm的节奏的间隔
        /// </summary>
        /// <param name="multiple">multiple: 1, 0.5, 1/3d, etc.</param>
        /// <returns></returns>
        public static Dictionary<double, double> GetInterval(this TimingSection timingSection, double multiple)
        {
            return timingSection.TimingList
                .Where(t => !t.Inherit)
                .OrderBy(t => t.Offset)
                .ToDictionary(k => k.Offset, v => 60000 / v.Bpm * multiple);
        }

        public static double[] GetTimings(this TimingSection timingSection, double multiple)
        {
            var array = timingSection.TimingList
                .Where(t => !t.Inherit)
                .OrderBy(t => t.Offset)
                .ToArray();
            var list = new List<double>();

            for (int i = 0; i < array.Length; i++)
            {
                decimal nextTime =
                    Convert.ToDecimal(i == array.Length - 1 ? timingSection.MaxTime : array[i + 1].Offset);
                var t = array[i];
                decimal decBpm = Convert.ToDecimal(t.Bpm);
                decimal decMult = Convert.ToDecimal(multiple);
                decimal interval = 60000 / decBpm * decMult;
                decimal current = Convert.ToDecimal(t.Offset);
                while (current < nextTime)
                {
                    list.Add(Convert.ToDouble(current));
                    current += interval;
                }
            }

            return list.ToArray();
        }

        public static TimingPoint GetRedLine(this TimingSection timingSection, double offset)
        {
            TimingPoint[] points = timingSection.TimingList
                .Where(t => !t.Inherit)
                .Where(t => Math.Abs(t.Offset - offset) < 1)
                .ToArray();
            return points.Length == 0 ? timingSection.TimingList.First(t => !t.Inherit) : points.Last();
        }

        public static TimingPoint GetLine(this TimingSection timingSection, double offset)
        {
            var lines = timingSection.TimingList
                .Where(t => t.Offset <= offset + 1 /*tolerance*/)
                .ToArray();
            if (lines.Length == 0)
                return timingSection.TimingList.First();
            var timing = lines.Max(t => t.Offset);
            var samePositionPoints = timingSection.TimingList
                .Where(t => Math.Abs(t.Offset - timing) < 1).
                ToArray();
            TimingPoint point;
            if (samePositionPoints.Length > 1)
            {
                var greens = samePositionPoints.Where(k => k.Inherit);
                point = greens.LastOrDefault() ?? samePositionPoints.Last();
                // there might be possibility that two red lines at same time
            }
            else
                point = samePositionPoints[0];

            return point;
        }

        public static double[] GetTimingBars(this TimingSection timingSection)
        {
            var array = timingSection.TimingList
                .Where(t => !t.Inherit)
                .OrderBy(t => t.Offset)
                .ToArray();
            var list = new List<double>();

            for (int i = 0; i < array.Length; i++)
            {
                decimal nextTime =
                    Convert.ToDecimal(i == array.Length - 1 ? timingSection.MaxTime : array[i + 1].Offset);
                var t = array[i];
                decimal decBpm = Convert.ToDecimal(t.Bpm);
                decimal decMult = Convert.ToDecimal(t.Rhythm);
                decimal interval = 60000 / decBpm * decMult;
                decimal current = Convert.ToDecimal(t.Offset);
                while (current < nextTime)
                {
                    list.Add(Convert.ToDouble(current));
                    current += interval;
                }
            }

            return list.ToArray();
        }

        public static RangeValue<double>[] GetTimingKiais(this TimingSection timingSection)
        {
            var array = timingSection.TimingList;
            var list = new List<RangeValue<double>>();
            double? tmpKiai = null;
            foreach (var t in array)
            {
                if (t.Kiai && tmpKiai == null)
                    tmpKiai = t.Offset;
                else if (!t.Kiai && tmpKiai != null)
                {
                    list.Add(new RangeValue<double>(tmpKiai.Value, t.Offset));
                    tmpKiai = null;
                }
            }

            if (tmpKiai != null)
                list.Add(new RangeValue<double>(tmpKiai.Value, timingSection.MaxTime));
            return list.ToArray();
        }
    }
}