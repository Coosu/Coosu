#if NETSTANDARD1_3_OR_GREATER
using System;
#endif

namespace Coosu.Shared
{
    public static class EmptyArray<T>
    {
        public static readonly T[] Value =
#if NETSTANDARD1_3_OR_GREATER
                Array.Empty<T>()
#else
                new T[0]
#endif
            ;
    }
}