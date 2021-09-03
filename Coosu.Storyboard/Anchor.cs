namespace Coosu.Storyboard
{
    public struct Anchor<T>
    {
        public Anchor(T x, T y)
        {
            X = x;
            Y = y;
        }
        public T X { get; set; }
        public T Y { get; set; }
    }
}