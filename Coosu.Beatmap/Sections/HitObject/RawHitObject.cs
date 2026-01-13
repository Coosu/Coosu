using System;
using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections.HitObject;

public sealed class RawHitObject : SerializeWritableObject
{
    public float X { get; set; }
    public float Y { get; set; }
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
            {
                ncBase = 1;
            }

            var newThing = 0b01110000 & (byte)RawType;
            var ncCount = newThing >> 4;
            return ncCount + ncBase;
        }
    }

    public HitsoundType Hitsound { get; set; }
    public int HoldEnd { get; set; }
    public ObjectSamplesetType SampleSet { get; set; }
    public ObjectSamplesetType AdditionSet { get; set; }
    public ushort CustomIndex { get; set; }
    public byte SampleVolume { get; set; }
    public string? FileName { get; set; }
    public ExtendedSliderInfo? SliderInfo { get; set; }

    internal void SetExtras(ReadOnlySpan<char> extraInfo)
    {
        var enumerator = extraInfo.SpanSplit(':');
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex)
            {
                case 0: SampleSet = (ObjectSamplesetType)ParseHelper.ParseByte(span); break;
                case 1: AdditionSet = (ObjectSamplesetType)ParseHelper.ParseByte(span); break;
                case 2: CustomIndex = ParseHelper.ParseUInt16(span); break;
                case 3: SampleVolume = ParseHelper.ParseByte(span); break;
                case 4: FileName = span.Trim().ToString(); break;
            }
        }
    }

    public override void AppendSerializedString(TextWriter textWriter, int version)
    {
        if (version < 128)
        {
            textWriter.Write((int)X);
            textWriter.Write(',');
            textWriter.Write((int)Y);
        }
        else
        {
            textWriter.Write(X.ToString(ParseHelper.EnUsNumberFormat));
            textWriter.Write(',');
            textWriter.Write(Y.ToString(ParseHelper.EnUsNumberFormat));
        }

        textWriter.Write(',');
        textWriter.Write(Offset);
        textWriter.Write(',');
        textWriter.Write((byte)RawType);
        textWriter.Write(',');
        textWriter.Write((byte)Hitsound);
        textWriter.Write(',');
        if (ObjectType == HitObjectType.Slider && SliderInfo != null)
        {
            SliderInfo.AppendSerializedString(textWriter, version);
            //if (SliderInfo.EdgeHitsounds == null &&
            //    SampleSet == 0 && AdditionSet == 0 && CustomIndex == 0 && SampleVolume == 0 &&
            //    string.IsNullOrEmpty(FileName))
            //{
            //    return;
            //}

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