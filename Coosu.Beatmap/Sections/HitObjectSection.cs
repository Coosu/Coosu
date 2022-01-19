using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("HitObjects")]
    public sealed class HitObjectSection : Section
    {
        private readonly List<TimingPoint> _timingPoints;
        private readonly DifficultySection _difficulty;
        private readonly SpanSplitArgs _e = new();
        public List<RawHitObject> HitObjectList { get; set; } = new();

        public double MinTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Min(t => t.Offset);
        public double MaxTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Max(t => t.Offset);

        public HitObjectSection(OsuFile osuFile)
        {
            _timingPoints = osuFile.TimingPoints.TimingList;
            _timingPoints.Sort(new TimingPointComparer());
            _difficulty = osuFile.Difficulty;
        }

        public RawHitObject this[int index] => HitObjectList[index];

        public override void Match(string line)
        {
            int x = default;
            int y = default;
            int offset = default;
            RawObjectType type = default;
            HitsoundType hitsound = default;
            ReadOnlySpan<char> others = default;

            int i = -1;
            _e.Canceled = false;
            foreach (var span in line.SpanSplit(',', _e))
            {
                i++;
#if NETCOREAPP3_1_OR_GREATER
                switch (i)
                {
                    case 0: x = int.Parse(span); break;
                    case 1: y = int.Parse(span); break;
                    case 2: offset = int.Parse(span); break;
                    case 3: type = (RawObjectType)int.Parse(span); break;
                    case 4: hitsound = (HitsoundType)int.Parse(span); _e.Canceled = true; break;
                    default: others = span; break;
                }
#else
                switch (i)
                {
                    case 0: x = int.Parse(span.ToString()); break;
                    case 1: y = int.Parse(span.ToString()); break;
                    case 2: offset = int.Parse(span.ToString()); break;
                    case 3: type = (RawObjectType)int.Parse(span.ToString()); break;
                    case 4: hitsound = (HitsoundType)int.Parse(span.ToString()); _e.Canceled = true; break;
                    default: others = span; break;
                }
#endif
            }

            var hitObject = new RawHitObject
            {
                X = x,
                Y = y,
                Offset = offset,
                RawType = type,
                Hitsound = hitsound
            };

            switch (hitObject.ObjectType)
            {
                case HitObjectType.Circle:
                    ToCircle(hitObject, others);
                    break;
                case HitObjectType.Slider:
                    ToSlider(hitObject, others);
                    break;
                case HitObjectType.Spinner:
                    ToSpinner(hitObject, others);
                    break;
                case HitObjectType.Hold:
                    ToHold(hitObject, others);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HitObjectList.Add(hitObject);
        }

        private void ToCircle(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            // extra
            hitObject.Extras = others.ToString();
        }

        private void ToSlider(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            var infos = others.ToString().Split(',');

            // extra
            string notSureExtra = infos[infos.Length - 1];
            bool supportExtra = notSureExtra.IndexOf(":", StringComparison.Ordinal) != -1;
            hitObject.Extras = supportExtra ? notSureExtra : null;

            // slider curve
            var curveInfo = infos[0];
            var sliderType = curveInfo[0];

            var points = new List<Vector2>(); // curvePoints skip 1
            int j = -1;
            foreach (var point in curveInfo.SpanSplit('|'))
            {
                j++;
                if (j < 1) continue;

                int i = 0;
                int x = 0;
                foreach (var s in point.SpanSplit(':'))
                {
                    if (i == 0)
                    {
#if NETCOREAPP3_1_OR_GREATER
                        x = int.Parse(s);
#else
                        x = int.Parse(s.ToString());
#endif
                    }
                    else
                    {
#if NETCOREAPP3_1_OR_GREATER
                        var y = int.Parse(s);
#else
                        var y = int.Parse(s.ToString());
#endif
                        points.Add(new Vector2(x, y));
                    }

                    i++;
                }
            }

            // repeat
            int repeat = int.Parse(infos[1]);

            // length
            var pixelLength = double.Parse(infos[2]);

            // edge hitsounds
            List<HitsoundType>? edgeHitsounds;
            ObjectSamplesetType[]? edgeSamples;
            ObjectSamplesetType[]? edgeAdditions;
            if (infos.Length <= 3)
            {
                edgeHitsounds = null;
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeHitsounds = new List<HitsoundType>();
                foreach (var span in infos[3].SpanSplit('|'))
                {
#if NETCOREAPP3_1_OR_GREATER
                    edgeHitsounds.Add((HitsoundType)int.Parse(span));
#else
                    edgeHitsounds.Add((HitsoundType)int.Parse(span.ToString()));
#endif
                }

                if (infos.Length > 4)
                {
                    edgeSamples = new ObjectSamplesetType[edgeHitsounds.Count];
                    edgeAdditions = new ObjectSamplesetType[edgeHitsounds.Count];
                    int i = -1;
                    foreach (var span in infos[4].SpanSplit('|'))
                    {
                        i++;
                        int k = -1;
                        foreach (var span2 in span.SpanSplit(':'))
                        {
                            k++;
                            switch (k)
                            {
                                case 0:
#if NETCOREAPP3_1_OR_GREATER
                                    var x = (ObjectSamplesetType)int.Parse(span2);
#else
                                    var x = (ObjectSamplesetType)int.Parse(span2.ToString());
#endif
                                    edgeSamples[i] = x;
                                    break;
                                case 1:
#if NETCOREAPP3_1_OR_GREATER
                                    var y = (ObjectSamplesetType)int.Parse(span2);
#else
                                    var y = (ObjectSamplesetType)int.Parse(span2.ToString());
#endif
                                    edgeAdditions[i] = y;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    edgeSamples = null;
                    edgeAdditions = null;
                }
            }

            TimingPoint? lastRedLine = _timingPoints
                .LastOrDefault(t => !t.IsInherit && t.Offset + 0.5 <= hitObject.Offset);

            // hitobjects before red lines is allowed
            lastRedLine ??= _timingPoints.First(t => !t.IsInherit);

            // ReSharper disable once ReplaceWithSingleCallToLastOrDefault
            TimingPoint? lastLine = _timingPoints
                .Where(t => t.Offset - 0.5 >= lastRedLine.Offset && t.Offset + 0.5 <= hitObject.Offset)
                //.OrderBy(k => k.Offset)
                //.ThenBy(k => k.Inherit) // 1 red + 1 green is allowed, and green has a higher priority.
                .LastOrDefault();
            // hitobjects before red lines is allowed
            lastLine ??= lastRedLine;

            hitObject.SliderInfo = new SliderInfo(new Vector2(hitObject.X, hitObject.Y),
                hitObject.Offset,
                lastRedLine?.Factor ?? 0,
                _difficulty.SliderMultiplier * (lastLine?.Multiple ?? 0),
                _difficulty.SliderTickRate, pixelLength)
            {
                ControlPoints = points,
                EdgeAdditions = edgeAdditions,
                EdgeHitsounds = edgeHitsounds,
                EdgeSamples = edgeSamples,
                Repeat = repeat,
                SliderType = sliderType.SliderFlagToEnum()
            };
        }

        private void ToSpinner(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            var s = others.ToString();
            var infos = s.Split(',');
            var holdEnd = infos[0];
            hitObject.HoldEnd = int.Parse(holdEnd);
            if (infos.Length > 1)
            {
                var extra = infos[1];
                hitObject.Extras = extra;
            }
        }

        private void ToHold(RawHitObject hitObject, ReadOnlySpan<char> others)
        {
            var s = others.ToString();
            var index = s.IndexOf(':');

            var holdEnd = s.Substring(0, index);
            var extra = s.Substring(index + 1);
            hitObject.HoldEnd = int.Parse(holdEnd);
            hitObject.Extras = extra;
        }

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.WriteLine($"[{SectionName}]");
            foreach (var hitObject in HitObjectList)
            {
                hitObject.AppendSerializedString(textWriter);
            }
        }
    }
}
