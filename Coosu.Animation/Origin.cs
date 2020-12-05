namespace Coosu.Animation
{
    public struct Origin<T>
    {
        public Origin(T x, T y)
        {
            X = x;
            Y = y;
        }
        public T X { get; set; }
        public T Y { get; set; }
    }
}