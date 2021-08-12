namespace Coosu.Storyboard
{
    public static class EventTypes
    {
        public static EventType Fade { get; } = new("F");
        public static EventType Move { get; } = new("M");
        public static EventType MoveX { get; } = new("MX");
        public static EventType MoveY { get; } = new("MY");
        public static EventType Scale { get; } = new("S");
        public static EventType Vector { get; } = new("V");
        public static EventType Rotate { get; } = new("R");
        public static EventType Color { get; } = new("C");
        public static EventType Parameter { get; } = new("P");
        public static EventType Loop { get; } = new("L");
        public static EventType Trigger { get; } = new("T");
    }
}