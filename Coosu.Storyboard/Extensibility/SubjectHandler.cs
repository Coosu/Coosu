using System.Collections.Generic;
using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensibility;

public abstract class SubjectHandler<T> : ISubjectParsingHandler<T> where T : ISceneObject
{
    private readonly Dictionary<string, IActionParsingHandler> _actionHandlerDic = new();

    public abstract string Flag { get; }

    ISceneObject ISubjectParsingHandler.Deserialize(ref ValueListBuilder<string> split)
    {
        return Deserialize(ref split);
    }

    object IParsingHandler.Deserialize(ref ValueListBuilder<string> split)
    {
        return Deserialize(ref split);
    }

    string ISubjectParsingHandler.Serialize(ISceneObject raw)
    {
        return Serialize((T)raw);
    }

    string IParsingHandler.Serialize(object raw)
    {
        return Serialize((T)raw);
    }

    public abstract string Serialize(T raw);

    public abstract T Deserialize(ref ValueListBuilder<string> split);

    public IActionParsingHandler? GetActionHandler(string magicWord)
    {
        return _actionHandlerDic.ContainsKey(magicWord) ? _actionHandlerDic[magicWord] : null;
    }

    public IParsingHandler RegisterAction(IActionParsingHandler handler)
    {
        _actionHandlerDic.Add(handler.Flag, handler);
        return handler;
    }
}