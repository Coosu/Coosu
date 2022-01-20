using System;
using System.Globalization;
using System.IO;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections.Timing
{
    public sealed class TimingPoint : SerializeWritableObject
    {
        private byte _rhythm;

        public bool Positive { get; set; }
        public double Offset { get; set; }
        public double Factor { get; set; } // 一拍的ms

        public double Bpm //计算属性
        {
            get => IsInherit ? -1 : Math.Round(60000d / Factor, 3);
            set
            {
                if (!IsInherit)
                    Factor = 60000d / value;
                else throw new Exception("The current timing point is inherited.");
            }
        }

        public double Multiple //计算属性
        {
            get => IsInherit ? Math.Round(100d / Math.Abs(Factor), 2) : 1;
            set
            {
                if (IsInherit)
                    Factor = Positive ? 100d / value : -100d / value;
                else throw new Exception("The current timing point is not inherited.");
            }
        }

        public byte Rhythm
        {
            get => _rhythm;
            set
            {
                if (value is < 1 or > 7) value = 4; //此处待定
                _rhythm = value;
            }
        } // 1/4, 2/4, 3/4, 4/4, etc ..

        public TimingSamplesetType TimingSampleset { get; set; }
        public ushort Track { get; set; }
        public byte Volume { get; set; }
        public bool IsInherit { get; set; }
        public bool IsKiai { get; set; }

        public override string ToString() =>
            string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                Offset,
                Factor.ToString(CultureInfo.InvariantCulture),
                Rhythm,
                (int)TimingSampleset + 1,
                Track,
                Volume,
                Convert.ToInt32(!IsInherit),
                Convert.ToInt32(IsKiai));

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write($"{Offset},");
            textWriter.Write($"{Factor.ToString(CultureInfo.InvariantCulture)},");
            textWriter.Write($"{Rhythm},");
            textWriter.Write($"{(int)TimingSampleset + 1},");
            textWriter.Write($"{Track},");
            textWriter.Write($"{Volume},");
            textWriter.Write($"{Convert.ToInt32(!IsInherit)},");
            textWriter.WriteLine(Convert.ToInt32(IsKiai));
        }
    }
}