using System;
using System.Collections.Generic;
using System.Text;
using Coosu.Shared;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public abstract class BasicTimelineHandler<T> : ActionHandler<T> where T : BasicEvent, new()
    {
        public override string Serialize(T raw)
        {
            if (!raw.IsHalfFilled && !raw.IsFilled)
            {
                throw new Exception("The starting parameter's count should equal to the ending parameter's count");
            }

            raw.Fill();
            var sequenceEqual = raw.IsStartsEqualsEnds();
            var size = raw.EventType.Size;

            var sb = new StringBuilder();
            if (sequenceEqual)
            {
                for (int i = 0; i < size; i++)
                {
                    sb.Append(raw.GetValue(i).ToEnUsFormatString());
                    if (i != size - 1)
                        sb.Append(',');
                }
            }
            else
            {
                for (int i = 0; i < size * 2; i++)
                {
                    sb.Append(raw.GetValue(i).ToEnUsFormatString());
                    if (i != size - 1)
                        sb.Append(',');
                }
            }

            var e = raw.EventType.Flag;
            var easing = ((int)raw.Easing.GetEasingType()).ToString();
            var startT = Math.Round(raw.StartTime).ToEnUsFormatString();
            var endT = raw.StartTime.Equals(raw.EndTime) ? "" : Math.Round(raw.EndTime).ToEnUsFormatString();

            return $"{e},{easing},{startT},{endT},{sb}";
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

            var value = new List<double>(ParameterDimension * 2);
            if (paramLength == ParameterDimension)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension; i++, j++)
                {
                    value[i] = double.Parse(split[j]);
                    value[i + ParameterDimension] = double.Parse(split[j]);
                }
            }
            else if (paramLength == ParameterDimension * 2)
            {
                int j = 4;
                for (int i = 0; i < ParameterDimension * 2; i++, j++)
                {
                    value[i] = double.Parse(split[j]);
                }
            }

            return new T
            {
                Easing = easing.ToEasingFunction(),
                StartTime = startTime,
                EndTime = endTime,
                Values = value
            };
        }
    }
}