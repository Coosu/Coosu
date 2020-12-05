using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("TimingPoints")]
    public class TimingSection : Section
    {
        public List<TimingPoint> TimingList { get; set; }
        public double MinTime => TimingList.Count == 0 ? 0 : TimingList.Min(t => t.Offset);
        public double MaxTime => TimingList.Count == 0 ? 0 : TimingList.Max(t => t.Offset);

        public TimingPoint this[int index] => TimingList[index];

        public override void Match(string line)
        {
            if (TimingList == null)
                TimingList = new List<TimingPoint>();

            string[] param = line.Split(',');
            double offset = double.Parse(param[0]);
            double factor = double.Parse(param[1]);
            int rhythm = int.Parse(param[2]);
            var timingSampleset = param.Length > 3
                ? (TimingSamplesetType)(int.Parse(param[3]) - 1)
                : TimingSamplesetType.None;
            var track = param.Length > 4 ? int.Parse(param[4]) : 0;
            var volume = param.Length > 5 ? int.Parse(param[5]) : 0;
            var inherit = param.Length > 6 && !Convert.ToBoolean(int.Parse(param[6]));
            var kiai = param.Length > 7 && Convert.ToBoolean(int.Parse(param[7]));
            var positive = factor >= 0;
            TimingList.Add(new TimingPoint
            {
                Offset = offset,
                Factor = factor,
                Rhythm = rhythm,
                TimingSampleset = timingSampleset,
                Track = track,
                Volume = volume,
                Inherit = inherit,
                Kiai = kiai,
                Positive = positive
            });
        }

        /// <summary>
        /// 获取当前bpm的节奏的间隔
        /// </summary>
        /// <param name="multiple">multiple: 1, 0.5, 1/3d, etc.</param>
        /// <returns></returns>
        public Dictionary<double, double> GetInterval(double multiple)
        {
            return TimingList.Where(t => !t.Inherit).OrderBy(t => t.Offset)
                .ToDictionary(k => k.Offset, v => 60000 / v.Bpm * multiple);
        }

        public double[] GetTimings(double multiple)
        {
            var array = TimingList.Where(t => !t.Inherit).OrderBy(t => t.Offset).ToArray();
            var list = new List<double>();

            for (int i = 0; i < array.Length; i++)
            {
                decimal nextTime = Convert.ToDecimal(i == array.Length - 1 ? MaxTime : array[i + 1].Offset);
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

        public TimingPoint GetRedLine(double offset)
        {
            TimingPoint[] points = TimingList.Where(t => !t.Inherit).Where(t => Math.Abs(t.Offset - offset) < 1).ToArray();
            return points.Length == 0 ? TimingList.First(t => !t.Inherit) : points.Last();
        }
        public TimingPoint GetLine(double offset)
        {
            var lines = TimingList.Where(t => t.Offset <= offset + 1/*tolerance*/).ToArray();
            if (lines.Length == 0)
                return TimingList.First();
            var timing = lines.Max(t => t.Offset);
            var samePositionPoints = TimingList.Where(t => Math.Abs(t.Offset - timing) < 1).ToArray();
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

        public double[] GetTimingBars()
        {
            var array = TimingList.Where(t => !t.Inherit).OrderBy(t => t.Offset).ToArray();
            var list = new List<double>();

            for (int i = 0; i < array.Length; i++)
            {
                decimal nextTime = Convert.ToDecimal(i == array.Length - 1 ? MaxTime : array[i + 1].Offset);
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

        public RangeValue<double>[] GetTimingKiais()
        {
            var array = TimingList;
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
                list.Add(new RangeValue<double>(tmpKiai.Value, MaxTime));
            return list.ToArray();
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.WriteLine($"[{SectionName}]");
            foreach (var timingPoint in TimingList)
            {
                timingPoint.AppendSerializedString(textWriter);
            }
        }
    }
}
