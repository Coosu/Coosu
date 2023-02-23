using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.SubjectHandlers;

namespace Coosu.Storyboard.OsbX;

public static class OsbxConvert
{
    static OsbxConvert()
    {
        HandlerRegister.RegisterSubject(new SpriteHandler());
        HandlerRegister.RegisterSubject(new AnimationHandler());
        HandlerRegister.RegisterSubject(new Camera25Handler());
    }

    public static async Task<string> SerializeObjectAsync(Scene manager)
    {
        var sb = new StringBuilder();
        foreach (var @group in manager.Layers.Values)
        {
            var result = await SerializeObjectAsync(group);
            sb.AppendLine(result);
        }

        return sb.ToString().TrimEnd('\n', '\r'); ;
    }

    public static async Task<string> SerializeObjectAsync(Layer group)
    {
        var sb = new StringBuilder();
        foreach (var sceneObject in group.SceneObjects)
        {
            var result = await SerializeObjectAsync(sceneObject);
            sb.AppendLine(result);
        }

        return sb.ToString().TrimEnd('\n', '\r'); ;
    }

    public static async Task<string> SerializeObjectAsync(ISceneObject sceneObject)
    {
        var sb = new StringBuilder();
        var subjectHandler = HandlerRegister.GetSubjectHandler(ObjectType.GetString(sceneObject.ObjectType));
        if (subjectHandler == null)
        {
            Console.WriteLine(
                $"Cannot find subject handler for `{ObjectType.GetString(sceneObject.ObjectType)}`: Skipped.");
            return "";
        }

        try
        {
            var text = subjectHandler.Serialize(sceneObject);
            sb.AppendLine(text);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Error while serializing element: `{ObjectType.GetString(sceneObject.ObjectType)}`\r\n{ex}");
            return "";
        }

        foreach (var keyEvent in sceneObject.Events)
        {
            var actionHandler = subjectHandler.GetActionHandler(keyEvent.EventType.Flag);
            if (actionHandler == null)
            {
                Console.WriteLine($"Cannot find action handler for `{keyEvent.EventType.Flag}`: Skipped.");
                continue;
            }

            try
            {
                var result = " " + actionHandler.Serialize(keyEvent);
                sb.AppendLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while serializing action: `{keyEvent.EventType.Flag}`\r\n{ex}");
            }
        }

        //else
        //{
        //    Console.WriteLine($"Unknown type of EventContainer: `{sceneObject.ToOsbString()}`");
        //}

        return sb.ToString().TrimEnd('\n', '\r');
    }

    public static async Task<Scene> DeserializeObjectAsync(TextReader reader)
    {
        ISubjectParsingHandler? lastSubjectHandler = null;
        IDetailedEventHost? lastSubject = null;
        IDetailedEventHost? lastSubSubject = null;

        var line = await reader.ReadLineAsync();
        int lineIndex = 0;

        var scene = new Scene();

        while (line != null)
        {
            if (line.StartsWith("//") || line.StartsWith("[") && line.EndsWith("]"))
            {

            }
            else
            {
                HandleLine(scene, lineIndex, line, ref lastSubject, ref lastSubSubject, ref lastSubjectHandler);
            }

            line = await reader.ReadLineAsync();
            lineIndex++;
        }

        return scene;
    }

    private static void HandleLine(Scene scene, int lineIndex, string line,
        ref IDetailedEventHost? lastSubject, ref IDetailedEventHost? lastSubSubject, ref ISubjectParsingHandler? lastSubjectHandler)
    {
        var array = ArrayPool<string>.Shared.Rent(16);
        var split = new ValueListBuilder<string>(array);
        try
        {
            var enumerator = line.SpanSplit(',');
            foreach (var span in enumerator)
            {
                split.Append(span.ToString());
            }

            var flagString = split[0].TrimStart();
            var deep = split[0].Length - flagString.Length;
            if (deep == 0)
            {
                var handler = HandlerRegister.GetSubjectHandler(flagString);
                if (handler == null && int.TryParse(flagString, out var flag))
                {
                    flagString = ObjectType.GetString(flag);
                    if (flagString != null) handler = HandlerRegister.GetSubjectHandler(flagString);
                }
                
                lastSubjectHandler = handler;
                if (handler == null)
                {
                    Console.WriteLine($"Cannot find subject handler. L {lineIndex}: `{line}`");
                    return;
                }

                try
                {
                    var sprite = handler.Deserialize(ref split);
                    var eg = scene.GetOrAddLayer(sprite.CameraIdentifier);
                    eg.AddObject(sprite);

                    lastSubject = sprite;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while parsing subject. L {lineIndex}: `{line}`\r\n{ex}");
                    lastSubject = null;
                    lastSubjectHandler = null;
                }
            }
            else
            {
                if (lastSubjectHandler == null)
                {
                    Console.WriteLine($"Current subject is null, skipped. L {lineIndex}: `{line}`");
                    return;
                }

                var actHandler = lastSubjectHandler.GetActionHandler(flagString);
                if (actHandler == null)
                {
                    Console.WriteLine($"Cannot find action handler. L {lineIndex}: `{line}`");
                    return;
                }

                try
                {
                    var action = actHandler.Deserialize(ref split);
                    IDetailedEventHost? eventHost;
                    if (deep == 1)
                    {
                        lastSubSubject = null;
                        eventHost = lastSubject;
                    }
                    else if (deep == 2)
                    {
                        if (lastSubSubject == null)
                        {
                            Console.WriteLine($"Deep is {deep} but no host event. L {lineIndex}: `{line}`");
                            return;
                        }
                        else
                        {
                            eventHost = lastSubSubject;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unknown deep {deep}. L {lineIndex}: `{line}`");
                        return;
                    }

                    eventHost?.ApplyAction(action);

                    if (action is IDetailedEventHost detailedEventHost)
                    {
                        lastSubSubject = detailedEventHost;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while parsing action. L {lineIndex}: `{line}`\r\n{ex}");
                }
            }
        }
        finally
        {
            split.Dispose();
        }
    }
}