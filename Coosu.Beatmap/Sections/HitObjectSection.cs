using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
            string[] param = line.SpanSplit(",");

            var x = int.Parse(param[0]);
            var y = int.Parse(param[1]);
            var offset = int.Parse(param[2]);
            var type = (RawObjectType)Enum.Parse(typeof(RawObjectType), param[3]);
            var hitsound = (HitsoundType)Enum.Parse(typeof(HitsoundType), param[4]);
            var others = string.Join(",", param.Skip(5));

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
            var infos = others.SpanSplit(",");

            // extra
            string notSureExtra = infos[infos.Length - 1];
            bool supportExtra = notSureExtra.IndexOf(":", StringComparison.Ordinal) != -1;
            hitObject.Extras = supportExtra ? notSureExtra : null;

            // slider curve
            var curveInfo = infos[0].SpanSplit("|");
            var sliderType = infos[0].Split('|')[0];

            var points = new Vector2<float>[curveInfo.Length - 1]; // curvePoints skip 1
            for (var i = 1; i < curveInfo.Length; i++)
            {
                var point = curveInfo[i];
                var xy = point.SpanSplit(":");
                points[i - 1] = new Vector2<float>(int.Parse(xy[0]), int.Parse(xy[1]));
            }

            // repeat
            int repeat = int.Parse(infos[1]);

            // length
            decimal pixelLength = decimal.Parse(infos[2]);

            // edge hitsounds
            HitsoundType[] edgeHitsounds;
            ObjectSamplesetType[] edgeSamples;
            ObjectSamplesetType[] edgeAdditions;
            if (infos.Length == 3)
            {
                edgeHitsounds = null;
                edgeSamples = null;
                edgeAdditions = null;
            }
            else if (infos.Length == 4)
            {
                edgeHitsounds = infos[3].SpanSplit("|").Select(t => t.ParseToEnum<HitsoundType>()).ToArray();
                edgeSamples = null;
                edgeAdditions = null;
            }
            else
            {
                edgeHitsounds = infos[3].SpanSplit("|").Select(t => t.ParseToEnum<HitsoundType>()).ToArray();
                string[] edgeAdditionsStrArr = infos[4].SpanSplit("|");
                edgeSamples = new ObjectSamplesetType[repeat + 1];
                edgeAdditions = new ObjectSamplesetType[repeat + 1];

                for (int i = 0; i < edgeAdditionsStrArr.Length; i++)
                {
                    var sampAdd = edgeAdditionsStrArr[i].SpanSplit(":");
                    edgeSamples[i] = sampAdd[0].ParseToEnum<ObjectSamplesetType>();
                    edgeAdditions[i] = sampAdd[1].ParseToEnum<ObjectSamplesetType>();
                }
            }

            TimingPoint[] lastRedLinesIfExsist = _timingPoints.TimingList.Where(t => !t.Inherit)
                .Where(t => t.Offset <= hitObject.Offset).ToArray();
            TimingPoint lastRedLine;

            // hitobjects before lines is allowed
            if (lastRedLinesIfExsist.Length == 0)
                lastRedLine = _timingPoints.TimingList.First(t => !t.Inherit);
            else
            {
                double lastRedLineOffset = lastRedLinesIfExsist.Max(t => t.Offset);

                //duplicate red lines, select the last one
                lastRedLine = _timingPoints.TimingList.Last(t => t.Offset == lastRedLineOffset && !t.Inherit);
            }

            TimingPoint[] lastLinesIfExist = _timingPoints.TimingList.Where(t => t.Offset <= hitObject.Offset).ToArray();
            TimingPoint[] lastLines; // 1 red + 1 green is allowed
            TimingPoint lastLine;

            // hitobjects before lines is allowed
            if (lastLinesIfExist.Length == 0)
                lastLines = new[] { _timingPoints.TimingList.First(t => !t.Inherit) }; //red line multiple default 1.0
            else
            {
                double lastLineOffset = lastLinesIfExist.Max(t => t.Offset);
                // 1 red + 1 green is allowed, so maybe here are two results
                lastLines = _timingPoints.TimingList.Where(t => t.Offset == lastLineOffset).ToArray();
            }

            if (lastLines.Length > 1)
            {
                lastLine = lastLines.LastOrDefault(k => k.Inherit) ?? lastLines.Last(k => !k.Inherit);

                //if (lastLines.Length == 2)
                //{
                //    if (lastLines[0].Inherit != lastLines[1].Inherit)
                //    {
                //        lastLine = lastLines.First(t => t.Inherit);
                //    }
                //    else
                //        throw new RepeatTimingSectionException("存在同一时刻两条相同类型的Timing Section。");
                //}
                //else
                //    throw new RepeatTimingSectionException("存在同一时刻多条Timing Section。");
            }
            else lastLine = lastLines[0];

            hitObject.SliderInfo = new SliderInfo(new Vector2<float>(hitObject.X, hitObject.Y),
                hitObject.Offset,
                lastRedLine.Factor,
                _difficulty.SliderMultiplier * lastLine.Multiple,
                _difficulty.SliderTickRate, pixelLength)
            {
                CurvePoints = points,
                EdgeAdditions = edgeAdditions,
                EdgeHitsounds = edgeHitsounds,
                EdgeSamples = edgeSamples,
                Repeat = repeat,
                SliderType = sliderType.ParseToEnum<SliderType>()
            };
        }

        private void ToSpinner(RawHitObject hitObject, string others)
        {
            var infos = others.SpanSplit(",");
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
            var index = others.SpanIndexOf(":");

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
