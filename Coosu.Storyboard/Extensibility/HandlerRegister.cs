using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Extensibility;

public static class HandlerRegister
{
    private static readonly Dictionary<EventType, EventCreationDelegate> EventTransformationDictionary = new();
    private static readonly Dictionary<string, ISubjectParsingHandler> SubjectHandlerDic = new();
    private static readonly Dictionary<Type, IActionParsingHandler> ActionHandlerInstances = new();

    public static ISubjectParsingHandler RegisterSubject(ISubjectParsingHandler handler)
    {
        SubjectHandlerDic.Add(handler.Flag, handler);
        return handler;
    }

    public static ISubjectParsingHandler? GetSubjectHandler(string flagString)
    {
        return SubjectHandlerDic.ContainsKey(flagString) ? SubjectHandlerDic[flagString] : null;
    }

    public static void RegisterEventTransformation(EventType eventType, EventCreationDelegate @delegate)
    {
        EventTransformationDictionary.Add(eventType, @delegate);
    }

    public static EventCreationDelegate? GetEventTransformation(EventType eventType)
    {
        if (EventTransformationDictionary.ContainsKey(eventType))
            return EventTransformationDictionary[eventType];
        return null;
    }

    public static T GetActionHandlerInstance<T>() where T : IActionParsingHandler, new()
    {
        var type = typeof(T);
        if (ActionHandlerInstances.ContainsKey(type))
            return (T)ActionHandlerInstances[type];

        var inst = new T();
        ActionHandlerInstances.Add(type, inst);
        return inst;
    }
}