#if NETSTANDARD2_0

using System.Runtime.InteropServices;

namespace Backports.System
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Half
    {
        internal const uint FloatSignMask = 0x8000_0000;
        internal const int FloatSignShift = 31;
        internal const uint FloatExponentMask = 0x7F80_0000;
        internal const int FloatExponentShift = 23;
        internal const uint FloatSignificandMask = 0x007F_FFFF;


        private const ushort ExponentMask = 0x7C00;
        private const ushort SignShift = 15;
        private const ushort ExponentShift = 10;
        private const ushort PositiveInfinityBits = 0x7C00;
        private const ushort NegativeInfinityBits = 0xFC00;

        private readonly ushort _value;

        internal Half(ushort value) => _value = value;

        private Half(bool sign, ushort exp, ushort sig)
            => _value = (ushort)(((sign ? 1 : 0) << SignShift) + (exp << ExponentShift) + sig);

        private static Half CreateHalfNaN(bool sign, ulong significand)
        {
            const uint naNBits = ExponentMask | 0x200; // Most significant significand bit

            var signInt = (sign ? 1U : 0U) << SignShift;
            var sigInt = (uint)(significand >> 54);

            return BitConverter.Int16BitsToHalf((short)(signInt | naNBits | sigInt));
        }

        private static ushort RoundPackToHalf(bool sign, short exp, ushort sig)
        {
            const int roundIncrement = 0x8; // Depends on rounding mode but it's always towards closest / ties to even
            var roundBits = sig & 0xF;

            if ((uint)exp >= 0x1D)
            {
                if (exp < 0)
                {
                    sig = (ushort)ShiftRightJam(sig, -exp);
                    exp = 0;
                }
                else if (exp > 0x1D || sig + roundIncrement >= 0x8000) // Overflow
                {
                    return sign ? NegativeInfinityBits : PositiveInfinityBits;
                }
            }

            sig = (ushort)((sig + roundIncrement) >> 4);
            sig &= (ushort)~(((roundBits ^ 8) != 0 ? 0 : 1) & 1);

            if (sig == 0)
            {
                exp = 0;
            }

            return new Half(sign, (ushort)exp, sig)._value;
        }

        // If any bits are lost by shifting, "jam" them into the LSB.
        // if dist > bit count, Will be 1 or 0 depending on i
        // (unlike bitwise operators that masks the lower 5 bits)
        private static uint ShiftRightJam(uint i, int dist)
            => dist < 31 ? (i >> dist) | (i << (-dist & 31) != 0 ? 1U : 0U) : i != 0 ? 1U : 0U;

        public static explicit operator Half(float value)
        {
            const int singleMaxExponent = 0xFF;

            var floatInt = (uint)BitConverter.SingleToInt32Bits(value);
            var sign = (floatInt & FloatSignMask) >> FloatSignShift != 0;
            var exp = (int)(floatInt & FloatExponentMask) >> FloatExponentShift;
            var sig = floatInt & FloatSignificandMask;

            if (exp == singleMaxExponent)
            {
                if (sig != 0) // NaN
                {
                    return CreateHalfNaN(sign, (ulong)sig << 41); // Shift the significand bits to the left end
                }

                return sign ? new Half(NegativeInfinityBits) : new Half(PositiveInfinityBits);
            }

            var sigHalf = sig >> 9 | ((sig & 0x1FFU) != 0 ? 1U : 0U); // RightShiftJam

            return (exp | (int)sigHalf) == 0
                ? new Half(sign, 0, 0)
                : new Half(RoundPackToHalf(sign, (short)(exp - 0x71), (ushort)(sigHalf | 0x4000)));
        }
    }
}

#endif