using System;
using System.IO;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Internal;

internal static class ReaderExtensions
{
    public static string ReadStringA(this BinaryReader binaryReader)
    {
        var flag = binaryReader.ReadByte();
        if (flag == 0x00) return "";
        if (flag != 0x0b)
        {
            throw new ArgumentOutOfRangeException(nameof(flag), $"0x{flag:X2}",
                "Error while reading string flag.");
        }

        return binaryReader.ReadString();
    }

    public static IntSinglePair ReadIntSinglePairA(this BinaryReader binaryReader)
    {
        var flag = binaryReader.ReadByte();
        if (flag != 0x08)
        {
            throw new ArgumentOutOfRangeException(nameof(flag), $"0x{flag:X2}",
                "Error while reading IntSinglePair first flag.");
        }

        var intValue = binaryReader.ReadInt32();
        flag = binaryReader.ReadByte();
        if (flag != 0x0c)
        {
            throw new ArgumentOutOfRangeException(nameof(flag), $"0x{flag:X2}",
                "Error while reading IntSinglePair second flag.");
        }

        var singleValue = binaryReader.ReadSingle();
        return new IntSinglePair(intValue, singleValue);
    }

    public static IntDoublePair ReadIntDoublePairA(this BinaryReader binaryReader)
    {
        var flag = binaryReader.ReadByte();
        if (flag != 0x08)
        {
            throw new ArgumentOutOfRangeException(nameof(flag), $"0x{flag:X2}",
                "Error while reading IntDoublePair first flag.");
        }

        var intValue = binaryReader.ReadInt32();
        flag = binaryReader.ReadByte();
        if (flag != 0x0d)
        {
            throw new ArgumentOutOfRangeException(nameof(flag), $"0x{flag:X2}",
                "Error while reading IntDoublePair second flag.");
        }

        var doubleValue = binaryReader.ReadDouble();
        return new IntDoublePair(intValue, doubleValue);
    }

    public static TimingPoint ReadTimingPointA(this BinaryReader binaryReader)
    {
        return new TimingPoint(binaryReader.ReadDouble(), binaryReader.ReadDouble(), !binaryReader.ReadBoolean());
    }

    public static DateTime ReadDateTimeA(this BinaryReader binaryReader)
    {
        return new DateTime(binaryReader.ReadInt64());
    }
}