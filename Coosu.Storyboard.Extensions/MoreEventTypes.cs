namespace Coosu.Storyboard.Extensions
{
    public static class MoreEventTypes
    {
        public static EventType MoveBy { get; } = new("Mc", 2, 100);
        public static EventType FadeBy { get; } = new("Fc", 1, 101);
        public static EventType ScaleBy { get; } = new("Sc", 1, 102);
        public static EventType RotateBy { get; } = new("Rc", 1, 103);
        public static EventType ColorBy { get; } = new("Cc", 3, 104);
        public static EventType MoveXBy { get; } = new("MXc", 1, 105);
        public static EventType MoveYBy { get; } = new("MYc", 1, 106);
        public static EventType VectorBy { get; } = new("Vc", 2, 107);
    }
}
