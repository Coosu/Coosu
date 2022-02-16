namespace Coosu.Shared.Numerics
{
    public readonly struct ReadyOnlyVector3<T>
    {
        public ReadyOnlyVector3(T x, T y, T z) : this()
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