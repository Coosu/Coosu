using System;
using System.Runtime.Serialization;

namespace Coosu.Storyboard.Storybrew;

[Serializable]
public class StoryboardLogicException : Exception
{
    public StoryboardLogicException()
    {
    }

    public StoryboardLogicException(string? message) : base(message)
    {
    }
    
    public StoryboardLogicException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    protected StoryboardLogicException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}