﻿namespace OSharp.Storyboard.Events
{
    public interface IEvent
    {
        float StartTime { get; }
        float EndTime { get; }
    }
}