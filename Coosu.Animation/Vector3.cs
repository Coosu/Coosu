namespace Coosu.Animation;

public struct Vector3<T>
{
    public Vector3(T x, T y,T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    public static implicit operator Vector3<T>((T, T, T) tuple)
    {
        var (x, y, z) = tuple;
        return new Vector3<T>(x, y, z);
    }
}