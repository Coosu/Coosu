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

        //        var rawLength = binaryReader.BaseStream.ReadLEB128Unsigned();
        //        if (rawLength > int.MaxValue)
        //        {
        //            throw new ArgumentException("Error while reading string flag. The string length is too long.");
        //        }

        //        var length = (int)rawLength;
        //        if (length == 0) return "";

        //        byte[]? buffer = null;

        //        Span<byte> span = length <= Constants.MaxStackLength
        //            ? stackalloc byte[length]
        //            : buffer = ArrayPool<byte>.Shared.Rent(length);
        //        try
        //        {
        //            if (buffer != null)
        //            {
        //                span = span.Slice(0, length);
        //            }

        //#if NETFRAMEWORK
        //            for (var i = 0; i < span.Length; i++)
        //            {
        //                span[i] = (byte)binaryReader.BaseStream.ReadByte();
        //            }

        //            unsafe
        //            {
        //                fixed (byte* p = span)
        //                {
        //                    var str = Encoding.UTF8.GetString(p, span.Length);
        //                    return str;
        //                }
        //            }
        //#else
        //            var readLen = binaryReader.BaseStream.Read(span);
        //            if (readLen < length)
        //            {
        //                throw new ArgumentException("Error while reading string. The string length doesn't match.");
        //            }

        //            var str = Encoding.UTF8.GetString(span);
        //            return str;
        //#endif
        //        }
        //        finally
        //        {
        //            if (buffer != null)
        //            {
        //                ArrayPool<byte>.Shared.Return(buffer);
        //            }
        //        }
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