namespace Coosu.Shared.Numerics;

public readonly struct Vector2D
{
    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public readonly double X;
    public readonly double Y;
}