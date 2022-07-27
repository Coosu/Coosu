using System;

namespace Coosu.Shared;

public class ValueConvertException : Exception
{
    public ValueConvertException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}