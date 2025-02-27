﻿using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Computing;

public static class BasicEventExtensions
{
    public static bool EqualsInitialPosition(this Move move, ICameraUsable cameraUsable)
    {
        return move.StartX.Equals(cameraUsable.DefaultX) &&
               move.StartY.Equals(cameraUsable.DefaultY);
    }

    public static List<double> ComputeFrame(this BasicEvent e, double currentTime, int? accuracy)
    {
        var easing = e.Easing;
        var size = e.EventType.Size;

        //var start = e.Start;
        //var end = e.End;
        e.Fill();
        var values = e.Values;

        var startTime = (int)e.StartTime;
        var endTime = (int)e.EndTime;

        var normalizedTime = (currentTime - startTime) / (endTime - startTime);
        var easedTime = (double)easing.Ease(normalizedTime);

        var list = new List<double>(size);
        for (int i = 0; i < size; i++)
        {
            var val = (values[i + size] - values[i]) * easedTime + values[i];
            if (accuracy == null) list.Add(val);
            else list.Add((double)Math.Round(val, accuracy.Value));
        }

        return list;
    }

    public static List<double> ComputeFrame(IEnumerable<BasicEvent> events, EventType eventType, double time, int? accuracy)
    {
        var basicEvents = events
            .OrderBy(k => k.StartTime)
            .ToList();
        if (basicEvents.Count == 0)
            return eventType.GetDefaultValue()?.ToList() ?? throw new NotSupportedException(eventType.Flag + " doesn't have any default value.");

        if (time < basicEvents[0].StartTime)
        {
            basicEvents[0].Fill();
            return basicEvents[0].Values.Take(eventType.Size).ToList();
        }

        var e = basicEvents.FirstOrDefault(k => k.StartTime <= time && k.EndTime > time);
        if (e != null) return KeyEventExtensions.ComputeFrame(e, time, accuracy);

        var lastE = basicEvents.Last(k => k.EndTime <= time);
        lastE.Fill();
        return lastE.Values.Skip(eventType.Size).ToList();
    }

    public static List<IKeyEvent> ComputeDiscretizedEvents(this BasicEvent e,
        int discretizingInterval,
        int? discretizingAccuracy)
    {
        var eventList = new List<IKeyEvent>();
        var targetEventType = e.EventType;

        var startTime = (int)e.StartTime;
        var endTime = (int)e.EndTime;

        var thisTime = startTime - (startTime % discretizingInterval);
        var nextTime = startTime - (startTime % discretizingInterval) + discretizingInterval;
        if (nextTime > endTime) nextTime = endTime;
        List<double> reusableValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);

        eventList.Add(BasicEvent.Create(targetEventType, LinearEase.Instance,
            startTime, nextTime, e.GetStarts(), reusableValue));

        while (nextTime < endTime)
        {
            thisTime += discretizingInterval;
            nextTime += discretizingInterval;
            if (nextTime > endTime) nextTime = endTime;
            List<double> newValue = e.ComputeFrame(nextTime, nextTime == endTime ? null : discretizingAccuracy);
            eventList.Add(BasicEvent.Create(targetEventType, LinearEase.Instance, thisTime, nextTime,
                reusableValue.ToList(), newValue));
            reusableValue = newValue;
        }

        return eventList;
    }
}