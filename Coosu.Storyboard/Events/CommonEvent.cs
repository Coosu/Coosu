using System;
using System.Globalization;

namespace Coosu.Storyboard.Events
{
    public abstract class CommonEvent : ICommonEvent, IAdjustableTimingEvent, IComparable<CommonEvent>
    {
        public abstract EventType EventType { get; }
        public EasingType Easing { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float[] Start { get; }
        public float[] End { get; }

        protected virtual string Script
        {
            get
            {
                bool sequenceEqual = true;
                int count = Start.Length;
                for (int i = 0; i < count; i++)
                {
                    if (Start[i] != End[i])
                    {
                        sequenceEqual = false;
                        break;
                    }
                }

                if (sequenceEqual)
                {
                    if (count == 1)
                        return Start[0].ToString(CultureInfo.InvariantCulture);
                    if (count == 2)
                        return Start[0].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[1].ToString(CultureInfo.InvariantCulture);
                    if (count == 3)
                        return Start[0].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[1].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[2].ToString(CultureInfo.InvariantCulture);
                    return string.Join(",", Start);
                }
                else
                {
                    if (count == 1)
                        return Start[0].ToString(CultureInfo.InvariantCulture) + "," +
                               End[0].ToString(CultureInfo.InvariantCulture);
                    if (count == 2)
                        return Start[0].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[1].ToString(CultureInfo.InvariantCulture) + "," +
                               End[0].ToString(CultureInfo.InvariantCulture) + "," +
                               End[1].ToString(CultureInfo.InvariantCulture);
                    if (count == 3)
                        return Start[0].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[1].ToString(CultureInfo.InvariantCulture) + "," +
                               Start[2].ToString(CultureInfo.InvariantCulture) + "," +
                               End[0].ToString(CultureInfo.InvariantCulture) + "," +
                               End[1].ToString(CultureInfo.InvariantCulture) + "," +
                               End[2].ToString(CultureInfo.InvariantCulture);
                    return $"{string.Join(",", Start)},{string.Join(",", End)}";
                }
            }
        }

        public virtual int ParamLength => Start.Length;
        public virtual bool IsStatic => Start.Equals(End);

        protected CommonEvent(EasingType easing, float startTime, float endTime, float[] start, float[] end)
        {
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            Start = start;
            End = end;
        }

        public int CompareTo(CommonEvent other)
        {
            if (other == null)
                return 1;

            if (StartTime > other.StartTime)
                return 1;

            if (StartTime.Equals(other.StartTime))
                return 0;

            if (StartTime < other.StartTime)
                return -1;

            throw new ArgumentOutOfRangeException(nameof(other));
        }

        public override string ToString()
        {
            return ToOsbString();
        }

        public virtual string ToOsbString()
        {
            string e = EventType.ToShortString();
            string easing = ((int)Easing).ToString();
            string startT = Math.Round(StartTime).ToString(CultureInfo.InvariantCulture);
            string endT = StartTime.Equals(EndTime)
                ? ""
                : Math.Round(EndTime).ToString(CultureInfo.InvariantCulture);
            return $"{e},{easing},{startT},{endT},{Script}";
        }

        public void AdjustTiming(float time)
        {
            StartTime += time;
            EndTime += time;
        }
    }
}
