namespace Coosu.Animation
{
    public struct Vector2<T>
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; set; }
        public T Y { get; set; }

        public static implicit operator Vector2<T>((T, T) tuple)
        {
            var (x, y) = tuple;
            return new Vector2<T>(x, y);
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }
    }
}
