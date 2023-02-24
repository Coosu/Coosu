using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensibility;

public interface IActionParsingHandler : IParsingHandler
{
    new IEvent Deserialize(ref ValueListBuilder<string> split);
    string Serialize(IEvent raw);
}

public interface IActionParsingHandler<T> : IActionParsingHandler where T : IEvent
{
    new T Deserialize(ref ValueListBuilder<string> split);
    string Serialize(T e);
}