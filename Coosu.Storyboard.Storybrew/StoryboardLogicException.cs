using System;

namespace Coosu.Storyboard.Storybrew;

[Serializable]
public class StoryboardLogicException : Exception
{
    public StoryboardLogicException(string? message) : base(message)
    {
    }
}