#if NETSTANDARD2_0

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Backports.System
{
    internal static class BitOperations
    {
        private static ReadOnlySpan<byte> Log2DeBruijn => new byte[]
        {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        };

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since Log(0) is undefined.
        /// Does not directly use any hardware intrinsics, nor does it incur branching.
        /// </summary>
        /// <param name="value">The value.</param>
        private static int Log2SoftwareFallback(uint value)
        {
            // No AggressiveInlining due to large method size
            // Has conventional contract 0->0 (Log(0) is undefined)

            // Fill trailing zeros with ones, eg 00010010 becomes 00011111
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

            // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
            return Unsafe.AddByteOffset(
                // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
                ref MemoryMarshal.GetReference(Log2DeBruijn),
                // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
        }



        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(ulong value)
        {
            //if (Lzcnt.X64.IsSupported)
            //{
            //    // LZCNT contract is 0->64
            //    return (int)Lzcnt.X64.LeadingZeroCount(value);
            //}

            //if (ArmBase.Arm64.IsSupported)
            //{
            //    return ArmBase.Arm64.LeadingZeroCount(value);
            //}

            //if (X86Base.X64.IsSupported)
            //{
            //    // BSR contract is 0->undefined
            //    return value == 0 ? 64 : 63 ^ (int)X86Base.X64.BitScanReverse(value);
            //}

            var hi = (uint)(value >> 32);

            if (hi == 0)
            {
                return 32 + LeadingZeroCount((uint)value);
            }

            return LeadingZeroCount(hi);
        }

        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(uint value)
        {
            //if (Lzcnt.IsSupported)
            //{
            //    // LZCNT contract is 0->32
            //    return (int)Lzcnt.LeadingZeroCount(value);
            //}

            //if (ArmBase.IsSupported)
            //{
            //    return ArmBase.LeadingZeroCount(value);
            //}

            // Unguarded fallback contract is 0->31, BSR contract is 0->undefined
            if (value == 0)
            {
                return 32;
            }

            //if (X86Base.IsSupported)
            //{
            //    // LZCNT returns index starting from MSB, whereas BSR gives the index from LSB.
            //    // 31 ^ BSR here is equivalent to 31 - BSR since the BSR result is always between 0 and 31.
            //    // This saves an instruction, as subtraction from constant requires either MOV/SUB or NEG/ADD.
            //    return 31 ^ (int)X86Base.BitScanReverse(value);
            //}

            return 31 ^ Log2SoftwareFallback(value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(uint value)
        {
            // The 0->0 contract is fulfilled by setting the LSB to 1.
            // Log(1) is 0, and setting the LSB for values > 1 does not change the log2 result.
            value |= 1;

            // value    lzcnt   actual  expected
            // ..0001   31      31-31    0
            // ..0010   30      31-30    1
            // 0010..    2      31-2    29
            // 0100..    1      31-1    30
            // 1000..    0      31-0    31

            // Fallback contract is 0->0
            return Log2SoftwareFallback(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(ulong value)
        {
            value |= 1;

            var hi = (uint)(value >> 32);

            if (hi == 0)
                return Log2((uint) value);

            return 32 + Log2(hi);
        }
    }
}

#endif