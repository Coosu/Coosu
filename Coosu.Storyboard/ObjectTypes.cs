namespace Coosu.Storyboard
{
    public static class ObjectTypes
    {
        public static ObjectType Background { get; } = new(0);
        public static ObjectType Video { get; } = new(1);
        public static ObjectType Break { get; } = new(2);
        public static ObjectType Color { get; } = new(3);
        public static ObjectType Sprite { get; } = new(4);
        public static ObjectType Sample { get; } = new(5);
        public static ObjectType Animation { get; } = new(6);
    }
}