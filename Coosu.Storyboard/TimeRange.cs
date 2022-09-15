using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard;

public class TimeRange
{
    private readonly List<TimingPoint> _timingPoints = new();
    private List<RangeValue<double>>? _timingList;
    public List<RangeValue<double>> TimingList => _timingList ??= GetTimingList();

    public double MinStartTime => TimingList.First().StartTime;
    public double MinEndTime => TimingList.First().EndTime;
    public double MaxStartTime => TimingList.Last().StartTime;
    public double MaxEndTime => TimingList.Last().EndTime;

    public int Count => TimingList.Count;

    public void Add(double startTime, double endTime)
    {
        if (startTime.Equals(endTime)) return;
        _timingPoints.AddSorted(new TimingPoint(startTime, true), TimingPointComparer.Instance);
        _timingPoints.AddSorted(new TimingPoint(endTime, false), TimingPointComparer.Instance);
        _timingList = null;
    }

    private List<RangeValue<double>> GetTimingList()
    {
        var list = new List<RangeValue<double>>();
        double? tmpStart = null, tmpEnd = null;
        for (var i = 0; i < _timingPoints.Count; i++)
        {
            var timingPoint = _timingPoints[i];
            if (tmpStart == null && tmpEnd == null)
            {
                if (timingPoint.IsStart)
                {
                    tmpStart = timingPoint.Timing;
                }
            }
            else if (tmpEnd == null)
            {
                if (!timingPoint.IsStart && i != _timingPoints.Count - 1 &&
                    _timingPoints[i + 1].IsStart &&
                    !Precision.AlmostEquals(timingPoint.Timing, _timingPoints[i + 1].Timing)
                    ||
                    !timingPoint.IsStart &&
                    i == _timingPoints.Count - 1)
                {
                    tmpEnd = timingPoint.Timing;
                    list.Add(new RangeValue<double>(tmpStart!.Value, tmpEnd.Value));
                    tmpStart = null;
                    tmpEnd = null;
                }
            }
        }

        return list;
    }

    public bool ContainsTimingPoint(double time, out RangeValue<double> patterned,
        double offsetStart = 0,
        double offsetEnd = 0)
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

    public bool OnTimingRangeBound(out RangeValue<double> patterned, double timingPoint)
    {
        for (var i = 0; i < TimingList.Count; i++)
        {
            var range = TimingList[i];
            if (Precision.AlmostEquals(timingPoint, range.StartTime) ||
                Precision.AlmostEquals(timingPoint, range.EndTime))
            {
                patterned = range;
                return true;
            }
        }

        patterned = default;
        return false;
    }

    public bool ContainsTimingPoint(out RangeValue<double> patterned, params double[] timeList)
    {
        Array.Sort(timeList);
        if (timeList.Length == 0)
            throw new ArgumentOutOfRangeException("length", timeList.Length, null);

        var first = timeList[0];
        var last = timeList[timeList.Length - 1];
        for (var i = 0; i < TimingList.Count; i++)
        {
            var range = TimingList[i];
            if (first >= range.StartTime && last <= range.EndTime)
            {
                patterned = range;
                return true;
            }
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