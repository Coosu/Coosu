using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.GamePlay;
using System;
using System.Globalization;
using System.IO;

namespace Coosu.Beatmap.Sections.Timing
{
    public class TimingPoint : SerializeWritableObject
    {
        public bool Positive { get; set; }
        public double Offset { get; set; }
        public double Factor { get; set; } // 一拍的ms
        public double Bpm //计算属性
        {
            get => Inherit ? -1 : Math.Round(60000d / Factor, 3);
            set
            {
                if (!Inherit)
                    Factor = 60000d / value;
                else throw new Exception("The current timing point is inherited.");
            }
        }
        public double Multiple //计算属性
        {
            get => Inherit ? Math.Round(100d / Math.Abs(Factor), 2) : 1;
            set
            {
                if (Inherit)
                    Factor = Positive ? 100d / value : -100d / value;
                else throw new Exception("The current timing point is not inherited.");
            }
        }

        public int Rhythm
        {
            get => _rhythm;
            set
            {
                if (value < 1 || value > 7) value = 4; //此处待定
                _rhythm = value;
            }
        } // 1/4, 2/4, 3/4, 4/4, etc ..
        public TimingSamplesetType TimingSampleset { get; set; }
        public int Track { get; set; }
        public int Volume { get; set; }
        public bool Inherit { get; set; }
        public bool Kiai { get; set; }

        private int _rhythm;

        public override string ToString() =>
            string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                Offset,
                Factor.ToString(CultureInfo.InvariantCulture),
                Rhythm,
                (int) TimingSampleset + 1,
                Track,
                Volume,
                Convert.ToInt32(!Inherit),
                Convert.ToInt32(Kiai));

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write($"{Offset},");
            textWriter.Write($"{Factor.ToString(CultureInfo.InvariantCulture)},");
            textWriter.Write($"{Rhythm},");
            textWriter.Write($"{(int)TimingSampleset + 1},");
            textWriter.Write($"{Track},");
            textWriter.Write($"{Volume},");
            textWriter.Write($"{Convert.ToInt32(!Inherit)},");
            textWriter.WriteLine(Convert.ToInt32(Kiai));
        }
    }
}
