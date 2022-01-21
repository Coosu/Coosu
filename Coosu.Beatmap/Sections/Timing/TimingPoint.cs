using System;
using System.Globalization;
using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.Timing
{
    public sealed class TimingPoint : SerializeWritableObject
    {
        private byte _rhythm;
        public double Offset { get; set; }
        public double Factor { get; set; } // 一拍的ms

        public double Bpm //计算属性
        {
            get => IsInherit ? -1 : Math.Round(60000d / Math.Abs(Factor), 3);
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
                    Factor = -100d / value;
                else throw new Exception("The current timing point is not inherited.");
            }
        }

        public byte Rhythm
        {
            get => _rhythm;
            set
            {
                if (value < 1) value = 1;
                _rhythm = value;
            }
        } // 1/4, 2/4, 3/4, 4/4, etc ..

        public TimingSamplesetType TimingSampleset { get; set; }
        public ushort Track { get; set; }
        public byte Volume { get; set; }
        public bool IsInherit { get; set; }
        public Effects Effects { get; set; } = Effects.None;

        [SectionIgnore]
        public bool IsKiai => (Effects & Effects.Kiai) == Effects.Kiai;
        [SectionIgnore]
        public bool IsOmitted => (Effects & Effects.OmitFirstBarLine) == Effects.OmitFirstBarLine;

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
            textWriter.Write(Offset);
            textWriter.Write(',');
            textWriter.Write(Factor.ToIcString());
            textWriter.Write(',');
            textWriter.Write(Rhythm);
            textWriter.Write(',');
            textWriter.Write((byte)TimingSampleset + 1);
            textWriter.Write(',');
            textWriter.Write(Track);
            textWriter.Write(',');
            textWriter.Write(Volume);
            textWriter.Write(',');
            textWriter.Write(IsInherit ? '0' : '1');
            textWriter.Write(',');
            textWriter.Write(IsKiai ? '1' : '0');
        }
    }
}