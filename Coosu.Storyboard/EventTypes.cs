namespace Coosu.Storyboard
{
    public static class EventTypes
    {
        public static EventType Fade { get; } = new("F", 1);
        public static EventType Move { get; } = new("M", 2);
        public static EventType MoveX { get; } = new("MX", 1);
        public static EventType MoveY { get; } = new("MY", 1);
        public static EventType Scale { get; } = new("S", 1);
        public static EventType Vector { get; } = new("V", 2);
        public static EventType Rotate { get; } = new("R", 1);
        public static EventType Color { get; } = new("C", 3);
        public static EventType Parameter { get; } = new("P", 0);
        public static EventType Loop { get; } = new("L", -1);
        public static EventType Trigger { get; } = new("T", -1);
    }
}