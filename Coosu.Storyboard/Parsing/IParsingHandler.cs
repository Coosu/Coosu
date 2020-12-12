namespace Coosu.Storyboard.Parsing
{
    public interface IParsingHandler
    {
        string Flag { get; }
        object Deserialize(string[] split);
        string Serialize(object raw);
    }

    public interface IParsingHandler<T> : IParsingHandler
    {
        new T Deserialize(string[] split);
        string Serialize(T raw);
    }
}