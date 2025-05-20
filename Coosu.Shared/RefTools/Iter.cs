using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RefTools
{
    public static class Iter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Begin<T>(this ReadOnlySpan<T> @this) where T : unmanaged
            => ref MemoryMarshal.GetReference(@this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T End<T>(this ReadOnlySpan<T> @this) where T : unmanaged
        // ReSharper disable once UseIndexFromEndExpression
            => ref Ref.Add(in @this.Begin(), @this.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T BeginMut<T>(this Span<T> @this) where T : unmanaged
            => ref MemoryMarshal.GetReference(@this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T EndMut<T>(this Span<T> @this) where T : unmanaged
        // ReSharper disable once UseIndexFromEndExpression
            => ref Ref.AddMut(ref @this.BeginMut(), @this.Length);
    }
}