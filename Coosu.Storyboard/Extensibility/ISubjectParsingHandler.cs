using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensibility
{
    public interface ISubjectParsingHandler : IParsingHandler
    {
        IActionParsingHandler? GetActionHandler(string magicWord);
        new ISceneObject Deserialize(string[] split);
        string Serialize(ISceneObject raw);
    }

    public interface ISubjectParsingHandler<T> : ISubjectParsingHandler where T : ISceneObject
    {
        new T Deserialize(string[] split);
        string Serialize(T raw);
    }
}