using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        ISubjectParsingHandler lastSubjectHandler = null;
        IDetailedEventHost lastSubject = null;
        //int lastDeep = 0;
        var line = await reader.ReadLineAsync();
        int l = 0;

        var em = new Scene();

        while (line != null)
        {
            if (line.StartsWith("//") || line.StartsWith("[") && line.EndsWith("]"))
            {

            }
            else
            {
                var split = line.Split(',');
                var mahou = split[0].TrimStart();
                var deep = split[0].Length - mahou.Length;
                if (deep == 0)
                {
                    var handler = HandlerRegister.GetSubjectHandler(mahou);
                    if (handler == null && int.TryParse(mahou, out var flag))
                    {
                        var magicWord = ObjectType.GetString(flag);
                        if (magicWord != null) handler = HandlerRegister.GetSubjectHandler(magicWord);
                    }

                    if (handler != null)
                    {
                        try
                        {
                            var sprite = handler.Deserialize(split);
                            var eg = em.GetOrAddLayer(sprite.CameraIdentifier);
                            eg.AddObject(sprite);

                            lastSubject = sprite;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while parsing subject. L {l}: `{line}`\r\n{ex}");
                            lastSubject = null;
                            handler = null;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cannot find subject handler. L {l}: `{line}`");
                    }

                    lastSubjectHandler = handler;
                }
                else if (deep == 1)
                {
                    if (lastSubjectHandler != null)
                    {
                        var actHandler = lastSubjectHandler.GetActionHandler(mahou);
                        if (actHandler != null)
                        {
                            try
                            {
                                var action = actHandler.Deserialize(split);
                                lastSubject.ApplyAction(action);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while parsing action. L {l}: `{line}`\r\n{ex}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Cannot find action handler. L {l}: `{line}`");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Current subject is null, skipped. L {l}: `{line}`");
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown deep {deep}. L {l}: `{line}`");
                }
            }

            //var deepMinus = deep - lastDeep;

            //lastDeep = deep;
            //if (deepMinus < 0)
            //{
            //    while (expression)
            //    {
            //        stack.pop;
            //    }
            //}


            line = await reader.ReadLineAsync();
            l++;
        }

        return em;
        //var count = Environment.ProcessorCount - 1;
        //var locks = new SemaphoreSlim(count, count);
        //var strLock = new object();

        //bool isFinished = false;
        //var str = await reader.ReadLineAsync();
        //if (str != null)
        //{

        //}

        //while (true)
        //{
        //    lock (strLock)
        //    {
        //        if (isFinished) break;
        //    }

        //    var inStr = await reader.ReadLineAsync();

        //    if (inStr == null)
        //    {
        //        lock (strLock)
        //        {
        //            isFinished = true;
        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        //while (locks.CurrentCount != count)
        //{
        //    await Task.Delay(1);
        //}
    }

    //public static ElementGroup DeserializeObject(string str)
    //{

    //}
}