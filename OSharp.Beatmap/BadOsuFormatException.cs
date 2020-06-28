using System;

namespace OSharp.Beatmap
{
    public class BadOsuFormatException : Exception
    {
        public BadOsuFormatException(string message) : base(message)
        {
        }
    }
}