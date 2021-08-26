using System;
using System.Globalization;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public abstract class BasicTimelineHandler<T> : ActionHandler<T> where T : BasicEvent, new()
    {
        public override string Serialize(T raw)
        {
            bool sequenceEqual = true;
            int count = raw.Start.Length;
            if (raw.End.Length != count)
            {
                throw new Exception("The starting parameter's count should equal to the ending parameter's count");
            }

            for (int i = 0; i < count; i++)
            {
                if (!raw.Start[i].Equals(raw.End[i]))
                {
                    sequenceEqual = false;
                    break;
                }
            }

            var cultureInfo = CultureInfo.InvariantCulture;
            string script;
            if (sequenceEqual)
            {
                if (count == 1)
                    script = raw.Start[0].ToString(cultureInfo);
                else if (count == 2)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo);
                else if (count == 3)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.Start[2].ToString(cultureInfo);
                else
                    script = string.Join(",", raw.Start);
            }
            else
            {
                if (count == 1)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo);
                else if (count == 2)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo) + "," +
                             raw.End[1].ToString(cultureInfo);
                else if (count == 3)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.Start[2].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo) + "," +
                             raw.End[1].ToString(cultureInfo) + "," +
                             raw.End[2].ToString(cultureInfo);
                else
                    script = $"{string.Join(",", raw.Start)},{string.Join(",", raw.End)}";
            }

            var e = raw.EventType.Flag;
            var easing = ((int)raw.Easing.GetEasingType()).ToString();
            var startT = Math.Round(raw.StartTime).ToString(cultureInfo);
            var endT = raw.StartTime.Equals(raw.EndTime) ? "" : Math.Round(raw.EndTime).ToString(cultureInfo);

            return $"{e},{easing},{startT},{endT},{script}";
        }

        public override T Deserialize(string[] split)
        {
            var paramLength = split.Length - 4;

            if (paramLength != ParameterDimension && paramLength != ParameterDimension * 2)
            {
                throw new ArgumentOutOfRangeException();
            }

            var easing = EasingConvert.ToEasing(split[1]);
            var startTime = double.Parse(split[2]);
            var endTime = string.IsNullOrWhiteSpace(split[3]) ? startTime : double.Parse(split[3]);

            var start = new double[ParameterDimension];
            var end = new double[ParameterDimension];
            if (paramLength == ParameterDimension)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    start[i] = double.Parse(split[j]);
                }

                start.CopyTo(end, 0);
            }
            else if (paramLength == ParameterDimension * 2)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    start[i] = double.Parse(split[j]);
                }

                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    end[i] = double.Parse(split[j]);
                }
            }

            return new T
            {
                Easing = easing.ToEasingFunction(), StartTime = startTime, EndTime = endTime, Start = start, End = end
            };
        }
    }
}