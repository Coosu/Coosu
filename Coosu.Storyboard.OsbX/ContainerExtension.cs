﻿using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX
{
    public static class ContainerExtension
    {
        public static void ApplyAction(this EventContainer container, CommonEvent commonEvent)
        {
            container.EventList.Add(commonEvent);
        }

        public static void ApplyAction<T>(this EventContainer container, T commonEvent) where T : CommonEvent
        {
            container.EventList.Add(commonEvent);
        }
    }
}