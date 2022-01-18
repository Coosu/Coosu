using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using Coosu.Beatmap.Utils;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("HitObjects")]
    public class HitObjectSection : Section
    {
        private readonly TimingSection _timingPoints;
        private readonly DifficultySection _difficulty;
        private readonly GeneralSection _general;
        public List<RawHitObject> HitObjectList { get; set; } = new List<RawHitObject>();

        public double MinTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Min(t => t.Offset);
        public double MaxTime => HitObjectList.Count == 0 ? 0 : HitObjectList.Max(t => t.Offset);

        public HitObjectSection(OsuFile osuFile)
        {
            _timingPoints = osuFile.TimingPoints;
            _difficulty = osuFile.Difficulty;
            _general = osuFile.General;
        }

        public RawHitObject this[int index] => HitObjectList[index];

        public override void Match(string line)
        {
            int xNext = line.IndexOf(',');
            int yNext = line.IndexOf(',', xNext + 1);
            int offsetNext = line.IndexOf(',', yNext + 1);
            int typeNext = line.IndexOf(',', offsetNext + 1);
            int hitsoundNext = line.IndexOf(',', typeNext + 1);

#if NETCOREAPP3_1_OR_GREATER
            var spanX = line.AsSpan(0, xNext);
            var spanY = line.AsSpan(xNext + 1, yNext - xNext - 1);
            var spanOffset = line.AsSpan(yNext + 1, offsetNext - yNext - 1);
            var spanType = line.AsSpan(offsetNext + 1, typeNext - offsetNext - 1);
            var spanHitsound = line.AsSpan(typeNext + 1, hitsoundNext - typeNext - 1);
            var x = int.Parse(spanX);
            var y = int.Parse(spanY);
            var offset = int.Parse(spanOffset);
            var type = (RawObjectType)int.Parse(spanType);
            var hitsound = (HitsoundType)int.Parse(spanHitsound);
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
            var pixelLength = double.Parse(infos[2]);

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

            TimingPoint? lastRedLine = _timingPoints.TimingList
                .LastOrDefault(t => !t.Inherit && t.Offset + 0.5 <= hitObject.Offset);

            // hitobjects before red lines is allowed
            lastRedLine ??= _timingPoints.TimingList.First(t => !t.Inherit);

            // ReSharper disable once ReplaceWithSingleCallToLastOrDefault
            TimingPoint? lastLine = _timingPoints.TimingList
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
