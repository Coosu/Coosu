namespace Coosu.Storyboard.Common
{
    public struct Vector2
    {
        public Vector2(float x, float y) : this()
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }
    }

    public struct Vector3
    {
        public Vector3(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
    }
}
