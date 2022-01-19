using System;
using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Sections.HitObject
{
    public sealed class RawHitObject : SerializeWritableObject
    {
        private string? _extras = "0:0:0:0:";
        private bool _extraInitial;
        private bool _extraAnyUpdated;
        private ObjectSamplesetType _sampleSet;
        private ObjectSamplesetType _additionSet;
        private int _customIndex;
        private int _sampleVolume;
        private string _fileName;

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
                var newThing = 0b01110000 & (int)RawType;
                var ncCount = newThing >> 4;
                return ncCount + ncBase;
            }
        }
        public HitsoundType Hitsound { get; set; }
        public SliderInfo? SliderInfo { get; set; }
        public int HoldEnd { get; set; }

        public string? Extras
        {
            get => _extras;
            set
            {
                _extras = value;
                _extraInitial = false;
            }
        }

        #region Extras

        public ObjectSamplesetType SampleSet
        {
            get
            {
                if (!_extraInitial) InitialExtra();
                return _sampleSet;
            }
            set
            {
                _sampleSet = value;
                _extraAnyUpdated = true;
            }
        }

        public ObjectSamplesetType AdditionSet
        {
            get
            {
                if (!_extraInitial) InitialExtra();
                return _additionSet;
            }
            set
            {
                _additionSet = value;
                _extraAnyUpdated = true;
            }
        }

        public int CustomIndex
        {
            get
            {
                if (!_extraInitial) InitialExtra();
                return _customIndex;
            }
            set
            {
                _customIndex = value;
                _extraAnyUpdated = true;
            }
        }

        public int SampleVolume
        {
            get
            {
                if (!_extraInitial) InitialExtra();
                return _sampleVolume;
            }
            set
            {
                _sampleVolume = value;
                _extraAnyUpdated = true;
            }
        }

        public string FileName
        {
            get
            {
                if (!_extraInitial) InitialExtra();
                return _fileName;
            }
            set
            {
                _fileName = value;
                _extraAnyUpdated = true;
            }
        }

        private void InitialExtra()
        {
            if (!string.IsNullOrWhiteSpace(Extras))
            {
                var arr = Extras.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0) _sampleSet = arr[0].ParseToEnum<ObjectSamplesetType>();
                if (arr.Length > 1) _additionSet = arr[1].ParseToEnum<ObjectSamplesetType>();
                if (arr.Length > 2) _customIndex = int.Parse(arr[2]);
                if (arr.Length > 3) _sampleVolume = int.Parse(arr[3]);
                if (arr.Length > 4) _fileName = arr[4];
            }

            _extraInitial = true;
        }

        #endregion

        public override string ToString()
        {
            if (_extraAnyUpdated) Extras = $"{(int)_sampleSet}:{(int)_additionSet}:{_customIndex}:{_sampleVolume}:{_fileName}";
            switch (ObjectType)
            {
                case HitObjectType.Circle:
                    return $"{X},{Y},{Offset},{(int)RawType},{(int)Hitsound}{(Extras == null ? "" : "," + Extras)}";
                case HitObjectType.Slider:
                    return string.Format("{0},{1},{2},{3},{4},{5}{6}",
                        X, Y, Offset, (int)RawType, (int)Hitsound, SliderInfo,
                        Extras == null ? "" : "," + Extras);
                case HitObjectType.Spinner:
                    return $"{X},{Y},{Offset},{(int)RawType},{(int)Hitsound},{HoldEnd},{Extras ?? ""}";
                case HitObjectType.Hold:
                    return $"{X},{Y},{Offset},{(int)RawType},{(int)Hitsound},{HoldEnd}:{Extras ?? ""}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.WriteLine(ToString());
        }
    }
}
