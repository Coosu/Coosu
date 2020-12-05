using System;

namespace Coosu.Storyboard.Events
{
    public enum EventType
    {
        Move, Fade, Scale, Rotate, Color, MoveX, MoveY, Vector, Parameter, Loop, Trigger
    }

    public static class EventEnumExtension
    {
        public static string ToShortString(this EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Fade:
                    return "F";
                case EventType.Move:
                    return "M";
                case EventType.MoveX:
                    return "MX";
                case EventType.MoveY:
                    return "MY";
                case EventType.Scale:
                    return "S";
                case EventType.Vector:
                    return "V";
                case EventType.Rotate:
                    return "R";
                case EventType.Color:
                    return "C";
                case EventType.Parameter:
                    return "P";
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        public static EventType ToCommandType(this string shortHand)
        {
            switch (shortHand)
            {
                case "F": return EventType.Fade;
                case "M": return EventType.Move;
                case "MX": return EventType.MoveX;
                case "MY": return EventType.MoveY;
                case "S": return EventType.Scale;
                case "V": return EventType.Vector;
                case "R": return EventType.Rotate;
                case "C": return EventType.Color;
                case "P": return EventType.Parameter;
                case "L": return EventType.Loop;
                case "T": return EventType.Trigger;
            }
            return EventType.Fade;
        }
    }
}
