using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensibility;

public abstract class ActionHandler<T> : IActionParsingHandler<T> where T : IEvent
{
    public abstract string Flag { get; }

    IEvent IActionParsingHandler.Deserialize(ref ValueListBuilder<string> split)
    {
        return Deserialize(ref split);
    }

    string IActionParsingHandler.Serialize(IEvent raw)
    {
        return Serialize((T)raw);
    }

    string IParsingHandler.Serialize(object raw)
    {
        return Serialize((T)raw);
    }

    object IParsingHandler.Deserialize(ref ValueListBuilder<string> split)
    {
        return Deserialize(ref split);
    }

    public abstract T Deserialize(ref ValueListBuilder<string> split);
    public abstract string Serialize(T e);
}