namespace Coosu.Database.DataTypes;

public readonly record struct TimingPoint(double Bpm, double Offset, bool IsInherited)
{
    public readonly double Bpm = Bpm;
    public readonly double Offset = Offset;
    public readonly bool IsInherited = IsInherited;
}