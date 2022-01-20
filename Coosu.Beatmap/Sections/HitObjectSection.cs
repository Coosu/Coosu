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
            ReadOnlySpan<char> curveInfo = default;
            int repeat = default;
            double pixelLength = default;
            ReadOnlySpan<char> edgeHitsoundInfo = default;
            ReadOnlySpan<char> sampleAdditionInfo = default;
            ReadOnlySpan<char> extraInfo = default;

            int index = -1;
            foreach (var infoSpan in others.SpanSplit(','))
            {
                index++;
                switch (index)
                {
                    case 0:
                        curveInfo = infoSpan;
                        break;
                    case 1:
                        // repeat
#if NETCOREAPP3_1_OR_GREATER
                        repeat = int.Parse(infoSpan);
#else
                        repeat = int.Parse(infoSpan.ToString());
#endif
                        break;
                    case 2:
                        // length
#if NETCOREAPP3_1_OR_GREATER
                        pixelLength = double.Parse(infoSpan);
#else
                        pixelLength = double.Parse(infoSpan.ToString());
#endif
                        break;
                    case 3:
                        edgeHitsoundInfo = infoSpan;
                        break;
                    case 4:
                        sampleAdditionInfo = infoSpan;
                        break;
                    case 5:
                        extraInfo = infoSpan;
                        break;
                }
            }

            // slider curve
            char sliderType = default;
            var points = curveInfo.Length > 100
                ? curveInfo.Length > 200
                    ? new List<Vector2>(50)
                    : new List<Vector2>(30)
                : new List<Vector2>();

            int j = -1;
            foreach (var point in curveInfo.SpanSplit('|'))
            {
                j++;
                if (j < 1)
                {
                    sliderType = point[0];
                    continue; // curvePoints skip 1
                }

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

            // edge hitsounds
            HitsoundType[]? edgeHitsounds;
            ObjectSamplesetType[]? edgeSamples;
            ObjectSamplesetType[]? edgeAdditions;
            if (index < 3)
            {
                edgeHitsounds = null;
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeHitsounds = new HitsoundType[repeat + 1];
                int g = -1;
                foreach (var span in edgeHitsoundInfo.SpanSplit('|'))
                {
                    g++;
#if NETCOREAPP3_1_OR_GREATER
                    edgeHitsounds[g] = (HitsoundType)int.Parse(span);
#else
                    edgeHitsounds[g] = (HitsoundType)int.Parse(span.ToString());
#endif
                }

                if (index >= 4)
                {
                    edgeSamples = new ObjectSamplesetType[edgeHitsounds.Length];
                    edgeAdditions = new ObjectSamplesetType[edgeHitsounds.Length];
                    int i = -1;
                    foreach (var span in sampleAdditionInfo.SpanSplit('|'))
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

            // extra
            if (index == 5)
            {
                hitObject.Extras = extraInfo.ToString();
            }

            hitObject.SliderInfo = new SliderInfo
            {
                StartPoint = new Vector2(hitObject.X, hitObject.Y),
                StartTime = hitObject.Offset,
                PixelLength = pixelLength,
                ControlPoints = points,
                EdgeAdditions = edgeAdditions,
                EdgeHitsounds = edgeHitsounds,
                EdgeSamples = edgeSamples,
                Repeat = repeat,
                SliderType = sliderType.SliderFlagToEnum()
            };

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

            hitObject.SliderInfo.SetVariables(
                lastRedLine?.Factor ?? 0,
                _difficulty.SliderMultiplier * (lastLine?.Multiple ?? 0),
                _difficulty.SliderTickRate);
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
