namespace Coosu.Database.DataTypes;

public readonly record struct IntSinglePair(int IntValue, float SingleValue)
{
    public readonly int IntValue = IntValue;
    public readonly float SingleValue = SingleValue;
}