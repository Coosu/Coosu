using Coosu.Shared;

namespace Coosu.Storyboard.Extensibility;

public interface IParsingHandler
{
    string Flag { get; }
    object Deserialize(ref ValueListBuilder<string> split);
    string Serialize(object raw);
}

public interface IParsingHandler<T> : IParsingHandler
{
    new T Deserialize(ref ValueListBuilder<string> split);
    string Serialize(T raw);
}