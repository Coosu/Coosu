using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Parsing
{
    public interface IActionParsingHandler : IParsingHandler
    {
        int ParameterDimension { get; }
        new CommonEvent Deserialize(string[] split);
        string Serialize(CommonEvent raw);
    }

    public interface IActionParsingHandler<T> : IActionParsingHandler where T : CommonEvent
    {
        new T Deserialize(string[] split);
        string Serialize(T raw);
    }
}