#if NETSTANDARD2_0

using System.Runtime.CompilerServices;

namespace Backports.System
{
    internal static class BitConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Int32BitsToSingle(int value) => Unsafe.As<int, float>(ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SingleToInt32Bits(float value) => Unsafe.As<float, int>(ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Int16BitsToHalf(short value) => Unsafe.As<short, Half>(ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short HalfToInt16Bits(Half value) => Unsafe.As<Half, short>(ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long DoubleToInt64Bits(double value) => Unsafe.As<double, long>(ref value);
    }
}

#endif