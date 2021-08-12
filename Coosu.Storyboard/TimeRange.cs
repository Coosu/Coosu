using System.Collections.Generic;
using System.Linq;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public class TimeRange
    {
        private readonly SortedSet<TimingPoint> _timingPoints = new(new TimingPointComparer());
        private List<RangeValue<float>>? _timingList;
        public List<RangeValue<float>> TimingList => _timingList ??= GetTimingList();

        public float MinStartTime => TimingList.First().StartTime;
        public float MinEndTime => TimingList.First().EndTime;
        public float MaxStartTime => TimingList.Last().StartTime;
        public float MaxEndTime => TimingList.Last().EndTime;

        public int Count => TimingList.Count;

        public void Add(float startTime, float endTime)
        {
            if (startTime.Equals(endTime)) return;
            _timingPoints.Add(new TimingPoint(startTime, true));
            _timingPoints.Add(new TimingPoint(endTime, false));
            _timingList = null;
        }

        private List<RangeValue<float>> GetTimingList()
        {
            var list = new List<RangeValue<float>>();
            float? tmpStart = null, tmpEnd = null;
            var array = _timingPoints.ToArray();
            for (var i = 0; i < array.Length; i++)
            {
                var timingPoint = array[i];
                if (tmpStart == null && tmpEnd == null)
                {
                    if (timingPoint.IsStart)
                    {
                        tmpStart = timingPoint.Timing;
                    }
                }
                else if (tmpEnd == null)
                {
                    if (!timingPoint.IsStart && i != array.Length - 1 &&
                        array[i + 1].IsStart &&
                        !timingPoint.Timing.Equals(array[i + 1].Timing) ||

                        !timingPoint.IsStart &&
                        i == array.Length - 1)
                    {
                        tmpEnd = timingPoint.Timing;
                        list.Add(new RangeValue<float>(tmpStart!.Value, tmpEnd.Value));
                        tmpStart = null;
                        tmpEnd = null;
                    }
                }
            }

            return list;
        }

        public bool ContainsTimingPoint(float time, out RangeValue<float> patterned,
            float offsetStart = 0,
            float offsetEnd = 0)
        {
            foreach (var range in TimingList)
                if (time >= range.StartTime + offsetStart && time <= range.EndTime + offsetEnd)
                {
                    patterned = range;
                    return true;
                }

            patterned = default;
            return false;
        }

        public bool OnTimingRange(out RangeValue<float> patterned, float timingPoint)
        {
            //int i = 0;
            foreach (var range in TimingList)
            {
                if (timingPoint.Equals(range.StartTime) || timingPoint.Equals(range.EndTime))
                {
                    patterned = range;
                    return true;
                }

                //i++;
            }

            patterned = default;
            return false;
        }

        public bool ContainsTimingPoint(out RangeValue<float> patterned, params float[] time)
        {
            //int i = 0;
            foreach (var range in TimingList)
            {
                if (time.All(t => t >= range.StartTime && t <= range.EndTime))
                {
                    patterned = range;
                    return true;
                }

                //i++;
            }

            patterned = default;
            return false;
        }

        public override string ToString()
        {
            return TimingList.Any()
                ? string.Join(",", TimingList.Select(k => $"[{k.StartTime},{k.EndTime}]"))
                : "{empty}";
        }
    }
}
