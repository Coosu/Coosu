using System;
using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.HitObject
{
    public sealed class RawHitObject : SerializeWritableObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Offset { get; set; }
        public RawObjectType RawType { get; set; }

        public HitObjectType ObjectType
        {
            get
            {
                if ((RawType & RawObjectType.Circle) == RawObjectType.Circle)
                    return HitObjectType.Circle;
                if ((RawType & RawObjectType.Slider) == RawObjectType.Slider)
                    return HitObjectType.Slider;
                if ((RawType & RawObjectType.Spinner) == RawObjectType.Spinner)
                    return HitObjectType.Spinner;
                if ((RawType & RawObjectType.Hold) == RawObjectType.Hold)
                    return HitObjectType.Hold;
                return HitObjectType.Circle;
            }
        }

        public int NewComboCount
        {
            get
            {
                int ncBase = 0;
                if ((RawType & RawObjectType.NewCombo) == RawObjectType.NewCombo)
                    ncBase = 1;
                var newThing = 0b01110000 & (byte)RawType;
                var ncCount = newThing >> 4;
                return ncCount + ncBase;
            }
        }

        public HitsoundType Hitsound { get; set; }
        public ExtendedSliderInfo? SliderInfo { get; set; }
        public int HoldEnd { get; set; }

        public ObjectSamplesetType SampleSet { get; set; }

        public ObjectSamplesetType AdditionSet { get; set; }

        public ushort CustomIndex { get; set; }

        public byte SampleVolume { get; set; }

        public string? FileName { get; set; }

        internal void SetExtras(ReadOnlySpan<char> extraInfo)
        {
            int i = -1;
            foreach (var span in extraInfo.SpanSplit(':'))
            {
                i++;
                switch (i)
                {
#if NETCOREAPP3_1_OR_GREATER
                    case 0: SampleSet = (ObjectSamplesetType)byte.Parse(span); break;
                    case 1: AdditionSet = (ObjectSamplesetType)byte.Parse(span); break;
                    case 2: CustomIndex = ushort.Parse(span); break;
                    case 3: SampleVolume = byte.Parse(span); break;
#else
                    case 0: SampleSet = (ObjectSamplesetType)byte.Parse(span.ToString()); break;
                    case 1: AdditionSet = (ObjectSamplesetType)byte.Parse(span.ToString()); break;
                    case 2: CustomIndex = ushort.Parse(span.ToString()); break;
                    case 3: SampleVolume = byte.Parse(span.ToString()); break;
#endif
                    case 4: FileName = span.Trim().ToString(); break;
                }
            }
        }
        
        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write(X);
            textWriter.Write(',');
            textWriter.Write(Y);
            textWriter.Write(',');
            textWriter.Write(Offset);
            textWriter.Write(',');
            textWriter.Write((byte)RawType);
            textWriter.Write(',');
            textWriter.Write((byte)Hitsound);
            textWriter.Write(',');
            if (ObjectType == HitObjectType.Slider && SliderInfo != null)
            {
                SliderInfo.AppendSerializedString(textWriter);
                textWriter.Write(',');
            }
            else if (ObjectType == HitObjectType.Spinner)
            {
                textWriter.Write(HoldEnd);
                textWriter.Write(',');
            }
            else if (ObjectType == HitObjectType.Hold)
            {
                textWriter.Write(HoldEnd);
                textWriter.Write(':');
            }

            textWriter.Write((byte)SampleSet);
            textWriter.Write(':');
            textWriter.Write((byte)AdditionSet);
            textWriter.Write(':');
            textWriter.Write(CustomIndex);
            textWriter.Write(':');
            textWriter.Write(SampleVolume);
            textWriter.Write(':');
            textWriter.Write(FileName);
        }
    }
}
