#if NETSTANDARD2_0

using System;
using System.Runtime.InteropServices;

namespace Backports
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct DecimalRep
    {
        // Scale mask for the flags field. This byte in the flags field contains
        // the power of 10 to divide the Decimal value by. The scale byte must
        // contain a value between 0 and 28 inclusive.
        // ReSharper disable once UnusedMember.Local
        private const int ScaleMask = 0x00FF0000;

        // Number of bits scale is shifted by.
        private const int ScaleShift = 16;
        private const uint TenToPowerNine = 1000000000;

        [FieldOffset(0)]
        public int Flags;
        [FieldOffset(sizeof(int))]
        public int Hi;
        [FieldOffset(2 * sizeof(int))]
        public int Lo;
        [FieldOffset(3 * sizeof(int))]
        public int Mid;

        [FieldOffset(0)]
        public uint UFlags;
        [FieldOffset(sizeof(int))]
        public uint UHi;
        [FieldOffset(2 * sizeof(int))]
        public uint ULo;
        [FieldOffset(3 * sizeof(int))]
        public uint UMid;



        internal readonly int Scale => (byte) (Flags >> ScaleShift);

        public readonly void Deconstruct(out int lo, out int mid, out int hi, out int flags) =>
            (flags, hi, lo, mid) = (Flags, Hi, Lo, Mid);

        public override readonly string ToString() => (Lo, Mid, Hi, Flags).ToString();

        public readonly bool TryGetBits(Span<int> destination, out int valuesWritten)
        {
            if (destination.Length < 4)
            {
                valuesWritten = 0;
                return false;
            }

            destination[0] = Lo;
            destination[1] = Mid;
            destination[2] = Hi;
            destination[3] = Flags;
            valuesWritten = 4;
            return true;
        }

        public readonly int GetBits(Span<int> destination)
        {
            if (destination.Length < 4)
                throw new ArgumentException("Target buffer is too short", nameof(destination));
            destination[0] = Lo;
            destination[1] = Mid;
            destination[2] = Hi;
            destination[3] = Flags;
            return 4;
        }

        internal static uint DecDivMod1E9(ref DecimalRep value)
        {
            var high64 = ((ulong)value.UHi << 32) + value.UMid;
            var div64 = high64 / TenToPowerNine;
            value.UHi = (uint)(div64 >> 32);
            value.UMid = (uint)div64;

            var num = ((high64 - (uint)div64 * TenToPowerNine) << 32) + value.ULo;
            var div = (uint)(num / TenToPowerNine);
            value.ULo = div;
            return (uint)num - div * TenToPowerNine;
        }

        public static explicit operator decimal(DecimalRep rep) => rep.BitsRepToDec();
        public static explicit operator DecimalRep(decimal d) => d.AsBitsRep();
    }
}
#endif