namespace Coosu.Storyboard;

public readonly struct Anchor<T>
{
    public readonly T X;
    public readonly T Y;

    public Anchor(T x, T y)
    {
        X = x;
        Y = y;
    }
}