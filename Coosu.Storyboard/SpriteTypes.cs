namespace Coosu.Storyboard
{
    public static class SpriteTypes
    {
        public static SpriteType Background { get; } = new(0);
        public static SpriteType Video { get; } = new(1);
        public static SpriteType Break { get; } = new(2);
        public static SpriteType Color { get; } = new(3);
        public static SpriteType Sprite { get; } = new(4);
        public static SpriteType Sample { get; } = new(5);
        public static SpriteType Animation { get; } = new(6);
    }
}