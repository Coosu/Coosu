using Coosu.Database.DataTypes;

namespace Coosu.Database;

public abstract class ReaderBase
{
    public string? Name { get; protected set; }
    public object? Value { get; protected set; }
    public NodeType NodeType { get; protected set; }
    public DataType DataType { get; protected set; }

    public byte GetByte() => (byte)Value!;
    public short GetInt16() => (short)Value!;
    public int GetInt32() => (int)Value!;
    public long GetInt64() => (long)Value!;
    public ushort GetUInt16() => (ushort)Value!;
    public uint GetUInt32() => (uint)Value!;
    public ulong GetUInt64() => (ulong)Value!;
    public float GetSingle() => (float)Value!;
    public double GetDouble() => (double)Value!;
    public bool GetBoolean() => (bool)Value!;
    public string GetString() => (string)Value!;
}