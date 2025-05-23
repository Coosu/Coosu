﻿using System;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility;

public abstract class BasicTimelineHandler<T> : ActionHandler<T> where T : IKeyEvent, new()
{
    public abstract int ParameterDimension { get; }
    public override string Serialize(T e)
    {
        if (e is { IsHalfFilled: false, IsFilled: false })
        {
            throw new Exception("The starting parameter's count should equal to the ending parameter's count");
        }

        e.Fill();
        var sequenceEqual = e.IsStartsEqualsEnds();
        var size = e.EventType.Size;

        var vsb = new ValueStringBuilder(stackalloc char[64]);
        if (sequenceEqual)
        {
            for (int i = 0; i < size; i++)
            {
                vsb.Append(e.GetValue(i).ToEnUsFormatString());
                if (i != size - 1)
                {
                    vsb.Append(',');
                }
            }
        }
        else
        {
            var dSize = size * 2;
            for (int i = 0; i < dSize; i++)
            {
                vsb.Append(e.GetValue(i).ToEnUsFormatString());
                if (i != dSize - 1)
                {
                    vsb.Append(',');
                }
            }
        }

        var flag = e.EventType.Flag;
        var easing = ((int)e.Easing.GetEasingType()).ToString();
        var startT = Math.Round(e.StartTime).ToEnUsFormatString();
        var endT = e.StartTime.Equals(e.EndTime) ? "" : Math.Round(e.EndTime).ToEnUsFormatString();

        return $"{flag},{easing},{startT},{endT},{vsb.ToString()}";
    }

    public override T Deserialize(ref ValueListBuilder<string> split)
    {
        var paramLength = split.Length - 4;
        if (paramLength != ParameterDimension && paramLength != ParameterDimension * 2)
        {
            throw new ArgumentException("Wrong parameter definition");
        }

        var easing = EasingConvert.ToEasing(split[1]);
        var startTime = double.Parse(split[2]);
        var endTime = string.IsNullOrWhiteSpace(split[3]) ? startTime : double.Parse(split[3]);

        var values = new double[ParameterDimension * 2];
        if (paramLength == ParameterDimension)
        {
            int j = 4;
            for (int i = 0; i < ParameterDimension; i++, j++)
            {
                values[i] = double.Parse(split[j]);
                values[i + ParameterDimension] = double.Parse(split[j]);
            }
        }
        else if (paramLength == ParameterDimension * 2)
        {
            int j = 4;
            for (int i = 0; i < ParameterDimension * 2; i++, j++)
            {
                values[i] = double.Parse(split[j]);
            }
        }

        var keyEvent = new T
        {
            Easing = easing.ToEasingFunction(),
            StartTime = startTime,
            EndTime = endTime,
        };
        if (keyEvent is BasicEvent be)
        {
            be.Values = values;
        }

        return keyEvent;
    }
}