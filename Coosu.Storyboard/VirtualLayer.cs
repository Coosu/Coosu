using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    public class VirtualLayer : IDisposable, IScriptable
    {
        public void Dispose() { }

        public double ZDistance { get; set; }
        public VirtualLayer(double zDistance) => ZDistance = zDistance;

        public List<ISceneObject> SceneObjects { get; set; } = new();

        public ISceneObject this[int index] => SceneObjects[index];

        public IEnumerable<ISceneObject> this[Func<ISceneObject, bool> predicate] => SceneObjects.Where(predicate);

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Sprite CreateSprite(string filePath)
        {
            var obj = new Sprite(LayerType.Foreground, OriginType.Centre, filePath, 320, 240);
            AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="originType">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Sprite CreateSprite(OriginType originType, string filePath)
        {
            var obj = new Sprite(LayerType.Foreground, originType, filePath, 320, 240);
            AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="layerType">Layer of the image.</param>
        /// <param name="originType">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <returns></returns>
        public Sprite CreateSprite(LayerType layerType, OriginType originType, string filePath)
        {
            var obj = new Sprite(layerType, originType, filePath, 320, 240);
            AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="layerType">Layer of the image.</param>
        /// <param name="originType">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <param name="defaultLocation">Default location of the image.</param>
        /// <returns></returns>
        public Sprite CreateSprite(LayerType layerType, OriginType originType, string filePath, System.Drawing.Point defaultLocation)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultLocation.X, defaultLocation.Y);
            AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite by a static image.
        /// </summary>
        /// <param name="layerType">Layer of the image.</param>
        /// <param name="originType">Origin of the image.</param>
        /// <param name="filePath">File path of the image.</param>
        /// <param name="defaultX">Default x-coordinate of the image.</param>
        /// <param name="defaultY">Default y-coordinate of the image.</param>
        /// <returns></returns>
        public Sprite CreateSprite(LayerType layerType, OriginType originType, string filePath, double defaultX, double defaultY)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultX, defaultY);
            AddSprite(obj);
            return obj;
        }

        public Animation CreateAnimation(
            LayerType layerType,
            OriginType originType,
            string filePath,
            int defaultX, int defaultY,
            int frameCount, double frameDelay, LoopType loopType)
        {
            var obj = new Animation(
                layerType,
                originType,
                filePath,
                defaultX,
                defaultY,
                frameCount,
                frameDelay,
                loopType
            );
            AddSprite(obj);
            return obj;
        }

        public void AddSprite(ISceneObject sprite)
        {
            SceneObjects.Add(sprite);
        }

        public void AddSprite(params ISceneObject[] sprites)
        {
            SceneObjects.AddRange(sprites);
        }

        //public void ExecuteBrew(StoryboardLayer layParsed)
        //{
        //    throw new NotImplementedException();
        //    foreach (var lib in ElementList)
        //    {
        //        lib.ExecuteBrew(layParsed);
        //    }
        //}

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync("Layer: ");
            await writer.WriteAsync(ZDistance);
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            foreach (var obj in SceneObjects)
            {
                await obj.WriteScriptAsync(writer);
            }
        }

        public async Task WriteFullScriptAsync(TextWriter writer)
        {
            await writer.WriteLineAsync("[Events]");
            await writer.WriteLineAsync("//Background and Video events");
            await writer.WriteLineAsync("//Storyboard Layer 0 (Background)");
            await writer.WriteLineAsync("//Storyboard Layer 1 (Fail)");
            await writer.WriteLineAsync("//Storyboard Layer 2 (Pass)");
            await writer.WriteLineAsync("//Storyboard Layer 3 (Foreground)");
            await WriteScriptAsync(writer);
            await writer.WriteLineAsync("//Storyboard Layer 4 (Overlay)");
            await writer.WriteLineAsync("//Storyboard Sound Samples");
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
            IDefinedObject? currentObj = null;
            //0 = isLooping, 1 = isTriggering, 2 = isBlank
            bool[] options = { false, false, false };

            int rowIndex = 0;
            string? line = textReader.ReadLine();
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
                        currentObj = ParseObject(line, rowIndex, currentObj, group, options);
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
        //private static readonly string[] SpriteDefinitions = { "Sprite", "Animation", "4", "6" };
        private static readonly char[] PrefixChars = { ' ', '_' };
        private static readonly string[] Prefixes = { " ", "_" };
        private static readonly string[] DoublePrefixes = { "  ", "__" };
        //private const string
        //    F = "F", S = "S", R = "R", Mx = "MX", My = "MY",
        //    M = "M", V = "V",
        //    C = "C",
        //    P = "P",
        //    L = "L", T = "T";

        private static IDefinedObject ParseObject(string line,
            int rowIndex,
            IDefinedObject? currentObj,
            VirtualLayer layer,
            bool[] options)
        {
            var sprite = currentObj as Sprite;
            ref bool isLooping = ref options[0];
            ref bool isTriggering = ref options[1];
            ref bool isBlank = ref options[2];

            //int count = line.Count(k => k == ',') + 1;
            var @params = line.Split(SplitChar);
            string identifier = @params[0];

            if (ObjectType.Contains(identifier))
            {
                sprite?.TryEndLoop();

                if (@params.Length == 6)
                {
                    currentObj = layer.CreateSprite(
                        //@params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        double.Parse(@params[4]),
                        double.Parse(@params[5])
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (@params.Length == 8)
                {
                    currentObj = layer.CreateAnimation(
                        //@params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        double.Parse(@params[4]),
                        double.Parse(@params[5]),
                        int.Parse(@params[6]),
                        double.Parse(@params[7]),
                        "LoopForever"
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (@params.Length == 9)
                {
                    currentObj = layer.CreateAnimation(
                        //@params[0],
                        @params[1],
                        @params[2],
                        @params[3].Trim(QuoteChar),
                        double.Parse(@params[4]),
                        double.Parse(@params[5]),
                        int.Parse(@params[6]),
                        double.Parse(@params[7]),
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
                if (identifier.Length - identifier.TrimStart(PrefixChars).Length > 2)
                {
                    throw new Exception("Unknown relation of the event");
                }
                else if (DoublePrefixes.Any(k => identifier.StartsWith(k)))
                {
                    if (!isLooping && !isTriggering)
                        throw new Exception("The event should be looping or triggering");
                }
                else if (Prefixes.Any(k => identifier.StartsWith(k)))
                {
                    if (isLooping || isTriggering)
                    {
                        sprite?.EndLoop();
                        isLooping = false;
                        isTriggering = false;
                    }
                }
                else
                {
                    throw new Exception("Unknown relation of the event");
                }

                // 开始验证event类别
                identifier = identifier.TrimStart(PrefixChars);

                int easing = int.MinValue, startTime = int.MinValue, endTime = int.MinValue;

                if (EventType.IsCommonEvent(identifier))
                {
                    easing = int.Parse(@params[1]);
                    if (easing is > 34 or < 0)
                        throw new FormatException("Unknown easing");
                    startTime = int.Parse(@params[2]);
                    endTime = @params[3] == ""
                        ? startTime
                        : int.Parse(@params[3]);
                }

                if (sprite != null)
                    ParseEvent(sprite, options, @params, identifier, easing, startTime, endTime);
            }

            return currentObj!;
        }

        private static void ParseEvent(Sprite currentObj, bool[] options, string?[] rawParams,
            string identifier, int easing, int startTime, int endTime)
        {
            int rawLength = rawParams.Length;
            var eventType = EventType.GetValue(identifier);
            if (eventType != default)
            {
                var size = eventType.Size;
                if (size >= 1)
                {
                    const int baseLength = 4;
                    // 验证是否存在缺省
                    if (rawLength == size + baseLength)
                    {
                        int length = size * 2;
                        Span<double> array = stackalloc double[length];
                        for (int i = 0; i < size; i++)
                            array[i] = double.Parse(rawParams[baseLength + i]!);
                        for (int i = 0; i < size; i++)
                            array[i + size] = double.Parse(rawParams[baseLength + i]!);

                        InjectEvent(array);
                    }
                    else if (rawLength == size * 2 + baseLength)
                    {
                        int length = size * 2;
                        Span<double> array = stackalloc double[length];
                        for (int i = 0; i < length; i++)
                            array[i] = double.Parse(rawParams[baseLength + i]!);

                        InjectEvent(array);
                    }
                    else if (rawLength > size * 2 + baseLength && (rawLength - baseLength) % size == 0)
                    {
                        var duration = endTime - startTime;
                        for (int i = baseLength, j = 0; i < rawParams.Length - size; i += size, j++)
                        {
                            ParseEvent(currentObj, options,
                                new[] { rawParams[0], null, null, null }
                                    .Concat(
                                        rawParams
                                            .Skip(i)
                                            .Take(size * 2)
                                    )
                                    .ToArray(),
                                identifier,
                                easing,
                                startTime + duration * j,
                                endTime + duration * j);
                        }
                    }
                    else
                    {
                        throw new Exception($"Wrong parameter for event: \"{identifier}\"");
                    }
                }
                else
                {
                    if (identifier == EventTypes.Parameter.Flag)
                    {
                        if (rawLength == 5)
                        {
                            currentObj.Parameter(
                                (EasingType)easing,
                                startTime,
                                endTime,
                                rawParams[4]!.ToParameterEnum());
                            return;
                        }
                    }
                    else if (identifier == EventTypes.Loop.Flag)
                    {
                        if (rawLength == 3)
                        {
                            startTime = int.Parse(rawParams[1]!);
                            int loopCount = int.Parse(rawParams[2]!);
                            currentObj.StartLoop(startTime, loopCount);

                            options[0] = true; // isLooping
                            return;
                        }
                    }
                    else if (identifier == EventTypes.Trigger.Flag)
                    {
                        if (rawLength == 4)
                        {
                            startTime = int.Parse(rawParams[2]!);
                            endTime = int.Parse(rawParams[3]!);

                            currentObj.StartTrigger(startTime, endTime, rawParams[1]!); // rawParams[1]: triggerType
                            options[1] = true; // isTriggering
                            return;
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(identifier));
                    }

                    throw new Exception($"Wrong parameter for event: \"{identifier}\"");
                }
            }
            else
            {
                throw new Exception($"Unknown event: \"{identifier}\"");
            }


            void InjectEvent(Span<double> array)
            {
                var commonEvent = CommonEvent.Create(eventType, (EasingType)easing, startTime, endTime, array.ToArray());
                currentObj.AddEvent(commonEvent);
            }
        }

        private Sprite CreateSprite(string layer, string origin, string imagePath, double defaultX, double defaultY)
        {
            var obj = new Sprite(layer, origin, imagePath, defaultX, defaultY);
            AddSprite(obj);
            return obj;
        }

        private Sprite CreateAnimation(string layer, string origin, string imagePath, double defaultX,
            double defaultY, int frameCount, double frameDelay, string loopType)
        {
            var obj = new Animation(layer, origin, imagePath, defaultX, defaultY, frameCount, frameDelay, loopType);
            AddSprite(obj);
            return obj;
        }
    }
}
