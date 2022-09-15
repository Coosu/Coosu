using System;

namespace Coosu.Storyboard.Storybrew;

public class StoryboardLogicException : Exception
{
    public StoryboardLogicException(string? message) : base(message)
    {
    }
}