using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared.Mathematics;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("TimingPoints")]
    public class TimingSection : Section
    {
        public List<TimingPoint> TimingList { get; set; } = new();
        public double MinTime => TimingList.Count == 0 ? 0 : TimingList.Min(t => t.Offset);
        public double MaxTime => TimingList.Count == 0 ? 0 : TimingList.Max(t => t.Offset);

        public TimingPoint this[int index] => TimingList[index];

        public override void Match(string line)
        {
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
