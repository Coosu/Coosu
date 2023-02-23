using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensibility;

public interface ISubjectParsingHandler : IParsingHandler
{
    IActionParsingHandler? GetActionHandler(string flagString);
    new ISceneObject Deserialize(ref ValueListBuilder<string> split);
    string Serialize(ISceneObject raw);
}

public interface ISubjectParsingHandler<T> : ISubjectParsingHandler where T : ISceneObject
{
    new T Deserialize(ref ValueListBuilder<string> split);
    string Serialize(T raw);
}