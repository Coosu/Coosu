using System;
using System.Runtime.CompilerServices;
using System.Globalization;
#if NETSTANDARD2_0
using Backports.System;
#endif

namespace Backports

{
    public static partial class Numbers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Parse<T>(this ReadOnlySpan<char> input) where T : unmanaged
            => TryParse(input, out T result) ? result : null;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Parse<T>(this ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? format) where T : unmanaged
            => TryParse(input, style, format, out T result) ? result : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse<T>(this ReadOnlySpan<char> input, out T value) where T : unmanaged
        {
#if NETSTANDARD2_0
            return TryParseBackported(input, out value);
#else
            return TryParseRuntime(input, out value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse<T>(this ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? format, out T value) where T : unmanaged
        {
#if NETSTANDARD2_0
            return TryParseBackported(input, style, format, out value);
#else
            return TryParseRuntime(input, style, format, out value);
#endif
        }

#if NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryParseBackported<T>(this ReadOnlySpan<char> input, out T value) where T : unmanaged
        {
            if(typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(byte) 
            || typeof(T) == typeof(short)
            || typeof(T) == typeof(ushort))
                return TryParseBackported(input, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out value);

            if (typeof(T) == typeof(int))
            {
                var parseStatus =
                    Number.TryParseInt32IntegerStyle(input, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out var result);
                value = Unsafe.As<int, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(uint))
            {
                var parseStatus =
                    Number.TryParseUInt32IntegerStyle(input, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out var result);
                value = Unsafe.As<uint, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(long))
            {
                var parseStatus =
                    Number.TryParseInt64IntegerStyle(input, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out var result);
                value = Unsafe.As<long, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(ulong))
            {
                var parseStatus =
                    Number.TryParseUInt64IntegerStyle(input, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out var result);
                value = Unsafe.As<ulong, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(float)
                || typeof(T) == typeof(double))
                return TryParseBackported(input, NumberStyles.Float | NumberStyles.AllowThousands,
                    NumberFormatInfo.CurrentInfo, out value);

            if (typeof(T) == typeof(decimal))
            {
                var parseStatus = Number.TryParseDecimal(input, NumberStyles.Number, NumberFormatInfo.CurrentInfo,
                    out var result);
                value = Unsafe.As<decimal, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            value = default;
            return false;
        }

        private static bool TryParseBackported<T>(this ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? format,
            out T value) where T : unmanaged
        {
            if (typeof(T) == typeof(sbyte))
            {
                style.ValidateParseStyleInteger();
                // For hex number styles AllowHexSpecifier >> 2 == 0x80 and cancels out MinValue so the check is effectively: (uint)i > byte.MaxValue
                // For integer styles it's zero and the effective check is (uint)(i - MinValue) > byte.MaxValue
                if (Number.TryParseInt32(input, style, NumberFormatInfo.GetInstance(format), out var i) != Number.ParsingStatus.OK
                    || (uint)(i - sbyte.MinValue - ((int)(style & NumberStyles.AllowHexSpecifier) >> 2)) > byte.MaxValue)
                {
                    value = default;
                    return false;
                }

                var sbyteVal = (sbyte) i;
                value = Unsafe.As<sbyte, T>(ref sbyteVal);
                return true;
            }

            if (typeof(T) == typeof(byte))
            {
                style.ValidateParseStyleInteger();

                if (Number.TryParseUInt32(input, style, NumberFormatInfo.GetInstance(format), out var i) != Number.ParsingStatus.OK
                    || i > byte.MaxValue)
                {
                    value = default;
                    return false;
                }

                var byteVal = (byte) i;
                value = Unsafe.As<byte, T>(ref byteVal);
                return true;
            }

            if (typeof(T) == typeof(short))
            {
                style.ValidateParseStyleInteger();

                // For hex number styles AllowHexSpecifier << 6 == 0x8000 and cancels out MinValue so the check is effectively: (uint)i > ushort.MaxValue
                // For integer styles it's zero and the effective check is (uint)(i - MinValue) > ushort.MaxValue
                if (Number.TryParseInt32(input, style, NumberFormatInfo.GetInstance(format), out var i) != Number.ParsingStatus.OK
                    || (uint)(i - short.MinValue - ((int)(style & NumberStyles.AllowHexSpecifier) << 6)) > ushort.MaxValue)
                {
                    value = default;
                    return false;
                }
                var shortVal = (short)i;
                value = Unsafe.As<short, T>(ref shortVal);
                return true;
            }

            if (typeof(T) == typeof(ushort))
            {
                style.ValidateParseStyleInteger();

                if (Number.TryParseUInt32(input, style, NumberFormatInfo.GetInstance(format), out var i) != Number.ParsingStatus.OK
                    || i > ushort.MaxValue)
                {
                    value = default;
                    return false;
                }

                var ushortVal = (ushort)i;
                value = Unsafe.As<ushort, T>(ref ushortVal);
                return true;
            }

            if (typeof(T) == typeof(int))
            {
                style.ValidateParseStyleInteger();

                var parseStatus =
                    Number.TryParseInt32(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<int, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(uint))
            {
                style.ValidateParseStyleInteger();

                var parseStatus =
                    Number.TryParseUInt32(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<uint, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(long))
            {
                style.ValidateParseStyleInteger();

                var parseStatus =
                    Number.TryParseInt64(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<long, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(ulong))
            {
                style.ValidateParseStyleInteger();

                var parseStatus =
                    Number.TryParseUInt64(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<ulong, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            if (typeof(T) == typeof(float))
            {
                style.ValidateParseStyleFloatingPoint();
                var wasSuccessful =
                    Number.TryParseSingle(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<float, T>(ref result);
                return wasSuccessful;
            }

            if (typeof(T) == typeof(double))
            {
                style.ValidateParseStyleFloatingPoint();
                var wasSuccessful =
                    Number.TryParseDouble(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<double, T>(ref result);
                return wasSuccessful;
            }

            if (typeof(T) == typeof(decimal))
            {
                style.ValidateParseStyleFloatingPoint();
                var parseStatus =
                    Number.TryParseDecimal(input, style, NumberFormatInfo.GetInstance(format), out var result);
                value = Unsafe.As<decimal, T>(ref result);
                return parseStatus is Number.ParsingStatus.OK;
            }

            value = default;
            return false;
        }
#else
        private static bool TryParseRuntime<T>(this ReadOnlySpan<char> input, out T value) where T : unmanaged
        {
            if (typeof(T) == typeof(sbyte))
            {
                var wasSuccessful = sbyte.TryParse(input, out var result);
                value = Unsafe.As<sbyte, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(byte))
            {
                var wasSuccessful = byte.TryParse(input, out var result);
                value = Unsafe.As<byte, T>(ref result);
                return wasSuccessful;
            }

            if (typeof(T) == typeof(short))
            {
                var wasSuccessful = short.TryParse(input, out var result);
                value = Unsafe.As<short, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(ushort))
            {
                var wasSuccessful = ushort.TryParse(input, out var result);
                value = Unsafe.As<ushort, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(int))
            {
                var wasSuccessful = int.TryParse(input, out var result);
                value = Unsafe.As<int, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(uint))
            {
                var wasSuccessful = uint.TryParse(input, out var result);
                value = Unsafe.As<uint, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(long)) 
            {
                var wasSuccessful = long.TryParse(input, out var result);
                value = Unsafe.As<long, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(ulong))
            {
                var wasSuccessful = ulong.TryParse(input, out var result);
                value = Unsafe.As<ulong, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(float)) 
            {
                var wasSuccessful = float.TryParse(input, out var result);
                value = Unsafe.As<float, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(double)) 
            {
                var wasSuccessful = double.TryParse(input, out var result);
                value = Unsafe.As<double, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(decimal))
            {
                var wasSuccessful = decimal.TryParse(input, out var result);
                value = Unsafe.As<decimal, T>(ref result);
                return wasSuccessful;
            }
            
            value = default;
            return false;
        }

        private static bool TryParseRuntime<T>(this ReadOnlySpan<char> input, NumberStyles style, IFormatProvider? format,
            out T value) where T : unmanaged
        {
            if (typeof(T) == typeof(sbyte))
            {
                var wasSuccessful = sbyte.TryParse(input, style, format, out var result);
                value = Unsafe.As<sbyte, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(byte))
            {
                var wasSuccessful = byte.TryParse(input, style, format, out var result);
                value = Unsafe.As<byte, T>(ref result);
                return wasSuccessful;
            }

            if (typeof(T) == typeof(short))
            {
                var wasSuccessful = short.TryParse(input, style, format, out var result);
                value = Unsafe.As<short, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(ushort))
            {
                var wasSuccessful = ushort.TryParse(input, style, format, out var result);
                value = Unsafe.As<ushort, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(int))
            {
                var wasSuccessful = int.TryParse(input, style, format, out var result);
                value = Unsafe.As<int, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(uint))
            {
                var wasSuccessful = uint.TryParse(input, style, format, out var result);
                value = Unsafe.As<uint, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(long))
            {
                var wasSuccessful = long.TryParse(input, style, format, out var result);
                value = Unsafe.As<long, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(ulong))
            {
                var wasSuccessful = ulong.TryParse(input, style, format, out var result);
                value = Unsafe.As<ulong, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(float))
            {
                var wasSuccessful = float.TryParse(input, style, format, out var result);
                value = Unsafe.As<float, T>(ref result);
                return wasSuccessful;
            }
            if (typeof(T) == typeof(double))
            {
                var wasSuccessful = double.TryParse(input, style, format, out var result);
                value = Unsafe.As<double, T>(ref result);
                return wasSuccessful;
            }

            if (typeof(T) == typeof(decimal))
            {
                var wasSuccessful = decimal.TryParse(input, style, format, out var result);
                value = Unsafe.As<decimal, T>(ref result);
                return wasSuccessful;
            }
            
            value = default;
            return false;
        }
#endif

        //private static Exception TypeDoesNotSupportTryParse<T>() where T : unmanaged => new NotSupportedException($"{typeof(T)} has no compatible TryParse method");
    }
}
