using System;

namespace Coosu.Beatmap
{
    public class BadOsuFormatException : Exception
    {
        public BadOsuFormatException(string message) : base(message)
        {
        }
    }
}