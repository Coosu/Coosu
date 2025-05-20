#if NETSTANDARD2_0
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Backports
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BigIntBuff128
    {
        
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        internal struct Byte64
        {
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // 0
            [FieldOffset(sizeof(ulong) * 0)]
            private ulong field1;
            // 8
            [FieldOffset(sizeof(ulong) * 1)]
            private ulong field2;
            // 16
            [FieldOffset(sizeof(ulong) * 2)]
            private ulong field3;
            // 24
            [FieldOffset(sizeof(ulong) * 3)]
            private ulong field4;
            // 32
            [FieldOffset(sizeof(ulong) * 4)]
            private ulong field5;
            // 40
            [FieldOffset(sizeof(ulong) * 5)]
            private ulong field6;
            // 48
            [FieldOffset(sizeof(ulong) * 6)]
            private ulong field7;
            // 56
            [FieldOffset(sizeof(ulong) * 7)]
            private ulong field8;
            // 64

            // ReSharper restore FieldCanBeMadeReadOnly.Local
            [FieldOffset(0)]
            internal uint zeroUint;

            [FieldOffset(0)]
            internal byte zeroByte;

        }

        // 0 : 0
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private Byte64 field1;
        // 64 : 16
        private Byte64 field2;
        // 128 : 32
        private Byte64 field3;
        // 196 : 48
        private Byte64 field4;
        // 256 : 64
        private Byte64 field5;
        // 320 : 80
        private Byte64 field6;
        // 384 : 96
        private Byte64 field7;
        // 448 : 112
        private uint field81;
        // 452 : 113
        private uint field82;
        // 456 : 114
        private uint field83;
        // 460 : 115
        //private Byte64 field8;
        // ReSharper restore FieldCanBeMadeReadOnly.Local


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref uint GetU32Ref(ref BigIntBuff128 buff) => ref buff.field1.zeroUint;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static ref readonly uint GetU32RefRO(in BigIntBuff128 buff) => ref buff.field1.zeroUint;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref uint UnsafeAtU32(ref BigIntBuff128 buff, int offset) =>
            ref Unsafe.Add(ref buff.field1.zeroUint, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref uint UnsafeAtU32(ref BigIntBuff128 buff, uint offset)
        {
            // Assuming this can be optimized by the JIT
            // Unsafe.Add handles either int or IntPtr offsets
            // So, in 32-bit process IntPtr equals in size to int, and cannot contain uint values
            // For 64-bit processes, uint offset can be safely cast to IntPtr
            if (Environment.Is64BitProcess)
                return ref UnsafeAtU32(ref buff, (IntPtr) offset);

            // In 32-bit process, if offset is within int value range, it is safe to cast
            if (offset <= int.MaxValue)
                return ref UnsafeAtU32(ref buff, (int) offset);

            // Otherwise, perform one addition by int.MaxValue followed by the difference between offset and int.MaxValue,
            // which definitely falls within int value range
            ref var p = ref Unsafe.Add(ref buff.field1.zeroUint, int.MaxValue);
            return ref Unsafe.Add(ref p, (int)(offset - int.MaxValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref uint UnsafeAtU32(ref BigIntBuff128 buff, IntPtr offset) =>
            ref Unsafe.Add(ref buff.field1.zeroUint, offset);


        // ReSharper disable once InconsistentNaming
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly uint UnsafeAtU32RO(in BigIntBuff128 buff, int offset) => ref UnsafeAtU32(ref Unsafe.AsRef(in buff), offset);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static ref readonly uint UnsafeAtU32RO(in BigIntBuff128 buff, uint offset) => ref UnsafeAtU32(ref Unsafe.AsRef(in buff), offset);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static ref readonly uint UnsafeAtU32RO(in BigIntBuff128 buff, IntPtr offset) => ref UnsafeAtU32(ref Unsafe.AsRef(in buff), offset);





    }
}
#endif