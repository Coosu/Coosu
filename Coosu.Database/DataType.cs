namespace Coosu.Database;

public enum DataType
{
    Unknown = -1, Object, Byte, Int16, Int32, Int64, ULEB128, Single, Double, Boolean, String,
    IntDoublePair, TimingPoint, DateTime, Array,
}