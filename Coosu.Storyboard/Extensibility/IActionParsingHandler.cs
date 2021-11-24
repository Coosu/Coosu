using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public interface IActionParsingHandler : IParsingHandler
    {
        int ParameterDimension { get; }
        new BasicEvent Deserialize(string[] split);
        string Serialize(BasicEvent raw);
    }

    public interface IActionParsingHandler<T> : IActionParsingHandler where T : BasicEvent
    {
        new T Deserialize(string[] split);
        string Serialize(T raw);
    }
}