using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Coosu.Osbx.SubjectHandlers;
using Coosu.Storyboard;
using Coosu.Storyboard.Management;
using Coosu.Storyboard.Parsing;

namespace Coosu.Osbx
{
    public static class OsbxConvert
    {
        static OsbxConvert()
        {
            Register.RegisterSubject(new SpriteHandler());
            Register.RegisterSubject(new AnimationHandler());
            Register.RegisterSubject(new Camera2Handler());
        }

        public static async Task<string> SerializeObjectAsync(ElementManager manager)
        {
            var sb = new StringBuilder();
            foreach (var @group in manager.GroupList.Values)
            {
                var result = await SerializeObjectAsync(group);
                sb.AppendLine(result);
            }

            return sb.ToString().TrimEnd('\n', '\r'); ;
        }

        public static async Task<string> SerializeObjectAsync(ElementGroup group)
        {
            var sb = new StringBuilder();
            foreach (var element in group.ElementList)
            {
                var result = await SerializeObjectAsync(element);
                sb.AppendLine(result);
            }

            return sb.ToString().TrimEnd('\n', '\r'); ;
        }

        public static async Task<string> SerializeObjectAsync(EventContainer element)
        {
            var sb = new StringBuilder();
            var subjectHandler = Register.GetSubjectHandler(ElementTypeSign.GetString(element.Type));
            if (subjectHandler == null)
            {
                Console.WriteLine(
                    $"Cannot find subject handler for `{ElementTypeSign.GetString(element.Type)}`: Skipped.");
                return "";
            }

            try
            {
                var text = subjectHandler.Serialize(element);
                sb.AppendLine(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error while serializing element: `{ElementTypeSign.GetString(element.Type)}`\r\n{ex}");
                return "";
            }

            foreach (var commonEvent in element.EventList)
            {
                var actionHandler = subjectHandler.GetActionHandler(commonEvent.EventType.Flag);
                if (actionHandler == null)
                {
                    Console.WriteLine($"Cannot find action handler for `{commonEvent.EventType.Flag}`: Skipped.");
                    continue;
                }

                try
                {
                    var result = " " + actionHandler.Serialize(commonEvent);
                    sb.AppendLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while serializing action: `{commonEvent.EventType.Flag}`\r\n{ex}");
                }
            }

            //else
            //{
            //    Console.WriteLine($"Unknown type of EventContainer: `{element.ToOsbString()}`");
            //}

            return sb.ToString().TrimEnd('\n', '\r');
        }

        public static async Task<ElementManager> DeserializeObjectAsync(TextReader reader)
        {
            ISubjectParsingHandler lastSubjectHandler = null;
            EventContainer lastSubject = null;
            //int lastDeep = 0;
            var line = await reader.ReadLineAsync();
            int l = 0;

            var em = new ElementManager();

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
                        var handler = Register.GetSubjectHandler(mahou);
                        if (handler == null && int.TryParse(mahou, out var flag))
                        {
                            var magicWord = ElementTypeSign.GetString(flag);
                            if (magicWord != null) handler = Register.GetSubjectHandler(magicWord);
                        }

                        if (handler != null)
                        {
                            try
                            {
                                var subject = handler.Deserialize(split);
                                var eg = em.GetOrAddGroup(subject.ZIndex);
                                eg.AddSubject(subject);

                                lastSubject = subject;
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
}
