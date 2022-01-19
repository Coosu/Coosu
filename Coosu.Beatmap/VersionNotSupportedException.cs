using System;

namespace Coosu.Beatmap
{
    public sealed class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException(int version)
            : base($"The specific osu file format version is not supported: {version}。")
        {
        }
    }
}