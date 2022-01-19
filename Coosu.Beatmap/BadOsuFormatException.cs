using System;

namespace Coosu.Beatmap
{
    public sealed class BadOsuFormatException : Exception
    {
        public BadOsuFormatException(string message) : base(message)
        {
        }
    }
}