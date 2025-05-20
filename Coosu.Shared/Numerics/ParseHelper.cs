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
    public static bool TryParseEnum<T>(ReadOnlySpan<char> input, out T value) where T : struct
    {
#if NETCOREAPP3_1_OR_GREATER
        return Enum.TryParse<T>(input, out value);
#else
        return Enum.TryParse(input.ToString(), false, out value);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> s, out T result) where T : struct =>
        TryParse(s, EnUsNumberFormat, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> s, NumberFormatInfo? provider, out T result) where T : struct
    {
        result = default;
        return typeof(T) switch
        {
            // Floating point types (provider sensitive)
            { } t when t == typeof(double) => TryParseDouble(s, out Unsafe.As<T, double>(ref result), provider),
            { } t when t == typeof(float) => TryParseSingle(s, out Unsafe.As<T, float>(ref result), provider),

            // Integer types (provider is ignored by these specific TryParse helpers)
            { } t when t == typeof(byte) => TryParseByte(s, out Unsafe.As<T, byte>(ref result)),
            { } t when t == typeof(sbyte) => TryParseSByte(s, out Unsafe.As<T, sbyte>(ref result)),
            { } t when t == typeof(short) => TryParseInt16(s, out Unsafe.As<T, short>(ref result)),
            { } t when t == typeof(ushort) => TryParseUInt16(s, out Unsafe.As<T, ushort>(ref result)),
            { } t when t == typeof(int) => TryParseInt32(s, out Unsafe.As<T, int>(ref result)),
            { } t when t == typeof(uint) => TryParseUInt32(s, out Unsafe.As<T, uint>(ref result)),
            { } t when t == typeof(long) => TryParseInt64(s, out Unsafe.As<T, long>(ref result)),
            { } t when t == typeof(ulong) => TryParseUInt64(s, out Unsafe.As<T, ulong>(ref result)),

            // Other types (provider is ignored by these specific TryParse helpers)
            { } t when t == typeof(bool) => TryParseBoolean(s, out Unsafe.As<T, bool>(ref result)),
            { } t when t == typeof(DateTime) => TryParseDateTime(s, out Unsafe.As<T, DateTime>(ref result)),

            { IsEnum: true } => TryParseEnum(s, out Unsafe.As<T, DateTime>(ref result)),

            _ => throw new NotSupportedException(
                $"Type {typeof(T)} is not supported by this generic TryParse method. " +
                $"Supported types are: double, float, byte, sbyte, short, ushort, int, uint, long, ulong, bool, DateTime.")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseNext<T>(ref StringExtensions.CharSplitEnumerator enumerator, out T value) where T : struct
    {
        if (enumerator.MoveNext() && TryParse(enumerator.Current, out value)) return true;
        value = default;
        return false;
    }
}