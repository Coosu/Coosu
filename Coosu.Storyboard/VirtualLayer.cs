using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard
{
    public class VirtualLayer : IDisposable, IScriptable
    {
        public void Dispose() { }

        public float ZDistance { get; set; }
        public VirtualLayer(float zDistance) => ZDistance = zDistance;

        public List<EventContainer> Elements { get; set; } = new();

        public EventContainer this[int index] => Elements[index];

        public IEnumerable<EventContainer> this[Func<EventContainer, bool> predicate] => Elements.Where(predicate);

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Element CreateSprite(string filePath)
        {
            var obj = new Element(ElementTypes.Sprite, LayerType.Foreground, OriginType.Centre, filePath, 320, 240);
            AddElement(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="origin">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Element CreateSprite(OriginType origin, string filePath)
        {
            var obj = new Element(ElementTypes.Sprite, LayerType.Foreground, origin, filePath, 320, 240);
            AddElement(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="layer">Layer of the image.</param>
        /// <param name="origin">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Element CreateSprite(LayerType layer, OriginType origin, string filePath)
        {
            var obj = new Element(ElementTypes.Sprite, layer, origin, filePath, 320, 240);
            AddElement(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="layer">Layer of the image.</param>
        /// <param name="origin">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <param name="defaultLocation">Default location of the image.</param>
        /// <returns></returns>
        public Element CreateSprite(LayerType layer, OriginType origin, string filePath, System.Drawing.Point defaultLocation)
        {
            var obj = new Element(ElementTypes.Sprite, layer, origin, filePath, defaultLocation.X, defaultLocation.Y);
            AddElement(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard element by a static image.
        /// </summary>
        /// <param name="layer">Layer of the image.</param>
        /// <param name="origin">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <param name="defaultX">Default x-coordinate of the image.</param>
        /// <param name="defaultY">Default y-coordinate of the image.</param>
        /// <returns></returns>
        public Element CreateSprite(LayerType layer, OriginType origin, string filePath, float defaultX, float defaultY)
        {
            var obj = new Element(ElementTypes.Sprite, layer, origin, filePath, defaultX, defaultY);
            AddElement(obj);
            return obj;
        }

        public AnimatedElement CreateAnimation(
            LayerType layer,
            OriginType origin,
            string filePath,
            int defaultX, int defaultY,
            int frameCount, float frameDelay, LoopType loopType)
        {
            var obj = new AnimatedElement(
                ElementTypes.Sprite,
                layer,
                origin,
                filePath,
                defaultX,
                defaultY,
                frameCount,
                frameDelay,
                loopType
            );
            AddElement(obj);
            return obj;
        }

        public void AddSubject(EventContainer element)
        {
            Elements.Add(element);
        }

        public void AddSubject(params EventContainer[] elements)
        {
            Elements.AddRange(elements);
        }

        public void AddElement(Element element)
        {
            Elements.Add(element);
        }

        public void AddElement(params Element[] elements)
        {
            Elements.AddRange(elements);
        }

        //public void ExecuteBrew(StoryboardLayer layParsed)
        //{
        //    throw new NotImplementedException();
        //    foreach (var lib in ElementList)
        //    {
        //        lib.ExecuteBrew(layParsed);
        //    }
        //}

        public async Task WriteScriptAsync(TextWriter sw)
        {
            foreach (var obj in Elements)
            {
                await obj.WriteScriptAsync(sw);
            }
        }

        public async Task WriteFullScriptAsync(TextWriter sw)
        {
            await sw.WriteLineAsync("[Events]");
            await sw.WriteLineAsync("//Background and Video events");
            await sw.WriteLineAsync("//Storyboard Layer 0 (Background)");
            await sw.WriteLineAsync("//Storyboard Layer 0 (Background)");
            await sw.WriteLineAsync("//Storyboard Layer 1 (Fail)");
            await sw.WriteLineAsync("//Storyboard Layer 3 (Foreground)");
            await WriteScriptAsync(sw);
            await sw.WriteLineAsync("//Storyboard Sound Samples");
        }

        public async Task<string> ToScriptStringAsync()
        {
            using var sw = new StringWriter();
            await WriteFullScriptAsync(sw);
            return sw.ToString();
        }

        public async Task SaveScriptAsync(string path)
        {
            using var sw = new StreamWriter(path, false);
            await WriteFullScriptAsync(sw);
        }

        public static async Task<VirtualLayer> ParseAsyncTextAsync(string osbString)
        {
            return await Task.Run(() => ParseFromText(osbString));
        }

        public static async Task<VirtualLayer> ParseFromFileAsync(string filePath)
        {
            return await Task.Run(() => ParseFromFile(filePath));
        }

        public static async Task<VirtualLayer> ParseAsync(TextReader textReader)
        {
            return await Task.Run(() => Parse(textReader));
        }

        public static VirtualLayer ParseFromText(string osbString)
        {
            using var sr = new StringReader(osbString);
            return Parse(sr);
        }

        public static VirtualLayer ParseFromFile(string filePath)
        {
            using var sr = new StreamReader(filePath);
            return Parse(sr);
        }

        public static VirtualLayer Parse(TextReader textReader)
        {
            VirtualLayer group = new VirtualLayer(0);
            Element currentObj = null;
            //0 = isLooping, 1 = isTriggering, 2 = isBlank
            bool[] options = { false, false, false };

            int rowIndex = 0;
            string line = textReader.ReadLine();
            while (line != null)
            {
                rowIndex++;
                if (line.StartsWith("//") || line.StartsWith("[Events]"))
                {
                    if (line.Contains("Sound Samples"))
                        break;
                }
                else
                {
                    try
                    {
                        currentObj = ParseElement(line, rowIndex, currentObj, group, options);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Line: {rowIndex}: {e.Message}");
                    }
                }

                line = textReader.ReadLine();
            }

            return group;
        }

        private const char SplitChar = ',';
        private const char QuoteChar = '\"';
        private static readonly string[] Sprites = { "Sprite", "Animation", "4", "6" };
        private static readonly char[] PrefixChars = { ' ', '_' };
        private static readonly string[] Prefixes = { " ", "_" };
        private static readonly string[] DoublePrefixes = { "  ", "__" };
        private const string
            F = "F", S = "S", R = "R", MX = "MX", MY = "MY",
            M = "M", V = "V",
            C = "C",
            P = "P",
            L = "L", T = "T";

        private static Element ParseElement(string line,
            int rowIndex,
            Element currentObj,
            VirtualLayer @group,
            bool[] options)
        {
            ref bool isLooping = ref options[0];
            ref bool isTriggering = ref options[1];
            ref bool isBlank = ref options[2];

            //int count = line.Count(k => k == ',') + 1;
            var @params = line.Split(SplitChar);
            if (Sprites.Contains(@params[0]))
            {
                currentObj?.TryEndLoop();

                if (@params.Length == 6)
                {
                    currentObj = group.CreateSprite(
                        @params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        float.Parse(@params[4]),
                        float.Parse(@params[5])
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (@params.Length == 8)
                {
                    currentObj = group.CreateAnimation(
                        @params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        float.Parse(@params[4]),
                        float.Parse(@params[5]),
                        int.Parse(@params[6]),
                        float.Parse(@params[7]),
                        "LoopForever"
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (@params.Length == 9)
                {
                    currentObj = group.CreateAnimation(
                        @params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        float.Parse(@params[4]),
                        float.Parse(@params[5]),
                        int.Parse(@params[6]),
                        float.Parse(@params[7]),
                        @params[8]
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else
                    throw new Exception("Sprite declared wrongly");
            }
            else if (string.IsNullOrEmpty(line.Trim()))
            {
                isBlank = true;
            }
            else
            {
                if (currentObj == null)
                    throw new Exception("Sprite need to be declared before using");
                if (isBlank)
                    throw new Exception("Events shouldn't be declared after blank line");

                // 验证层次是否合法
                if (@params[0].Length - @params[0].TrimStart(PrefixChars).Length > 2)
                {
                    throw new Exception("Unknown relation of the event");
                }
                else if (DoublePrefixes.Any(k => @params[0].StartsWith(k)))
                {
                    if (!isLooping && !isTriggering)
                        throw new Exception("The event should be looping or triggering");
                }
                else if (Prefixes.Any(k => @params[0].StartsWith(k)))
                {
                    if (isLooping || isTriggering)
                    {
                        currentObj.EndLoop();
                        isLooping = false;
                        isTriggering = false;
                    }
                }
                else
                {
                    throw new Exception("Unknown relation of the event");
                }

                // 开始验证event类别
                @params[0] = @params[0].TrimStart(PrefixChars);

                string _event = @params[0];
                int easing = int.MinValue, startTime = int.MinValue, endTime = int.MinValue;

                if (_event != T && _event != L)
                {
                    easing = int.Parse(@params[1]);
                    if (easing > 34 || easing < 0)
                        throw new FormatException("Unknown easing");
                    startTime = int.Parse(@params[2]);
                    endTime = @params[3] == ""
                        ? startTime
                        : int.Parse(@params[3]);
                }

                ParseEvent(currentObj, options, @params, _event, easing, startTime, endTime);
            }

            return currentObj;
        }

        private static void ParseEvent(Element currentObj, bool[] options, string[] rawParams,
            string eventStr, int easing, int startTime, int endTime)
        {
            int rawLength = rawParams.Length;
            switch (rawParams[0])
            {
                case F:
                case S:
                case R:
                case MX:
                case MY:
                    AddEvent(1);
                    break;
                case M:
                case V:
                    AddEvent(2);
                    return;
                case C:
                    AddEvent(3);
                    break;
                case P:
                case L:
                case T:
                    AddEvent(0);
                    break;
                default:
                    throw new Exception($"Unknown event: \"{eventStr}\"");
            }

            void AddEvent(int paramLength)
            {
                if (paramLength != 0)
                {
                    const int baseLength = 4;
                    // 验证是否存在缺省
                    if (rawLength == paramLength + baseLength)
                    {
                        int length = paramLength * 2;
                        Span<float> array = stackalloc float[length];
                        for (int i = 0; i < paramLength; i++)
                            array[i] = float.Parse(rawParams[baseLength + i]);
                        for (int i = 0; i < paramLength; i++)
                            array[i + paramLength] = float.Parse(rawParams[baseLength + i]);

                        InjectEvent(array);
                    }
                    else if (rawLength == paramLength * 2 + baseLength)
                    {
                        int length = paramLength * 2;
                        Span<float> array = stackalloc float[length];
                        for (int i = 0; i < length; i++)
                            array[i] = float.Parse(rawParams[baseLength + i]);

                        InjectEvent(array);
                    }
                    else if (rawLength > paramLength * 2 + baseLength && (rawLength - baseLength) % paramLength == 0)
                    {
                        var duration = endTime - startTime;
                        for (int i = baseLength, j = 0; i < rawParams.Length - paramLength; i += paramLength, j++)
                        {
                            ParseEvent(currentObj, options,
                                new[] { rawParams[0], null, null, null }
                                    .Concat(
                                        rawParams
                                            .Skip(i)
                                            .Take(paramLength * 2)
                                    )
                                    .ToArray(),
                                eventStr,
                                easing,
                                startTime + duration * j,
                                endTime + duration * j);
                        }
                    }
                    else
                    {
                        throw new Exception($"Wrong parameter for event: \"{eventStr}\"");
                    }
                }
                else
                {
                    switch (eventStr)
                    {
                        case P:
                            if (rawLength == 5)
                            {
                                currentObj.Parameter(
                                    (EasingType)easing,
                                    startTime,
                                    endTime,
                                    rawParams[4].ToParameterEnum());
                                return;
                            }

                            break;
                        case L:
                            if (rawLength == 3)
                            {
                                startTime = int.Parse(rawParams[1]);
                                int loopCount = int.Parse(rawParams[2]);
                                currentObj.StartLoop(startTime, loopCount);

                                options[0] = true; // isLooping
                                return;
                            }

                            break;
                        case T:
                            if (rawLength == 4)
                            {
                                startTime = int.Parse(rawParams[2]);
                                endTime = int.Parse(rawParams[3]);

                                currentObj.StartTrigger(startTime, endTime, rawParams[1]); // rawParams[1]: triggerType
                                options[1] = true; // isTriggering
                                return;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eventStr));
                    }

                    throw new Exception($"Wrong parameter for event: \"{eventStr}\"");
                }

                void InjectEvent(Span<float> array)
                {
                    switch (eventStr)
                    {
                        case F:
                            currentObj.Fade((EasingType)easing,
                                startTime, endTime,
                                array[0],
                                array[1]
                            );
                            break;
                        case S:
                            currentObj.Scale((EasingType)easing,
                                startTime, endTime,
                                array[0],
                                array[1]
                            );
                            break;
                        case R:
                            currentObj.Rotate((EasingType)easing,
                                startTime, endTime,
                                array[0],
                                array[1]
                            );
                            break;
                        case MX:
                            currentObj.MoveX((EasingType)easing,
                                startTime, endTime,
                                array[0],
                                array[1]
                            );
                            break;
                        case MY:
                            currentObj.MoveY((EasingType)easing,
                                startTime, endTime,
                                array[0],
                                array[1]
                            );
                            break;
                        case M:
                            currentObj.Move((EasingType)easing,
                                startTime, endTime,
                                array[0], array[1],
                                array[2], array[3]
                            );
                            break;
                        case V:
                            currentObj.Vector((EasingType)easing,
                                startTime, endTime,
                                array[0], array[1],
                                array[2], array[3]
                            );
                            break;
                        case C:
                            currentObj.Color((EasingType)easing,
                                startTime, endTime,
                                array[0], array[1], array[2],
                                array[3], array[4], array[5]
                            );
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eventStr));
                    }
                }
            }
        }

        private Element CreateSprite(string type, string layer, string origin, string imagePath, float defaultX, float defaultY)
        {
            var obj = new Element(type, layer, origin, imagePath, defaultX, defaultY);
            AddElement(obj);
            return obj;
        }

        private Element CreateAnimation(string type, string layer, string origin, string imagePath, float defaultX,
            float defaultY, int frameCount, float frameDelay, string loopType)
        {
            var obj = new AnimatedElement(type, layer, origin, imagePath, defaultX, defaultY, frameCount, frameDelay, loopType);
            AddElement(obj);
            return obj;
        }
    }
}
