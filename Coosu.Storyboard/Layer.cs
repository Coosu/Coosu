using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
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
        /// Layer Z-distance.
        /// The default value is 1.
        /// The range of the value is from 0 to <see cref="double.MaxValue"/>
        /// </summary>
        public double ZDistance { get; set; }

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
            ZDistance = 1;
            Name = name;
        }

        /// <summary>
        /// Create a new layer.
        /// </summary>
        /// <param name="zDistance">Layer Z-distance.</param>
        /// <param name="name">Layer name.</param>
        public Layer(double zDistance, string name = "CoosuDefaultLayer")
        {
            ZDistance = zDistance;
            Name = name;
        }

        /// <summary>
        /// Sprite list in this layer.
        /// </summary>
        public List<ISceneObject> SceneObjects { get; set; } = new();

        #region Create Sprite

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath)
        {
            var obj = new Sprite(LayerType.Foreground, OriginType.Centre, filePath, 320, 240);
            AddObject(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath, OriginType originType)
        {
            var obj = new Sprite(LayerType.Foreground, originType, filePath, 320, 240);
            AddObject(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath, LayerType layerType)
        {
            var obj = new Sprite(layerType, OriginType.Centre, filePath, 320, 240);
            AddObject(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath, LayerType layerType, OriginType originType)
        {
            var obj = new Sprite(layerType, originType, filePath, 320, 240);
            AddObject(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <param name="defaultLocation">The sprite's default location.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath, LayerType layerType, OriginType originType, Vector2 defaultLocation)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultLocation.X, defaultLocation.Y);
            AddObject(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <param name="defaultX">The sprite's default x.</param>
        /// <param name="defaultY">The sprite's default y.</param>
        /// <returns>Created sprite.</returns>
        public Sprite CreateSprite(string filePath, LayerType layerType, OriginType originType, double defaultX,
            double defaultY)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultX, defaultY);
            AddObject(obj);
            return obj;
        }

        #endregion

        /// <summary>
        /// Create a storyboard animation.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The animation's layer.</param>
        /// <param name="originType">The animation's origin.</param>
        /// <param name="defaultX">The animation's default x.</param>
        /// <param name="defaultY">The animation's default y.</param>
        /// <param name="frameCount">The animation's total frame count.</param>
        /// <param name="frameDelay">The animation's frame delay between each frames.</param>
        /// <param name="loopType">The animation's loop type.</param>
        /// <returns>Created animation.</returns>
        public Animation CreateAnimation(
            string filePath,
            LayerType layerType,
            OriginType originType,
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
            AddObject(obj);
            return obj;
        }

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
            Layer layer,
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

                if (EventTypes.IsBasicEvent(identifier))
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
            var eventType = EventTypes.GetValue(identifier);
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


            void InjectEvent(Span<double> span)
            {
                var basicEvent = BasicEvent.Create(eventType, (EasingType)easing, startTime, endTime, span);
                currentObj.AddEvent(basicEvent);
            }
        }

        private Sprite CreateSprite(string layer, string origin, string imagePath, double defaultX, double defaultY)
        {
            var obj = new Sprite(layer, origin, imagePath, defaultX, defaultY);
            AddObject(obj);
            return obj;
        }

        private Sprite CreateAnimation(string layer, string origin, string imagePath, double defaultX,
            double defaultY, int frameCount, double frameDelay, string loopType)
        {
            var obj = new Animation(layer, origin, imagePath, defaultX, defaultY, frameCount, frameDelay, loopType);
            AddObject(obj);
            return obj;
        }

        #region ISpriteHost

        public object Clone()
        {
            return new Layer(ZDistance, Name)
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

        public ICollection<Sprite> Sprites => SceneObjects
            .Where(k => k is Sprite)
            .Cast<Sprite>()
            .ToList();

        public Camera2 Camera2 { get; } = new();

        #endregion
    }
}