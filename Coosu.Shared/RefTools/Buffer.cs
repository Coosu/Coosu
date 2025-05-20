using System.Runtime.CompilerServices;

namespace RefTools
{
    public static class Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ZeroMemory(ref byte ptr, uint len) =>
            Unsafe.InitBlockUnaligned(ref ptr, 0, len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memcpy(ref byte dest, in byte src, int len) =>
            // Using AsRef to bypass limitations
            Unsafe.CopyBlockUnaligned(ref dest, ref Unsafe.AsRef(in src), (uint) len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memcpy<T>(ref T dest, in T src, int len) where T : unmanaged =>
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<T, byte>(ref dest), ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in src)),
                (uint) (Unsafe.SizeOf<T>() * len)
            );
    }

}