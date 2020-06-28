namespace OSharp.Beatmap
{
    public struct Vector2<T>
    {
        public Vector2(T x, T y) : this()
        {
            X = x;
            Y = y;
        }

        public T X { get; set; }
        public T Y { get; set; }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}