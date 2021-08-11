namespace Coosu.Storyboard
{
    public static class ObjectTypes
    {
        public static OsbObjectType Background { get; } = new(0);
        public static OsbObjectType Video { get; } = new(1);
        public static OsbObjectType Break { get; } = new(2);
        public static OsbObjectType Color { get; } = new(3);
        public static OsbObjectType Sprite { get; } = new(4);
        public static OsbObjectType Sample { get; } = new(5);
        public static OsbObjectType Animation { get; } = new(6);
    }
}