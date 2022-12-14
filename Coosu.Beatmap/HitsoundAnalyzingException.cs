using System;

namespace Coosu.Beatmap;

public class HitsoundAnalyzingException : Exception
{
    public HitsoundAnalyzingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}