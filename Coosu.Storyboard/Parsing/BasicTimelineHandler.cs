using System;
using System.Globalization;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Parsing
{
    public abstract class BasicTimelineHandler<T> : ActionHandler<T> where T : CommonEvent, new()
    {
        public override string Serialize(T raw)
        {
            bool sequenceEqual = true;
            int count = raw.Start.Length;
            if (raw.End.Length != count)
            {
                throw new Exception("wtf");
            }

            for (int i = 0; i < count; i++)
            {
                if (raw.Start[i] != raw.End[i])
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
                if (count == 2)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo);
                if (count == 3)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.Start[2].ToString(cultureInfo);
                script = string.Join(",", raw.Start);
            }
            else
            {
                if (count == 1)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo);
                if (count == 2)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo) + "," +
                             raw.End[1].ToString(cultureInfo);
                if (count == 3)
                    script = raw.Start[0].ToString(cultureInfo) + "," +
                             raw.Start[1].ToString(cultureInfo) + "," +
                             raw.Start[2].ToString(cultureInfo) + "," +
                             raw.End[0].ToString(cultureInfo) + "," +
                             raw.End[1].ToString(cultureInfo) + "," +
                             raw.End[2].ToString(cultureInfo);
                script = $"{string.Join(",", raw.Start)},{string.Join(",", raw.End)}";
            }

            var e = raw.EventType.ToShortString();
            var easing = ((int)raw.Easing).ToString();
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
            var startTime = float.Parse(split[2]);
            var endTime = string.IsNullOrWhiteSpace(split[3]) ? startTime : float.Parse(split[3]);

            var start = new float[ParameterDimension];
            var end = new float[ParameterDimension];
            if (paramLength == ParameterDimension)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    start[i] = float.Parse(split[j]);
                }

                start.CopyTo(end, 0);
            }
            else if (paramLength == ParameterDimension * 2)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    start[i] = float.Parse(split[j]);
                }

                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    end[i] = float.Parse(split[j]);
                }
            }

            return new T { Easing = easing, StartTime = startTime, EndTime = endTime, Start = start, End = end };
        }
    }

    public class EasingConvert
    {
        public static EasingType ToEasing(string s)
        {
            var easing = int.Parse(s);
            if (easing > 34 || easing < 0)
                throw new FormatException("Unknown easing");
            return (EasingType)easing;
        }
    }
}