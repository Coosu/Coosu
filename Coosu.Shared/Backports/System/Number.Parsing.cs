// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETSTANDARD2_0

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using static RefTools.Ref;

namespace Backports.System
{
    // The Parse methods provided by the numeric classes convert a
    // string to a numeric value. The optional style parameter specifies the
    // permitted style of the numeric string. It must be a combination of bit flags
    // from the NumberStyles enumeration. The optional info parameter
    // specifies the NumberFormatInfo instance to use when parsing the
    // string. If the info parameter is null or omitted, the numeric
    // formatting information is obtained from the current culture.
    //
    // Numeric strings produced by the Format methods using the Currency,
    // Decimal, Engineering, Fixed point, General, or Number standard formats
    // (the C, D, E, F, G, and N format specifiers) are guaranteed to be parseable
    // by the Parse methods if the NumberStyles.Any style is
    // specified. Note, however, that the Parse methods do not accept
    // NaNs or Infinities.

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal static partial class Number
    {
        private const int Int32Precision = 10;
        private const int UInt32Precision = Int32Precision;
        private const int Int64Precision = 19;
        private const int UInt64Precision = 20;

        private const int DoubleMaxExponent = 309;
        private const int DoubleMinExponent = -324;

        private const int FloatingPointMaxExponent = DoubleMaxExponent;
        private const int FloatingPointMinExponent = DoubleMinExponent;

        private const int SingleMaxExponent = 39;
        private const int SingleMinExponent = -45;

        private const int HalfMaxExponent = 5;
        private const int HalfMinExponent = -8;

        private static bool TryNumberToInt32(in NumberBuffer number, out int value)
        {
            value = default;
            number.CheckConsistency();

            var i = number.Scale;
            if (i > Int32Precision || i < number.DigitsCount)
                return false;
            //var p = number.GetDigitsPointer();
            ref readonly var p = ref number.GetRef();
            var n = 0;
            while (--i >= 0)
            {
                if ((uint)n > 0x7FFFFFFF / 10)
                {
                    return false;
                }
                n *= 10;
                if (p == '\0') 
                    continue;
                //n += (*p++ - '0');
                n += p - '0';
                //p = ref Ref.Increment(ref p);
                p = ref Inc(in p);
            }
            if (number.IsNegative)
            {
                n = -n;
                if (n > 0)
                    return false;
            }
            else
            {
                if (n < 0)
                    return false;
            }
            value = n;
            return true;
        }

        private static bool TryNumberToInt64(in NumberBuffer number, out long value)
        {
            value = default;
            number.CheckConsistency();

            var i = number.Scale;
            if (i > Int64Precision || i < number.DigitsCount)
                return false;
            ref readonly var p = ref number.GetRef();
            long n = 0;
            while (--i >= 0)
            {
                if ((ulong)n > 0x7FFFFFFFFFFFFFFF / 10)
                    return false;
                n *= 10;
                if (p == '\0') 
                    continue;
                //n += (*p++ - '0');
                n += p - '0';
                p = ref Inc(in p);
            }
            if (number.IsNegative)
            {
                n = -n;
                if (n > 0)
                    return false;
            }
            else
            {
                if (n < 0)
                    return false;
            }
            value = n;
            return true;
        }

        private static bool TryNumberToUInt32(in NumberBuffer number, out uint value)
        {
            value = default;
            number.CheckConsistency();

            var i = number.Scale;
            if (i > UInt32Precision || i < number.DigitsCount || number.IsNegative)
                return false;
            ref readonly var p = ref number.GetRef();
            uint n = 0;
            while (--i >= 0)
            {
                if (n > 0xFFFFFFFF / 10)
                {
                    return false;
                }
                n *= 10;
                if (p == '\0') 
                    continue;
                //var newN = n + (uint)(*p++ - '0');
                var newN = n + (uint) (p - '0');
                p = ref Inc(in p);
                // Detect an overflow here...
                if (newN < n)
                    return false;
                n = newN;
            }
            value = n;
            return true;
        }

        private static bool TryNumberToUInt64(in NumberBuffer number, out ulong value)
        {
            value = default;
            number.CheckConsistency();

            var i = number.Scale;
            if (i > UInt64Precision || i < number.DigitsCount || number.IsNegative)
                return false;
            ref readonly var p = ref number.GetRef();
            ulong n = 0;
            while (--i >= 0)
            {
                if (n > 0xFFFFFFFFFFFFFFFF / 10)
                    return false;
                n *= 10;
                if (p == '\0') 
                    continue;
                //var newN = n + (ulong)(*p++ - '0');
                var newN = n + (ulong) (p - '0');
                p = ref Inc(in p);
                // Detect an overflow here...
                if (newN < n)
                    return false;
                n = newN;
            }
            value = n;
            return true;
        }

        // Per https://github.com/pgovind/runtime/commit/9897a27aaace156628735acdcb06938e3a24ce15
        // To fix https://github.com/dotnet/runtime/issues/48648
        private static bool TryParseNumber(ReadOnlySpan<char> str, NumberStyles styles, 
                                             ref NumberBuffer number, NumberFormatInfo info, out int parsed)
        {
            static char IncPtr(ref ReadOnlySpan<char> p)
            {
                p = p.Length > 0 ? p.Slice(1) : p;
                //ch = ++p < strEnd ? *p : '\0';
                return p.IsEmpty ? '\0' : p[0];
            }

            Debug.Assert(!str.IsEmpty);
            Debug.Assert((styles & NumberStyles.AllowHexSpecifier) == 0);

            const int stateSign = 0x0001;
            const int stateParens = 0x0002;
            const int stateDigits = 0x0004;
            const int stateNonZero = 0x0008;
            const int stateDecimal = 0x0010;
            const int stateCurrency = 0x0020;

            Debug.Assert(number.DigitsCount == 0);
            Debug.Assert(number.Scale == 0);
            Debug.Assert(!number.IsNegative);
            Debug.Assert(!number.HasNonZeroTail);

            number.CheckConsistency();

            string decSep;                  // decimal separator from NumberFormatInfo.
            string groupSep;                // group separator from NumberFormatInfo.
            string? currSymbol = null;       // currency symbol from NumberFormatInfo.

            var parsingCurrency = false;
            if ((styles & NumberStyles.AllowCurrencySymbol) != 0)
            {
                currSymbol = info.CurrencySymbol;

                // The idea here is to match the currency separators and on failure match the number separators to keep the perf of VB's IsNumeric fast.
                // The values of decSep are setup to use the correct relevant separator (currency in the if part and decimal in the else part).
                decSep = info.CurrencyDecimalSeparator;
                groupSep = info.CurrencyGroupSeparator;
                parsingCurrency = true;
            }
            else
            {
                decSep = info.NumberDecimalSeparator;
                groupSep = info.NumberGroupSeparator;
            }

            var state = 0;
            ReadOnlySpan<char> p = str;
            //char ch = p < strEnd ? *p : '\0';
            var ch = p.IsEmpty ? '\0' : p[0];
            ReadOnlySpan<char> next;

            while (true)
            {
                // Eat whitespace unless we've found a sign which isn't followed by a currency symbol.
                // "-Kr 1231.47" is legal but "- 1231.47" is not.
                if (!IsWhite(ch)
                  || (styles & NumberStyles.AllowLeadingWhite) == 0
                  || (state & stateSign)       != 0
                  && (state & stateCurrency)   == 0
                  && info.NumberNegativePattern != 2)
                {
                    if ((styles & NumberStyles.AllowLeadingSign) != 0 && (state & stateSign) == 0 &&
                        ((next = MatchChars(p, info.PositiveSign.AsSpan())).IsNotEmpty() ||
                         (next = MatchChars(p, info.NegativeSign.AsSpan())).IsNotEmpty() && (number.IsNegative = true)))
                    {
                        state |= stateSign;
                        p = next;
                    }
                    else if (ch == '(' && (styles & NumberStyles.AllowParentheses) != 0 && (state & stateSign) == 0)
                    {
                        state |= stateSign | stateParens;
                        number.IsNegative = true;
                    }
                    else if (currSymbol != null && (next = MatchChars(p, currSymbol.AsSpan())).IsNotEmpty())
                    {
                        state |= stateCurrency;
                        currSymbol = null;
                        // We already found the currency symbol. There should not be more currency symbols. Set
                        // currSymbol to NULL so that we won't search it again in the later code path.
                        p = next;
                    }
                    else
                    {
                        break;
                    }
                }

                //ch = ++p < strEnd ? *p : '\0';
                ch = IncPtr(ref p);
            }

            var digCount = 0;
            var digEnd = 0;
            var maxDigCount = number.Digits.Length - 1;
            var numberOfTrailingZeros = 0;

            while (true)
            {
                if (IsDigit(ch))
                {
                    state |= stateDigits;

                    if (ch != '0' || (state & stateNonZero) != 0)
                    {
                        if (digCount < maxDigCount)
                        {
                            number.DigitsMut[digCount] = (byte)ch;
                            if (ch != '0' || number.Kind != NumberBufferKind.Integer)
                            {
                                digEnd = digCount + 1;
                            }
                        }
                        else if (ch != '0')
                        {
                            // For decimal and binary floating-point numbers, we only
                            // need to store digits up to maxDigCount. However, we still
                            // need to keep track of whether any additional digits past
                            // maxDigCount were non-zero, as that can impact rounding
                            // for an input that falls evenly between two representable
                            // results.

                            number.HasNonZeroTail = true;
                        }

                        if ((state & stateDecimal) == 0)
                        {
                            number.Scale++;
                        }

                        if (digCount < maxDigCount)
                        {
                            // Handle a case like "53.0". We need to ignore trailing zeros in the fractional part for floating point numbers, so we keep a count of the number of trailing zeros and update digCount later
                            if (ch == '0')
                            {
                                numberOfTrailingZeros++;
                            }
                            else
                            {
                                numberOfTrailingZeros = 0;
                            }
                        }
                        digCount++;
                        state |= stateNonZero;
                    }
                    else if ((state & stateDecimal) != 0)
                    {
                        number.Scale--;
                    }
                }
                else if ((styles & NumberStyles.AllowDecimalPoint) != 0 
                      && (state & stateDecimal) == 0
                      && ((next = MatchChars(p, decSep.AsSpan())).IsNotEmpty() 
                       || parsingCurrency 
                       && (state & stateCurrency) == 0                                         
                       && (next = MatchChars(p, info.NumberDecimalSeparator.AsSpan())).IsNotEmpty()))
                {
                    state |= stateDecimal;
                    p = next;
                }
                else if ((styles & NumberStyles.AllowThousands) != 0
                      && (state  & stateDigits)                 != 0
                      && (state & stateDecimal)                == 0
                      && ((next = MatchChars(p, groupSep.AsSpan())).IsNotEmpty()
                       || parsingCurrency
                       && (state & stateCurrency) == 0
                       && (next = MatchChars(p, info.NumberGroupSeparator.AsSpan())).IsNotEmpty()))
                {
                    p = next;
                }
                else
                {
                    break;
                }
                //ch = ++p < strEnd ? *p : '\0';
                ch = IncPtr(ref p);
            }

            var negExp = false;
            number.DigitsCount = digEnd;
            number.DigitsMut[digEnd] = (byte)'\0';
            if ((state & stateDigits) != 0)
            {
                if ((ch == 'E' || ch == 'e') && (styles & NumberStyles.AllowExponent) != 0)
                {
                    ReadOnlySpan<char> temp = p;
                    //ch = ++p < strEnd ? *p : '\0';
                    ch = IncPtr(ref p);
                    
                    if ((next = MatchChars(p, info.PositiveSign.AsSpan())).IsNotEmpty())
                    {
                        ch = (p = next.Slice(1)).IsNotEmpty() ? p[0] : '\0';
                    }
                    else if ((next = MatchChars(p, info.NegativeSign.AsSpan())).IsNotEmpty())
                    {
                        ch = (p = next.Slice(1)).IsNotEmpty() ? p[0] : '\0';
                        negExp = true;
                    }
                    if (IsDigit(ch))
                    {
                        var exp = 0;
                        do
                        {
                            exp = exp * 10 + (ch - '0');
                            //ch = ++p < strEnd ? *p : '\0';
                            ch = IncPtr(ref p);

                            if (exp > 1000)
                            {
                                exp = 9999;
                                while (IsDigit(ch))
                                {
                                    //ch = ++p < strEnd ? *p : '\0';
                                    ch = IncPtr(ref p);
                                }
                            }
                        } while (IsDigit(ch));
                        if (negExp)
                        {
                            exp = -exp;
                        }
                        number.Scale += exp;
                    }
                    else
                    {
                        p = temp;
                        //ch = ++p < strEnd ? *p : '\0';
                        ch = IncPtr(ref p);
                    }
                }

                if (number.Kind == NumberBufferKind.FloatingPoint && !number.HasNonZeroTail)
                {
                    // Adjust the number buffer for trailing zeros
                    var numberOfFractionalDigits = digEnd - number.Scale;
                    if (numberOfFractionalDigits > 0)
                    {
                        numberOfTrailingZeros = Math.Min(numberOfTrailingZeros, numberOfFractionalDigits);
                        number.DigitsCount = digEnd - numberOfTrailingZeros;
                        number.DigitsMut[number.DigitsCount] = (byte)'\0';
                    }
                }

                while (true)
                {
                    if (!IsWhite(ch) || (styles & NumberStyles.AllowTrailingWhite) == 0)
                    {
                        if ((styles & NumberStyles.AllowTrailingSign) != 0 
                         && (state & stateSign) == 0
                         && ((next = MatchChars(p, info.PositiveSign.AsSpan())).IsNotEmpty()
                          || (next = MatchChars(p, info.NegativeSign.AsSpan())).IsNotEmpty() && (number.IsNegative = true)))
                        {
                            state |= stateSign;
                            p = next;
                        }
                        else if (ch == ')' && (state & stateParens) != 0)
                        {
                            state &= ~stateParens;
                        }
                        else if (currSymbol != null && (next = MatchChars(p, currSymbol.AsSpan())).IsNotEmpty())
                        {
                            currSymbol = null;
                            p = next;
                        }
                        else
                        {
                            break;
                        }
                    }
                    //ch = ++p < strEnd ? *p : '\0';
                    ch = IncPtr(ref p);
                }
                if ((state & stateParens) == 0)
                {
                    if ((state & stateNonZero) == 0)
                    {
                        if (number.Kind != NumberBufferKind.Decimal)
                        {
                            number.Scale = 0;
                        }
                        if (number.Kind == NumberBufferKind.Integer && (state & stateDecimal) == 0)
                        {
                            number.IsNegative = false;
                        }
                    }
                    //str = p; // assignment
                    parsed = str.Length - p.Length;
                    return true;
                }
            }
            //str = p; // assignment
            parsed = str.Length - p.Length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParsingStatus TryParseInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
        {
            if ((styles & ~NumberStyles.Integer) == 0)
            {
                // Optimized path for the common case of anything that's allowed for integer style.
                return TryParseInt32IntegerStyle(value, styles, info, out result);
            }

            if ((styles & NumberStyles.AllowHexSpecifier) != 0)
            {
                result = 0;
                return TryParseUInt32HexNumberStyle(value, styles, out Unsafe.As<int, uint>(ref result));
            }

            return TryParseInt32Number(value, styles, info, out result);
        }

        private static ParsingStatus TryParseInt32Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
        {
            result = 0;
            var number = new NumberBuffer(NumberBufferKind.Integer, stackalloc byte[Int32NumberBufferLength]);

            if (!TryStringToNumber(value, styles, ref number, info))
                return ParsingStatus.Failed;

            return TryNumberToInt32(in number, out result) ? ParsingStatus.OK : ParsingStatus.Overflow;
        }

        /// <summary>Parses int limited to styles that make up NumberStyles.Integer.</summary>
        internal static ParsingStatus TryParseInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
        {
            Debug.Assert((styles & ~NumberStyles.Integer) == 0, "Only handles subsets of Integer format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            // Parse leading sign.
            var sign = 1;
            if ((styles & NumberStyles.AllowLeadingSign) != 0)
            {
                if (info.HasInvariantNumberSigns())
                {
                    switch (num)
                    {
                        case '-':
                        {
                            sign = -1;
                            index++;
                            if ((uint)index >= (uint)value.Length)
                                goto FalseExit;
                            num = value[index];
                            break;
                        }
                        case '+':
                        {
                            index++;
                            if ((uint)index >= (uint)value.Length)
                                goto FalseExit;
                            num = value[index];
                            break;
                        }
                    }
                }
                else
                {
                    value = value.Slice(index);
                    index = 0;
                    string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
                    if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
                    {
                        index += positiveSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
                    {
                        sign = -1;
                        index += negativeSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
            }

            var overflow = false;
            var answer = 0;

            if (IsDigit(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                }

                // Parse most digits, up to the potential for overflow, which can't happen until after 9 digits.
                answer = num - '0'; // first digit
                index++;
                for (var i = 0; i < 8; i++) // next 8 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEnd;
                    num = value[index];
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                    index++;
                    answer = 10 * answer + num - '0';
                }

                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEnd;
                num = value[index];
                if (!IsDigit(num))
                    goto HasTrailingChars;
                index++;
                // Potential overflow now processing the 10th digit.
                overflow = answer > int.MaxValue / 10;
                answer = answer * 10 + num - '0';
                overflow |= (uint)answer > int.MaxValue + ((uint)sign >> 31);
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility.
                num = value[index];
                while (IsDigit(num))
                {
                    overflow = true;
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                }
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
                goto OverflowExit;

            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = answer * sign;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        /// <summary>Parses long inputs limited to styles that make up NumberStyles.Integer.</summary>
        internal static ParsingStatus TryParseInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
        {
            Debug.Assert((styles & ~NumberStyles.Integer) == 0, "Only handles subsets of Integer format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            // Parse leading sign.
            var sign = 1;
            if ((styles & NumberStyles.AllowLeadingSign) != 0)
            {
                if (info.HasInvariantNumberSigns())
                {
                    if (num == '-')
                    {
                        sign = -1;
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (num == '+')
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
                else
                {
                    value = value.Slice(index);
                    index = 0;
                    string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
                    if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
                    {
                        index += positiveSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
                    {
                        sign = -1;
                        index += negativeSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
            }

            var overflow = false;
            long answer = 0;

            if (IsDigit(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                }

                // Parse most digits, up to the potential for overflow, which can't happen until after 18 digits.
                answer = num - '0'; // first digit
                index++;
                for (var i = 0; i < 17; i++) // next 17 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEnd;
                    num = value[index];
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                    index++;
                    answer = 10 * answer + num - '0';
                }

                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEnd;
                num = value[index];
                if (!IsDigit(num))
                    goto HasTrailingChars;
                index++;
                // Potential overflow now processing the 19th digit.
                overflow = answer > long.MaxValue / 10;
                answer = answer * 10 + num - '0';
                overflow |= (ulong)answer > (ulong)long.MaxValue + ((uint)sign >> 31);
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility.
                num = value[index];
                while (IsDigit(num))
                {
                    overflow = true;
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                }
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
                goto OverflowExit;
            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = answer * sign;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParsingStatus TryParseInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
        {
            if ((styles & ~NumberStyles.Integer) == 0)
            {
                // Optimized path for the common case of anything that's allowed for integer style.
                return TryParseInt64IntegerStyle(value, styles, info, out result);
            }

            if ((styles & NumberStyles.AllowHexSpecifier) != 0)
            {
                result = 0;
                return TryParseUInt64HexNumberStyle(value, styles, out Unsafe.As<long, ulong>(ref result));
            }

            return TryParseInt64Number(value, styles, info, out result);
        }

        private static ParsingStatus TryParseInt64Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
        {
            result = 0;
            var number = new NumberBuffer(NumberBufferKind.Integer, stackalloc byte[Int64NumberBufferLength]);

            if (!TryStringToNumber(value, styles, ref number, info))
                return ParsingStatus.Failed;

            return TryNumberToInt64(in number, out result) ? ParsingStatus.OK : ParsingStatus.Overflow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParsingStatus TryParseUInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
        {
            if ((styles & ~NumberStyles.Integer) == 0)
            {
                // Optimized path for the common case of anything that's allowed for integer style.
                return TryParseUInt32IntegerStyle(value, styles, info, out result);
            }

            return (styles & NumberStyles.AllowHexSpecifier) != 0 
                ? TryParseUInt32HexNumberStyle(value, styles, out result)
                : TryParseUInt32Number(value, styles, info, out result);
        }

        private static ParsingStatus TryParseUInt32Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
        {
            result = 0;
            var number = new NumberBuffer(NumberBufferKind.Integer, stackalloc byte[UInt32NumberBufferLength]);

            return TryStringToNumber(value, styles, ref number, info)
                ? TryNumberToUInt32(in number, out result) ? ParsingStatus.OK : ParsingStatus.Overflow
                : ParsingStatus.Failed;
        }

        /// <summary>Parses uint limited to styles that make up NumberStyles.Integer.</summary>
        internal static ParsingStatus TryParseUInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
        {
            Debug.Assert((styles & ~NumberStyles.Integer) == 0, "Only handles subsets of Integer format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            // Parse leading sign.
            var overflow = false;
            if ((styles & NumberStyles.AllowLeadingSign) != 0)
            {
                if (info.HasInvariantNumberSigns())
                {
                    if (num == '+')
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (num == '-')
                    {
                        overflow = true;
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
                else
                {
                    value = value.Slice(index);
                    index = 0;
                    string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
                    if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
                    {
                        index += positiveSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
                    {
                        overflow = true;
                        index += negativeSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
            }

            var answer = 0;

            if (IsDigit(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!IsDigit(num))
                        goto HasTrailingCharsZero;
                }

                // Parse most digits, up to the potential for overflow, which can't happen until after 9 digits.
                answer = num - '0'; // first digit
                index++;
                for (var i = 0; i < 8; i++) // next 8 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEndButPotentialOverflow;
                    num = value[index];
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                    index++;
                    answer = 10 * answer + num - '0';
                }

                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
                num = value[index];
                if (!IsDigit(num))
                    goto HasTrailingChars;
                index++;
                // Potential overflow now processing the 10th digit.
                overflow |= (uint)answer > uint.MaxValue / 10 || (uint)answer == uint.MaxValue / 10 && num > '5';
                answer = answer * 10 + num - '0';
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility.
                num = value[index];
                while (IsDigit(num))
                {
                    overflow = true;
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                }
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
            {
                goto OverflowExit;
            }
            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = (uint)answer;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingCharsZero:
            overflow = false;
        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        /// <summary>Parses uint limited to styles that make up NumberStyles.HexNumber.</summary>
        private static ParsingStatus TryParseUInt32HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, out uint result)
        {
            Debug.Assert((styles & ~NumberStyles.HexNumber) == 0, "Only handles subsets of HexNumber format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            var overflow = false;
            uint answer = 0;

            if (HexConverter.IsHexChar(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!HexConverter.IsHexChar(num))
                        goto HasTrailingChars;
                }

                // Parse up through 8 digits, as no overflow is possible
                answer = (uint)HexConverter.FromChar(num); // first digit
                index++;
                for (var i = 0; i < 7; i++) // next 7 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEnd;
                    num = value[index];

                    var numValue = (uint)HexConverter.FromChar(num);
                    if (numValue == 0xFF)
                        goto HasTrailingChars;
                    index++;
                    answer = 16 * answer + numValue;
                }

                // If there's another digit, it's an overflow.
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEnd;
                num = value[index];
                if (!HexConverter.IsHexChar(num))
                    goto HasTrailingChars;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility. Read through any remaining digits.
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                } while (HexConverter.IsHexChar(num));
                overflow = true;
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
            {
                goto OverflowExit;
            }

            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = answer;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParsingStatus TryParseUInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
        {
            if ((styles & ~NumberStyles.Integer) == 0)
            {
                // Optimized path for the common case of anything that's allowed for integer style.
                return TryParseUInt64IntegerStyle(value, styles, info, out result);
            }

            return (styles & NumberStyles.AllowHexSpecifier) != 0
                ? TryParseUInt64HexNumberStyle(value, styles, out result) 
                : TryParseUInt64Number(value, styles, info, out result);
        }

        private static ParsingStatus TryParseUInt64Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
        {
            result = 0;
            
            var number = new NumberBuffer(NumberBufferKind.Integer, stackalloc byte[UInt64NumberBufferLength]);

            if (!TryStringToNumber(value, styles, ref number, info))
                return ParsingStatus.Failed;

            return TryNumberToUInt64(in number, out result) ? ParsingStatus.OK : ParsingStatus.Overflow;
        }

        /// <summary>Parses ulong limited to styles that make up NumberStyles.Integer.</summary>
        internal static ParsingStatus TryParseUInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
        {
            Debug.Assert((styles & ~NumberStyles.Integer) == 0, "Only handles subsets of Integer format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            // Parse leading sign.
            var overflow = false;
            if ((styles & NumberStyles.AllowLeadingSign) != 0)
            {
                if (info.HasInvariantNumberSigns())
                {
                    if (num == '+')
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (num == '-')
                    {
                        overflow = true;
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
                else
                {
                    value = value.Slice(index);
                    index = 0;
                    string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
                    if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
                    {
                        index += positiveSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                    else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
                    {
                        overflow = true;
                        index += negativeSign.Length;
                        if ((uint)index >= (uint)value.Length)
                            goto FalseExit;
                        num = value[index];
                    }
                }
            }

            long answer = 0;

            if (IsDigit(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!IsDigit(num))
                        goto HasTrailingCharsZero;
                }

                // Parse most digits, up to the potential for overflow, which can't happen until after 19 digits.
                answer = num - '0'; // first digit
                index++;
                for (var i = 0; i < 18; i++) // next 18 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEndButPotentialOverflow;
                    num = value[index];
                    if (!IsDigit(num))
                        goto HasTrailingChars;
                    index++;
                    answer = 10 * answer + num - '0';
                }

                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
                num = value[index];
                if (!IsDigit(num))
                    goto HasTrailingChars;
                index++;
                // Potential overflow now processing the 20th digit.
                overflow |= (ulong)answer > ulong.MaxValue / 10 || (ulong)answer == ulong.MaxValue / 10 && num > '5';
                answer = answer * 10 + num - '0';
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility.
                num = value[index];
                while (IsDigit(num))
                {
                    overflow = true;
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                }
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
            {
                goto OverflowExit;
            }
            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = (ulong)answer;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingCharsZero:
            overflow = false;
        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        /// <summary>Parses ulong limited to styles that make up NumberStyles.HexNumber.</summary>
        private static ParsingStatus TryParseUInt64HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, out ulong result)
        {
            Debug.Assert((styles & ~NumberStyles.HexNumber) == 0, "Only handles subsets of HexNumber format");

            if (value.IsEmpty)
                goto FalseExit;

            var index = 0;
            int num = value[0];

            // Skip past any whitespace at the beginning.
            if ((styles & NumberStyles.AllowLeadingWhite) != 0 && IsWhite(num))
            {
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto FalseExit;
                    num = value[index];
                }
                while (IsWhite(num));
            }

            var overflow = false;
            ulong answer = 0;

            if (HexConverter.IsHexChar(num))
            {
                // Skip past leading zeros.
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if ((uint)index >= (uint)value.Length)
                            goto DoneAtEnd;
                        num = value[index];
                    } while (num == '0');
                    if (!HexConverter.IsHexChar(num))
                        goto HasTrailingChars;
                }

                // Parse up through 16 digits, as no overflow is possible
                answer = (uint)HexConverter.FromChar(num); // first digit
                index++;
                for (var i = 0; i < 15; i++) // next 15 digits can't overflow
                {
                    if ((uint)index >= (uint)value.Length)
                        goto DoneAtEnd;
                    num = value[index];

                    var numValue = (uint)HexConverter.FromChar(num);
                    if (numValue == 0xFF)
                        goto HasTrailingChars;
                    index++;
                    answer = 16 * answer + numValue;
                }

                // If there's another digit, it's an overflow.
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEnd;
                num = value[index];
                if (!HexConverter.IsHexChar(num))
                    goto HasTrailingChars;

                // At this point, we're either overflowing or hitting a formatting error.
                // Format errors take precedence for compatibility. Read through any remaining digits.
                do
                {
                    index++;
                    if ((uint)index >= (uint)value.Length)
                        goto OverflowExit;
                    num = value[index];
                } while (HexConverter.IsHexChar(num));
                overflow = true;
                goto HasTrailingChars;
            }
            goto FalseExit;

        DoneAtEndButPotentialOverflow:
            if (overflow)
            {
                goto OverflowExit;
            }
            // ReSharper disable once BadChildStatementIndent
        DoneAtEnd:
            result = answer;
            var status = ParsingStatus.OK;
        Exit:
            return status;

        FalseExit: // parsing failed
            result = 0;
            status = ParsingStatus.Failed;
            goto Exit;
        OverflowExit:
            result = 0;
            status = ParsingStatus.Overflow;
            goto Exit;

        HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
            // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
            if (IsWhite(num))
            {
                if ((styles & NumberStyles.AllowTrailingWhite) == 0)
                    goto FalseExit;
                for (index++; index < value.Length; index++)
                {
                    if (!IsWhite(value[index]))
                        break;
                }
                if ((uint)index >= (uint)value.Length)
                    goto DoneAtEndButPotentialOverflow;
            }

            if (!TrailingZeros(value, index))
                goto FalseExit;

            goto DoneAtEndButPotentialOverflow;
        }

        internal static bool TryNumberToDecimal(in NumberBuffer number, ref decimal value)
        {
            number.CheckConsistency();

            ref readonly var p = ref number.GetRef();
            var e = number.Scale;
            var sign = number.IsNegative;
            uint c = p;
            if (c == 0)
            {
                // To avoid risking an app-compat issue with pre 4.5 (where some app was illegally using Reflection to examine the internal scale bits), we'll only force
                // the scale to 0 if the scale was previously positive (previously, such cases were unparsable to a bug.)
                value = new decimal(0, 0, 0, sign, (byte)MathP.Clamp(-e, 0, 28));
                return true;
            }

            if (e > DecimalPrecision)
                return false;

            ulong low64 = 0;
            while (e > -28)
            {
                e--;
                low64 *= 10;
                low64 += c - '0';
                //c = *++p;
                c = p = ref Inc(in p);
                if (low64 >= ulong.MaxValue / 10)
                    break;
                if (c != 0) 
                    continue;
                while (e > 0)
                {
                    e--;
                    low64 *= 10;
                    if (low64 >= ulong.MaxValue / 10)
                        break;
                }
                break;
            }

            uint high = 0;
            while ((e > 0 || c != 0 && e > -28) &&
                   (high < uint.MaxValue / 10 || high == uint.MaxValue / 10
                    && (low64 < 0x99999999_99999999 || low64 == 0x99999999_99999999 && c <= '5')))
            {
                // multiply by 10
                var tmpLow = (uint) low64 * 10UL;
                var tmp64 = (uint) (low64 >> 32) * 10UL + (tmpLow >> 32);
                low64 = (uint) tmpLow       + (tmp64 << 32);
                high = (uint) (tmp64 >> 32) + high * 10;

                if (c != 0)
                {
                    c -= '0';
                    low64 += c;
                    if (low64 < c)
                        high++;
                    //c = *++p;
                    c = p = ref Inc(in p);
                }

                e--;
            }

            if (c >= '5')
            {
                if (c == '5' && (low64 & 1) == 0)
                {
                    //c = *++p;
                    c = p = ref Inc(in p);

                    var hasZeroTail = !number.HasNonZeroTail;

                    // We might still have some additional digits, in which case they need
                    // to be considered as part of hasZeroTail. Some examples of this are:
                    //  * 3.0500000000000000000001e-27
                    //  * 3.05000000000000000000001e-27
                    // In these cases, we will have processed 3 and 0, and ended on 5. The
                    // buffer, however, will still contain a number of trailing zeros and
                    // a trailing non-zero number.

                    while (c != 0 && hasZeroTail)
                    {
                        hasZeroTail &= c == '0';
                        //c = *++p;
                        c = p = ref Inc(in p);
                    }

                    // We should either be at the end of the stream or have a non-zero tail
                    Debug.Assert(c == 0 || !hasZeroTail);

                    if (hasZeroTail)
                    {
                        // When the next digit is 5, the number is even, and all following
                        // digits are zero we don't need to round.
                        goto NoRounding;
                    }
                }

                if (++low64 == 0 && ++high == 0)
                {
                    low64 = 0x99999999_9999999A;
                    high = uint.MaxValue / 10;
                    e++;
                }
            }
            // ReSharper disable once BadChildStatementIndent
        NoRounding:

            switch (e)
            {
                case > 0:
                    return false;
                case <= -DecimalPrecision:
                    // Parsing a large scale zero can give you more precision than fits in the decimal.
                    // This should only happen for actual zeros or very small numbers that round to zero.
                    value = new decimal(0, 0, 0, sign, DecimalPrecision - 1);
                    break;
                default:
                    value = new decimal((int)low64, (int)(low64 >> 32), (int)high, sign, (byte)-e);
                    break;
            }

            return true;
        }

        internal static ParsingStatus TryParseDecimal(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out decimal result)
        {
            var number = new NumberBuffer(NumberBufferKind.Decimal, stackalloc byte[DecimalNumberBufferLength]);

            result = 0;

            if (!TryStringToNumber(value, styles, ref number, info))
                return ParsingStatus.Failed;

            return TryNumberToDecimal(in number, ref result) ? ParsingStatus.OK : ParsingStatus.Overflow;
        }

        internal static bool TryParseDouble(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out double result)
        {
            var number = new NumberBuffer(NumberBufferKind.FloatingPoint, stackalloc byte[DoubleNumberBufferLength]);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.

                if (valueTrim.Equals(info.PositiveInfinitySymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    result = double.PositiveInfinity;
                else if (valueTrim.Equals(info.NegativeInfinitySymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    result = double.NegativeInfinity;
                else if (valueTrim.Equals(info.NaNSymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    result = double.NaN;
                else if (valueTrim.StartsWith(info.PositiveSign.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (valueTrim.Equals(info.PositiveInfinitySymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        result = double.PositiveInfinity;
                    else if (valueTrim.Equals(info.NaNSymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        result = double.NaN;
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                         valueTrim.Slice(info.NegativeSign.Length).Equals(info.NaNSymbol.AsSpan(),
                             StringComparison.OrdinalIgnoreCase))
                    result = double.NaN;
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
                result = NumberToDouble(in number);

            return true;
        }

        //internal static unsafe bool TryParseHalf(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out Half result)
        //{
        //    byte* pDigits = stackalloc byte[HalfNumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, HalfNumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        ReadOnlySpan<char> valueTrim = value.Trim();

        //        // This code would be simpler if we only had the concept of `InfinitySymbol`, but
        //        // we don't so we'll check the existing cases first and then handle `PositiveSign` +
        //        // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
        //        //
        //        // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
        //        // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
        //        // that case so that we don't start parsing things like `++infini`.

        //        if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
        //        {
        //            result = Half.PositiveInfinity;
        //        }
        //        else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
        //        {
        //            result = Half.NegativeInfinity;
        //        }
        //        else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //        {
        //            result = Half.NaN;
        //        }
        //        else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
        //        {
        //            valueTrim = valueTrim.Slice(info.PositiveSign.Length);

        //            if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
        //            {
        //                result = Half.PositiveInfinity;
        //            }
        //            else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //            {
        //                result = Half.NaN;
        //            }
        //            else
        //            {
        //                result = (Half)0;
        //                return false;
        //            }
        //        }
        //        else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
        //                 !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
        //                 valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //        {
        //            result = Half.NaN;
        //        }
        //        else
        //        {
        //            result = (Half)0;
        //            return false; // We really failed
        //        }
        //    }
        //    else
        //    {
        //        result = NumberToHalf(ref number);
        //    }

        //    return true;
        //}

        internal static bool TryParseSingle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out float result)
        {
            var number = new NumberBuffer(NumberBufferKind.FloatingPoint, stackalloc byte[SingleNumberBufferLength]);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
                //
                // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
                // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
                // that case so that we don't start parsing things like `++infini`.

                if (valueTrim.Equals(info.PositiveInfinitySymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    result = float.PositiveInfinity;
                else if (valueTrim.Equals(info.NegativeInfinitySymbol.AsSpan(), StringComparison.Ordinal))
                    result = float.NegativeInfinity;
                else if (valueTrim.Equals(info.NaNSymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                    result = float.NaN;
                else if (valueTrim.StartsWith(info.PositiveSign.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase)
                     && valueTrim.Equals(info.PositiveInfinitySymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        result = float.PositiveInfinity;
                    else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase)
                          && valueTrim.Equals(info.NaNSymbol.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        result = float.NaN;
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                         !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         valueTrim.Slice(info.NegativeSign.Length).Equals(info.NaNSymbol.AsSpan(),
                                                                          StringComparison.OrdinalIgnoreCase))
                    result = float.NaN;
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
                result = NumberToSingle(ref number);

            return true;
        }

        internal static bool TryStringToNumber(ReadOnlySpan<char> value, NumberStyles styles, ref NumberBuffer number, NumberFormatInfo info)
        {
            //fixed (char* stringPointer = &MemoryMarshal.GetReference(value))
            //{
            //    var p = stringPointer;
            //    if (!TryParseNumber(ref p, p + value.Length, styles, ref number, info!)
            //        || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
            //    {
            //        number.CheckConsistency();
            //        return false;
            //    }
            //}
            if (!TryParseNumber(value, styles, ref number, info, out var nParsed) ||
                nParsed < value.Length && !TrailingZeros(value, nParsed))
            {
                number.CheckConsistency();
                return false;
            }

            number.CheckConsistency();
            return true;
        }

        private static bool TrailingZeros(ReadOnlySpan<char> value, int index)
        {
            // For compatibility, we need to allow trailing zeros at the end of a number string
            for (var i = index; (uint)i < (uint)value.Length; i++)
                if (value[i] != '\0')
                    return false;

            return true;
        }

        private static bool IsSpaceReplacingChar(char c) => c == '\u00a0' || c == '\u202f';

        private static ReadOnlySpan<char> MatchChars(ReadOnlySpan<char> p, ReadOnlySpan<char> str)
        {
                Debug.Assert(!str.IsEmpty);
                if(str[0] == '\0') 
                    return ReadOnlySpan<char>.Empty;
                // We only hurt the failure case
                // This fix is for French or Kazakh cultures. Since a user cannot type 0xA0 or 0x202F as a
                // space character we use 0x20 space character instead to mean the same.

                var pIndex = 0;
                var sIndex = 0;
                while (true)
                {
                    var cp = pIndex < p.Length ? p[pIndex] : '\0';
                    if (cp != str[sIndex] && !(IsSpaceReplacingChar(str[sIndex]) && cp == '\u0020'))
                        break;

                    pIndex++;
                    sIndex++;
                    if (sIndex >= str.Length || str[sIndex] == '\0')
                        return pIndex - 1 < p.Length ? p.Slice(pIndex - 1) : ReadOnlySpan<char>.Empty;
                }

                return ReadOnlySpan<char>.Empty;
        }

        // Ternary op is a workaround for https://github.com/dotnet/runtime/issues/4207
        // ReSharper disable once RedundantTernaryExpression
#pragma warning disable IDE0075 // Simplify conditional expression
        private static bool IsWhite(int ch) => ch == 0x20 || (uint)(ch - 0x09) <= 0x0D - 0x09 ? true : false;
#pragma warning restore IDE0075 // Simplify conditional expression

        private static bool IsDigit(int ch) => (uint)ch - '0' <= 9;

        internal enum ParsingStatus
        {
            // ReSharper disable once InconsistentNaming
            OK,
            Failed,
            Overflow
        }

        internal static double NumberToDouble(in NumberBuffer number)
        {
            // This is for debug purposes
            number.CheckConsistency();

            var result = number switch
            {
                {DigitsCount: 0} or {Scale: < DoubleMinExponent} => 0,
                {Scale: > DoubleMaxExponent} => double.PositiveInfinity,
                _ => global::System.BitConverter.Int64BitsToDouble(
                    (long) NumberToFloatingPointBits(in number, in FloatingPointInfo.Double))
            };

            return number.IsNegative ? -result : result;
        }

        //internal static Half NumberToHalf(ref NumberBuffer number)
        //{
        //    number.CheckConsistency();
        //    Half result;

        //    if ((number.DigitsCount == 0) || (number.Scale < HalfMinExponent))
        //    {
        //        result = default;
        //    }
        //    else if (number.Scale > HalfMaxExponent)
        //    {
        //        result = Half.PositiveInfinity;
        //    }
        //    else
        //    {
        //        ushort bits = (ushort)(NumberToFloatingPointBits(ref number, in FloatingPointInfo.Half));
        //        result = new Half(bits);
        //    }

        //    return number.IsNegative ? Half.Negate(result) : result;
        //}

        internal static float NumberToSingle(ref NumberBuffer number)
        {
            // This is for debug purposes
            number.CheckConsistency();

            var result = number switch
            {
                {DigitsCount: 0} or {Scale: < SingleMinExponent} => 0,
                {Scale: > SingleMaxExponent} => float.PositiveInfinity,
                _ => BitConverter.Int32BitsToSingle(
                    (int) (uint) NumberToFloatingPointBits(in number, in FloatingPointInfo.Single))
            };
            
            return number.IsNegative ? -result : result;
        }
    }
}

#endif