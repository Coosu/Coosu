using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace RefTools
{
    public static class Ref
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo As<TFrom, TTo>(in TFrom source)
            where TFrom : unmanaged
            where TTo : unmanaged =>
            ref Unsafe.As<TFrom, TTo>(ref AsRef(in source));
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Inc<T>(in T @this) where T : unmanaged =>
            ref Unsafe.Add(ref AsRef(in @this), 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Dec<T>(in T @this) where T : unmanaged =>
            ref Subtract(ref AsRef(in @this), 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T IncMut<T>(ref T @this) where T : unmanaged => ref Unsafe.Add(ref @this, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DecMut<T>(ref T @this) where T : unmanaged => ref Subtract(ref @this, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint ItemOffset<T>(in T origin, in T target) where T : unmanaged =>
            (nint)((long)ByteOffset(in origin, in target) / SizeOf<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint ItemOffset(in byte origin, in byte target) =>
            ByteOffset(in origin, in target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint ByteOffset<T>(in T origin, in T target) where T : unmanaged =>
            Unsafe.ByteOffset(ref AsRef(in origin), ref AsRef(in target));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Add<T>(in T source, int elementOffset) where T : unmanaged =>
            ref Unsafe.Add(ref AsRef(in source), elementOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Sub<T>(in T source, int elementOffset) where T : unmanaged =>
            ref Subtract(ref AsRef(in source), elementOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddMut<T>(ref T source, int elementOffset) where T : unmanaged =>
            ref Unsafe.Add(ref source, elementOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T SubMut<T>(ref T source, int elementOffset) where T : unmanaged =>
            ref Subtract(ref source, elementOffset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSame<T>(in T left, in T right) where T : unmanaged =>
            Unsafe.AreSame(ref AsRef(in left), ref AsRef(in right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreater<T>(in T left, in T right) where T : unmanaged =>
            IsAddressGreaterThan(ref AsRef(in left), ref AsRef(in right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLess<T>(in T left, in T right) where T : unmanaged =>
            IsAddressLessThan(ref AsRef(in left), ref AsRef(in right));

        public static int Compare<T>(in T left, in T right) where T : unmanaged
        {
            if (IsLess(in left, in right))
                return -1;
            return IsGreater(in left, in right) ? 1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T NullRef<T>() where T : unmanaged => ref Unsafe.NullRef<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(in T @ref) where T : unmanaged => IsNullRef(ref AsRef(in @ref));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint AddrOf<T>(in T @ref) where T : unmanaged
            => ByteOffset(in NullRef<T>(), in @ref);

    }
}