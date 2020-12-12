namespace Coosu.Storyboard.Extensibility
{
    public interface ISubjectParsingHandler : IParsingHandler
    {
        IActionParsingHandler GetActionHandler(string magicWord);
        new EventContainer Deserialize(string[] split);
        string Serialize(EventContainer raw);
    }

    public interface ISubjectParsingHandler<T> : ISubjectParsingHandler where T : EventContainer
    {
        new T Deserialize(string[] split);
        string Serialize(T raw);
    }
}