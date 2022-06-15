namespace Coosu.Database.DataTypes;

public readonly record struct IntDoublePair(int IntValue, double DoubleValue)
{
    public readonly int IntValue = IntValue;
    public readonly double DoubleValue = DoubleValue;
}