namespace Coosu.Shared.Numerics;

public readonly struct Vector3D
{
    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public readonly double X;
    public readonly double Y;
    public readonly double Z;
}