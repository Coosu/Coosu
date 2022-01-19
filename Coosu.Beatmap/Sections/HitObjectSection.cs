using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("HitObjects")]
    public sealed class HitObjectSection : Section
    {
        private readonly List<TimingPoint> _timingPoints;
        private readonly DifficultySection _difficulty;
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
            var xNext = line.IndexOf(',');
            var yNext = line.IndexOf(',', xNext + 1);
            var offsetNext = line.IndexOf(',', yNext + 1);
            var typeNext = line.IndexOf(',', offsetNext + 1);
            var hitsoundNext = line.IndexOf(',', typeNext + 1);

#if NETCOREAPP3_1_OR_GREATER
            var x = int.Parse(line.AsSpan(0, xNext));
            var y = int.Parse(line.AsSpan(xNext + 1, yNext - xNext - 1));
            var offset = int.Parse(line.AsSpan(yNext + 1, offsetNext - yNext - 1));
            var type = (RawObjectType)int.Parse(line.AsSpan(offsetNext + 1, typeNext - offsetNext - 1));
            var hitsound = (HitsoundType)int.Parse(line.AsSpan(typeNext + 1, hitsoundNext - typeNext - 1));
#else
            string[] param = line.Split(',');
            var x = int.Parse(param[0]);
            var y = int.Parse(param[1]);
            var offset = int.Parse(param[2]);
            var type = (RawObjectType)int.Parse(param[3]);
            var hitsound = (HitsoundType)int.Parse(param[4]);
#endif

            var others = line.Substring(hitsoundNext + 1);
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

        private void ToCircle(RawHitObject hitObject, string others)
        {
            // extra
            hitObject.Extras = others;
        }

        private void ToSlider(RawHitObject hitObject, string others)
        {
            var infos = others.Split(',');

            // extra
            string notSureExtra = infos[infos.Length - 1];
            bool supportExtra = notSureExtra.IndexOf(":", StringComparison.Ordinal) != -1;
            hitObject.Extras = supportExtra ? notSureExtra : null;

            // slider curve
            var curveInfo = infos[0];
            //var controlPointsInfo = curveInfo.AsSpan(2);
            var controlPoints = curveInfo.Split('|');
            var sliderType = curveInfo[0];

            var points = new Vector2[controlPoints.Length - 1]; // curvePoints skip 1
            for (var i = 1; i < controlPoints.Length; i++)
            {
                var point = controlPoints[i];
#if NETCOREAPP3_1_OR_GREATER
                var indexOfColon = point.IndexOf(':');
                var spanX = point.AsSpan(0, indexOfColon);
                var spanY = point.AsSpan(indexOfColon + 1, point.Length - indexOfColon - 1);
                points[i - 1] = new Vector2(int.Parse(spanX), int.Parse(spanY));
#else
                var xy = point.Split(':');
                points[i - 1] = new Vector2(int.Parse(xy[0]), int.Parse(xy[1]));
#endif
            }

            // repeat
            int repeat = int.Parse(infos[1]);

            // length
            var pixelLength = float.Parse(infos[2]);

            // edge hitsounds
            HitsoundType[]? edgeHitsounds;
            ObjectSamplesetType[]? edgeSamples;
            ObjectSamplesetType[]? edgeAdditions;
            if (infos.Length == 3)
            {
                edgeHitsounds = null;
                edgeSamples = null;
                edgeAdditions = null;
            }
            else if (infos.Length == 4)
            {
                edgeHitsounds = infos[3]
                    .Split('|')
                    .Select(t => (HitsoundType)int.Parse(t))
                    .ToArray();
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeHitsounds = infos[3]
                    .Split('|')
                    .Select(t => (HitsoundType)int.Parse(t))
                    .ToArray();
                string[] edgeAdditionsStrArr = infos[4].Split('|');
                edgeSamples = new ObjectSamplesetType[repeat + 1];
                edgeAdditions = new ObjectSamplesetType[repeat + 1];

                for (int i = 0; i < edgeAdditionsStrArr.Length; i++)
                {
#if NETCOREAPP3_1_OR_GREATER
                    var xy = edgeAdditionsStrArr[i];
                    var indexOfColon = xy.IndexOf(':');
                    var spanX = xy.AsSpan(0, indexOfColon);
                    var spanY = xy.AsSpan(indexOfColon + 1, xy.Length - indexOfColon - 1);
                    edgeSamples[i] = (ObjectSamplesetType)int.Parse(spanX);
                    edgeAdditions[i] = (ObjectSamplesetType)int.Parse(spanY);
#else
                    var sampAdd = edgeAdditionsStrArr[i].Split(':');
                    edgeSamples[i] = (ObjectSamplesetType)int.Parse(sampAdd[0]);
                    edgeAdditions[i] = (ObjectSamplesetType)int.Parse(sampAdd[1]);
#endif
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

        private void ToSpinner(RawHitObject hitObject, string others)
        {
            var infos = others.Split(',');
            var holdEnd = infos[0];
            hitObject.HoldEnd = int.Parse(holdEnd);
            if (infos.Length > 1)
            {
                var extra = infos[1];
                hitObject.Extras = extra;
            }
        }

        private void ToHold(RawHitObject hitObject, string others)
        {
            var index = others.IndexOf(':');

            var holdEnd = others.Substring(0, index);
            var extra = others.Substring(index + 1);
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
