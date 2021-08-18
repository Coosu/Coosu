namespace Coosu.Storyboard.Extensions
{
    public static class MoreEventTypes
    {
        public static EventType MoveBy { get; } = new("M~", 2, 101); // 100 greater than the original event 
        public static EventType FadeBy { get; } = new("F~", 1, 102);
        public static EventType ScaleBy { get; } = new("S~", 1, 103);
        public static EventType RotateBy { get; } = new("R~", 1, 104);
        public static EventType ColorBy { get; } = new("C~", 3, 105);
        public static EventType MoveXBy { get; } = new("MX~", 1, 106);
        public static EventType MoveYBy { get; } = new("MY~", 1, 107);
        public static EventType VectorBy { get; } = new("V~", 2, 108);
    }
}
