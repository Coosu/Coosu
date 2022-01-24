using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    /// <summary>
    /// Coosu layer.
    /// <para>This is more of a group for controlling a set of sprites instead of osu!storyboard layer.</para>
    /// </summary>
    public class Layer : ISpriteHost, IAdjustable
    {
        /// <summary>
        /// Layer name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Create a new layer.
        /// </summary>
        /// <param name="name"></param>
        public Layer(string name = "CoosuDefaultLayer")
        {
            Camera2.DefaultZ = 1;
            Name = name;
        }

        /// <summary>
        /// Create a new layer.
        /// </summary>
        /// <param name="defaultZ">Layer Z-distance.</param>
        /// <param name="name">Layer name.</param>
        public Layer(double defaultZ, string name = "CoosuDefaultLayer")
        {
            Camera2.DefaultZ = defaultZ;
            Name = name;
        }

        /// <summary>
        /// Sprite list in this layer.
        /// </summary>
        public List<ISceneObject> SceneObjects { get; set; } = new();

        public void AddObject(ISceneObject @object)
        {
            SceneObjects.Add(@object);
        }

        public void AddObjects(params ISceneObject[] objects)
        {
            SceneObjects.AddRange(objects);
        }

        public void AdjustTiming(double offset)
        {
            foreach (var sceneObject in SceneObjects)
            {
                sceneObject.Adjust(0, 0, offset);
            }
        }

        public void AdjustPosition(double x, double y)
        {
            foreach (var sceneObject in SceneObjects)
            {
                sceneObject.Adjust(x, y, 0);
            }
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync("Layer: ");
            await writer.WriteAsync(Name);
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

        public static async Task<Layer> ParseAsyncTextAsync(string osbString)
        {
            return await Task.Run(() => ParseFromText(osbString));
        }

        public static async Task<Layer> ParseFromFileAsync(string filePath)
        {
            return await Task.Run(() => ParseFromFile(filePath));
        }

        public static async Task<Layer> ParseAsync(TextReader textReader)
        {
            return await Task.Run(() => Parse(textReader));
        }

        public static Layer ParseFromText(string osbString)
        {
            using var sr = new StringReader(osbString);
            return Parse(sr);
        }

        public static Layer ParseFromFile(string filePath)
        {
            using var sr = new StreamReader(filePath);
            return Parse(sr);
        }

        public static Layer Parse(TextReader textReader)
        {
            Layer group = new Layer(0);
            IDefinedObject? currentObj = null;
            //0 = isLooping, 1 = isTriggering, 2 = isBlank
            bool[] options = { false, false, false };

            var spanSplitArgs = new SpanSplitArgs();
            var valueCache = new List<double>();
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
                        currentObj = ParseObject(line, rowIndex, currentObj, group, options, spanSplitArgs, valueCache);
                        valueCache.Clear();
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
            Layer layer,
            bool[] options,
            in SpanSplitArgs e,
            in List<double> valueCache)
        {
            var sprite = currentObj as Sprite;
            ref bool isLooping = ref options[0];
            ref bool isTriggering = ref options[1];
            ref bool isBlank = ref options[2];

            int i = -1;
            ReadOnlySpan<char> param0 = default;
            ReadOnlySpan<char> others = default;
            foreach (var span in line.SpanSplit(SplitChar, e))
            {
                i++;
                switch (i)
                {
                    case 0: param0 = span; e.Canceled = true; break;
                    case 1: others = span; break;
                }
            }

            e.Canceled = false;
            var identifier = param0.ToString();

            if (ObjectType.Contains(identifier))
            {
                ReadOnlySpan<char> param1 = default;
                ReadOnlySpan<char> param2 = default;
                ReadOnlySpan<char> param3 = default;
                ReadOnlySpan<char> param4 = default;
                ReadOnlySpan<char> param5 = default;
                ReadOnlySpan<char> param6 = default;
                ReadOnlySpan<char> param7 = default;
                ReadOnlySpan<char> param8 = default;

                i = 0;
                foreach (var span in others.SpanSplit(SplitChar))
                {
                    i++;
                    switch (i)
                    {
                        case 1: param1 = span; break;
                        case 2: param2 = span; break;
                        case 3: param3 = span; break;
                        case 4: param4 = span; break;
                        case 5: param5 = span; break;
                        case 6: param6 = span; break;
                        case 7: param7 = span; break;
                        case 8: param8 = span; break;
                    }
                }

                sprite?.TryEndLoop();

                if (i == 5)
                {
                    currentObj = layer.CreateSprite(
                        //@params[0],
                        param1,
                      param2,
                        param3.Trim(QuoteChar),
                        double.Parse(param4),
                        double.Parse(param5)
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (i == 7)
                {
                    currentObj = layer.CreateAnimation(
                        //@params[0],
                        @param1,
                        @param2,
                        @param3.Trim(QuoteChar),
                        double.Parse(@param4),
                        double.Parse(@param5),
                        int.Parse(@param6),
                        double.Parse(@param7),
                        "LoopForever"
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else if (i == 8)
                {
                    currentObj = layer.CreateAnimation(
                        //@params[0],
                        @param1,
                        @param2,
                        @param3.Trim(QuoteChar),
                        double.Parse(@param4),
                        double.Parse(@param5),
                        int.Parse(@param6),
                        double.Parse(@param7),
                        @param8
                    );
                    currentObj.RowInSource = rowIndex;
                    isLooping = false;
                    isTriggering = false;
                    isBlank = false;
                }
                else
                    throw new Exception("Sprite declared wrongly");
            }
            else if (string.IsNullOrWhiteSpace(line))
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

                e.Canceled = false;
                if (EventTypes.IsBasicEvent(identifier))
                {
                    ReadOnlySpan<char> param1 = default;
                    ReadOnlySpan<char> param2 = default;
                    ReadOnlySpan<char> param3 = default;

                    i = 0;
                    foreach (var span in others.SpanSplit(SplitChar, e))
                    {
                        i++;
                        switch (i)
                        {
                            case 1: param1 = span; break;
                            case 2: param2 = span; break;
                            case 3: param3 = span; e.Canceled = true; break;
                            case 4: break;
                        }
                    }

                    easing = int.Parse(param1);
                    if (easing is > 34 or < 0)
                        throw new FormatException("Unknown easing");
                    startTime = int.Parse(param2);
                    endTime = param3.Length == 0
                        ? startTime
                        : int.Parse(param3);
                }

                if (sprite != null)
                    ParseEvent(sprite, options, others, identifier, easing, startTime, endTime, e, valueCache);
            }

            return currentObj!;
        }

        private static void ParseEvent(Sprite currentObj, bool[] options,
            ReadOnlySpan<char> rawParams,
            string identifier, int easing, int startTime, int endTime,
            in SpanSplitArgs e)
        {
            //int rawLength = rawParams.Length;
            var eventType = EventTypes.GetValue(identifier);
            if (eventType != default)
            {
                var size = eventType.Size;
                if (size >= 1)
                {
                    var t = 0;
                    var valueStore = new List<double>();
                    foreach (var span in rawParams.SpanSplit(SplitChar, e))
                    {
                        t++;
                        if (t >= 4)
                        {
                            valueStore.Add(double.Parse(span));
                        }
                    }

                    var basicEvent = BasicEvent.Create(eventType, (EasingType)easing, startTime, endTime, valueStore);

                    //const int baseLength = 3;


                    // 验证是否存在缺省
                    if (doubleList.Count == size)
                    {
                        InjectEvent(others);
                    }
                    else if (doubleList.Count == size * 2)
                    {
                        InjectEvent(array);
                    }
                    else if (doubleList.Count > size * 2 && (doubleList.Count) % size == 0)
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
                                startTime,
                                endTime,
                                rawParams[4].ToParameterEnum());
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


            void InjectEvent(Span<double> span)
            {
                var basicEvent = BasicEvent.Create(eventType, (EasingType)easing, startTime, endTime, span);
                currentObj.AddEvent(basicEvent);
            }
        }

        private Sprite CreateSprite(ReadOnlySpan<char> layer,
            ReadOnlySpan<char> origin,
            ReadOnlySpan<char> imagePath,
            double defaultX, double defaultY)
        {
            var obj = new Sprite(layer, origin, imagePath, defaultX, defaultY);
            AddObject(obj);
            return obj;
        }

        private Sprite CreateAnimation(ReadOnlySpan<char> layer,
            ReadOnlySpan<char> origin,
            ReadOnlySpan<char> imagePath,
            double defaultX, double defaultY,
            int frameCount, double frameDelay, ReadOnlySpan<char> loopType)
        {
            var obj = new Animation(layer, origin, imagePath, defaultX, defaultY, frameCount, frameDelay, loopType);
            AddObject(obj);
            return obj;
        }

        #region ISpriteHost

        public object Clone()
        {
            return new Layer(Camera2.DefaultZ, Name)
            {
                SceneObjects = SceneObjects.Select(k => k.Clone()).Cast<ISceneObject>().ToList()
            };
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return Sprites.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public double MaxTime => SceneObjects.Count == 0 ? 0 : SceneObjects.Max(k => k.MaxTime);
        public double MinTime => SceneObjects.Count == 0 ? 0 : SceneObjects.Min(k => k.MinTime);
        public double MaxStartTime => SceneObjects.Count == 0 ? 0 : SceneObjects.Max(k => k.MaxStartTime);
        public double MinEndTime => SceneObjects.Count == 0 ? 0 : SceneObjects.Min(k => k.MinEndTime);

        public IList<Sprite> Sprites => SceneObjects
            .Where(k => k is Sprite)
            .Cast<Sprite>()
            .ToList();

        public IList<ISpriteHost> SubHosts => SceneObjects
            .Where(k => k is ISpriteHost)
            .Cast<ISpriteHost>()
            .ToList();

        public Camera2 Camera2 { get; } = new();
        public void AddSprite(Sprite sprite)
        {
            AddObject(sprite);
        }

        public void AddSubHost(ISpriteHost spriteHost)
        {
            if (spriteHost is not ISceneObject iso) throw new InvalidCastException();
            AddObject(iso);
        }

        public ISpriteHost? BaseHost => null;
        public Dictionary<string, object> Tags { get; } = new();

        #endregion
    }
}