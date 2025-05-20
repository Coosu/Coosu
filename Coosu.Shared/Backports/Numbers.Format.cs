using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Backports
{
    public static partial class Numbers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormat<T>(this T @this, Span<char> destination, out int charsWritten,
            ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : unmanaged
        {
#if NETSTANDARD2_0
            return TryFormatBackported(@this, destination, out charsWritten, format, provider);

#else
            return TryFormatRuntime(@this, destination, out charsWritten, format, provider);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringCompat<T>(this T @this, string? format = null, IFormatProvider? provider = null)
            where T : unmanaged
        {
            ThrowIfTypeNotSupported<T>();
            Span<char> destination = stackalloc char[
            //    System.Number.CharStackBufferSize
            64
            ];
            // ReSharper disable once MergeConditionalExpression
            if (@this.TryFormat(destination, out var nChars, format is not null ? format.AsSpan() : default, provider))
                return destination.Slice(0, nChars).ToString();
            throw new FormatException("Unable to format value");
        }

#if NETSTANDARD2_0
        private static bool TryFormatBackported<T>(this T @this, Span<char> destination, out int charsWritten,
            ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : unmanaged
        {
            if (typeof(T) == typeof(sbyte))
                return System.Number.TryFormatInt32(Unsafe.As<T, sbyte>(ref @this), 0x000000FF, format, provider,
                    destination, out charsWritten);
            if (typeof(T) == typeof(byte))
                return System.Number.TryFormatUInt32(Unsafe.As<T, byte>(ref @this), format, provider, destination,
                    out charsWritten);
            if (typeof(T) == typeof(short))
                return System.Number.TryFormatInt32(Unsafe.As<T, short>(ref @this), 0x0000FFFF, format, provider, 
                    destination, out charsWritten);
            if (typeof(T) == typeof(ushort))
                return System.Number.TryFormatUInt32(Unsafe.As<T, ushort>(ref @this), format, provider, destination,
                    out charsWritten);
            if (typeof(T) == typeof(int))
                return System.Number.TryFormatInt32(Unsafe.As<T, int>(ref @this), ~0, format, provider, destination,
                    out charsWritten);
            if (typeof(T) == typeof(uint))
                return System.Number.TryFormatUInt32(Unsafe.As<T, uint>(ref @this), format, provider, destination,
                    out charsWritten);
            if (typeof(T) == typeof(long))
                return System.Number.TryFormatInt64(Unsafe.As<T, long>(ref @this), format, provider, destination,
                    out charsWritten);
            if (typeof(T) == typeof(ulong))
                return System.Number.TryFormatUInt64(Unsafe.As<T, ulong>(ref @this), format, provider, destination,
                    out charsWritten);
            if(typeof(T) == typeof(float))
                return System.Number.TryFormatSingle(Unsafe.As<T, float>(ref @this), format, NumberFormatInfo.GetInstance(provider),
                    destination, out charsWritten);
            if (typeof(T) == typeof(double))
                return System.Number.TryFormatDouble(Unsafe.As<T, double>(ref @this), format, NumberFormatInfo.GetInstance(provider),
                    destination, out charsWritten);
            if (typeof(T) == typeof(decimal))
                return System.Number.TryFormatDecimal(Unsafe.As<T, decimal>(ref @this), format, NumberFormatInfo.GetInstance(provider),
                    destination, out charsWritten);

            //throw TypeDoesNotSupportTryFormat<T>();
            charsWritten = 0;
            return false;
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatRuntime<T>(this T @this, Span<char> destination, out int charsWritten,
            ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : unmanaged
        {
            if(typeof(T) == typeof(sbyte))
                return Unsafe.As<T, sbyte>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if(typeof(T) == typeof(byte))
                return Unsafe.As<T, byte>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(short))
                return Unsafe.As<T, short>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(ushort))
                return Unsafe.As<T, ushort>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(int))
                return Unsafe.As<T, int>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(uint))
                return Unsafe.As<T, uint>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(long))
                return Unsafe.As<T, long>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(ulong))
                return Unsafe.As<T, ulong>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(float))
                return Unsafe.As<T, float>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(double))
                return Unsafe.As<T, double>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            if (typeof(T) == typeof(decimal))
                return Unsafe.As<T, decimal>(ref @this).TryFormat(destination, out charsWritten, format, provider);
            //throw TypeDoesNotSupportTryFormat<T>();
            charsWritten = 0;
            return false;
        }
#endif
        //private static Exception TypeDoesNotSupportTryFormat<T>() where T : unmanaged => new NotSupportedException($"{typeof(T)} has no compatible TryFormat method");

        private static void ThrowIfTypeNotSupported<T>() where T : unmanaged
        {
            if (typeof(T) == typeof(sbyte) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(short) ||
                typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(int) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(long) ||
                typeof(T) == typeof(ulong) ||
                typeof(T) == typeof(float) ||
                typeof(T) == typeof(double) ||
                typeof(T) == typeof(decimal))
                return;

            throw new NotSupportedException($"{typeof(T)} does not support parsing/formatting");
        }

    }
}
