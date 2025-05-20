using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Coosu.Shared.Numerics;

public static class ParseHelper
{
    public static readonly NumberFormatInfo EnUsNumberFormat = new CultureInfo("en-US", false).NumberFormat;
    private const NumberStyles DefaultNumberStyle = NumberStyles.Float | NumberStyles.AllowThousands;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ParseBoolean(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return bool.Parse(input);
#else
        return Backports.Numbers.Parse<bool>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ParseByte(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return byte.Parse(input);
#else
        return Backports.Numbers.Parse<byte>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ParseSByte(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return sbyte.Parse(input);
#else
        return Backports.Numbers.Parse<sbyte>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ParseInt16(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return short.Parse(input);
#else
        return Backports.Numbers.Parse<short>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ParseUInt16(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ushort.Parse(input);
#else
        return Backports.Numbers.Parse<ushort>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt32(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return int.Parse(input);
#else
        return Backports.Numbers.Parse<int>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParseUInt32(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return uint.Parse(input);
#else
        return Backports.Numbers.Parse<uint>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParseInt64(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return long.Parse(input);
#else
        return Backports.Numbers.Parse<long>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ParseUInt64(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ulong.Parse(input);
#else
        return Backports.Numbers.Parse<ulong>(input) ?? throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseSingle(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return float.Parse(input, provider: nfi);
#else
        return Backports.Numbers.Parse<float>(input, DefaultNumberStyle, nfi) ??
               throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return double.Parse(input, provider: nfi);
#else
        return Backports.Numbers.Parse<double>(input, DefaultNumberStyle, nfi) ??
               throw new FormatException();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ParseEnum<T>(string value) where T : struct
    {
#if NETCOREAPP3_1_OR_GREATER
        return Enum.Parse<T>(value);
#else
        return (T)Enum.Parse(typeof(T), value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseDateTime(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return DateTime.Parse(input);
#else
        return DateTime.Parse(input.ToString());
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseBoolean(ReadOnlySpan<char> input, out bool value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return bool.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseByte(ReadOnlySpan<char> input, out byte value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return byte.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseSByte(ReadOnlySpan<char> input, out sbyte value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return sbyte.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt16(ReadOnlySpan<char> input, out short value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return short.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt16(ReadOnlySpan<char> input, out ushort value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ushort.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt32(ReadOnlySpan<char> input, out int value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return int.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt32(ReadOnlySpan<char> input, out uint value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return uint.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt64(ReadOnlySpan<char> input, out long value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return long.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt64(ReadOnlySpan<char> input, out ulong value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ulong.TryParse(input, out value);
#else
        return Backports.Numbers.TryParse(input, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseSingle(ReadOnlySpan<char> input, out float value, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return float.TryParse(input, DefaultNumberStyle, nfi, out value);
#else
        return Backports.Numbers.TryParse(input, DefaultNumberStyle, nfi, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDouble(ReadOnlySpan<char> input, out double value, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return double.TryParse(input, DefaultNumberStyle, nfi, out value);
#else
        return Backports.Numbers.TryParse(input, DefaultNumberStyle, nfi, out value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDateTime(ReadOnlySpan<char> input, out DateTime value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return DateTime.TryParse(input, out value);
#else
        return DateTime.TryParse(input.ToString(), out value);
#endif
    }
}