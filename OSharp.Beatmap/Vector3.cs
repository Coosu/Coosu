namespace OSharp.Beatmap
{
    public struct Vector3<T>
    {
        public Vector3(T x, T y, T z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public T X { get; }
        public T Y { get; }
        public T Z { get; }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }
    }
}