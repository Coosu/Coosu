using System;

namespace Coosu.Beatmap
{
    public class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException(int version) : base($"尚不支持 osu file format v[{version}]。")
        {
        }
    }
}