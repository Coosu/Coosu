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

    public static ISubjectParsingHandler? GetSubjectHandler(string? flagString)
    {
        var hasValue = SubjectHandlerDic.TryGetValue(flagString, out var value);
        return hasValue ? value : null;
    }

    public static void RegisterEventTransformation(EventType eventType, EventCreationDelegate @delegate)
    {
        EventTransformationDictionary.Add(eventType, @delegate);
    }

    public static EventCreationDelegate? GetEventTransformation(EventType eventType)
    {
        var hasValue = EventTransformationDictionary.TryGetValue(eventType, out var value);
        return hasValue ? value : null;
    }

    public static T GetActionHandlerInstance<T>() where T : IActionParsingHandler, new()
    {
        var type = typeof(T);
        if (ActionHandlerInstances.TryGetValue(type, out var value))
        {
            return (T)value;
        }

        var inst = new T();
        ActionHandlerInstances.Add(type, inst);
        return inst;
    }
}