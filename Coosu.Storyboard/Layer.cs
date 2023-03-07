using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Shared.Numerics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard;

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
        Camera2.CameraIdentifier = name;
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
        Camera2.CameraIdentifier = name;
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

    public double MaxTime() => SceneObjects.Count == 0 ? 0 : SceneObjects.Max(k => k.MaxTime());
    public double MinTime() => SceneObjects.Count == 0 ? 0 : SceneObjects.Min(k => k.MinTime());
    public double MaxStartTime() => SceneObjects.Count == 0 ? 0 : SceneObjects.Max(k => k.MaxStartTime());
    public double MinEndTime() => SceneObjects.Count == 0 ? 0 : SceneObjects.Min(k => k.MinEndTime());

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
        var layer = new Layer(0);
        var parsingContext = new ParsingContext(layer);

        string? line = textReader.ReadLine();
        bool isInEvent = true, isInVariable = false;
        while (line != null)
        {
            parsingContext.CurrentRow++;
            if (line.StartsWith("[Variables]", StringComparison.Ordinal))
            {
                isInVariable = true;
                isInEvent = false;
            }
            else if (line.StartsWith("[Events]", StringComparison.Ordinal))
            {
                isInVariable = false;
                isInEvent = true;
            }
            else if (line.StartsWith("//Storyboard Sound Samples", StringComparison.Ordinal))
            {
                break;
            }
            else if (line.StartsWith("//", StringComparison.Ordinal))
            {
                // ignored
            }
            else if (isInVariable && line.IndexOf('$') == 0)
            {
                var i = line.IndexOf('=');
                if (i >= 0)
                {
                    parsingContext.Variables.Add(new KeyValuePair<string, string>(
                        line.Substring(0, i), line.Substring(i + 1, line.Length - 1 - i)
                    ));
                }
            }
            else if (isInEvent)
            {
                try
                {
                    // Actually the best performance here, see ReplaceBenchmark.csproj
                    line = ParseVariables(line, parsingContext);
                    ParseObject(line, parsingContext);
                }
                catch (Exception e)
                {
                    throw new Exception($"Line {parsingContext.CurrentRow} {{{line}}}: {e.Message}");
                }
            }

            line = textReader.ReadLine();
        }

        return layer;
    }

    private static string ParseVariables(string line, ParsingContext context)
    {
        if (context.Variables.Count == 0 || line.IndexOf('$') < 0)
            return line;

        var count = context.Variables.Count;
        for (var i = count - 1; i >= 0; i--)
        {
            if (i < count - 1 && line.IndexOf('$') < 0)
                return line;
            var kvp = context.Variables[i];
            line = line.Replace(kvp.Key, kvp.Value);
        }

        return line;
    }

    private const char SplitChar = ',';
    private const char QuoteChar = '\"';
    //private static readonly string[] SpriteDefinitions = { "Sprite", "Animation", "4", "6" };
    //private static readonly char[] PrefixChars = { Prefix0, Prefix1 };
    //private static readonly string[] Prefixes = { " ", "_" };
    private const char Prefix0 = '_';
    private const char Prefix1 = ' ';
    private const string DoublePrefix0 = "__";
    private const string DoublePrefix1 = "  ";
    private const string TriplePrefix0 = "___";
    private const string TriplePrefix1 = "   ";
    //private static readonly string[] DoublePrefixes = { "  ", "__" };
    //private const string
    //    F = "F", S = "S", R = "R", Mx = "MX", My = "MY",
    //    M = "M", V = "V",
    //    C = "C",
    //    P = "P",
    //    L = "L", T = "T";

    private static void ParseObject(string line, ParsingContext context)
    {
        int i = -1;
        ReadOnlySpan<char> identifierSpan = default;
        ReadOnlySpan<char> others = default;
        foreach (var span in line.SpanSplit(SplitChar, context.SpanSplitArgs))
        {
            i++;
            switch (i)
            {
                case 0: identifierSpan = span; context.SpanSplitArgs.Canceled = true; break;
                case 1: others = span; break;
            }
        }

        context.SpanSplitArgs.Canceled = false;
        var identifier = identifierSpan.Trim().ToString();

        if (string.IsNullOrWhiteSpace(line))
        {
            context.IsLastLineBlank = true;
        }
        else if (ObjectType.Contains(identifier))
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

            context.CurrentObject?.TryEndLoop();

            if (i == 5)
            {
                context.CurrentObject = context.Layer.CreateSprite(
                    //@params[0],
                    param1,
                    param2,
                    param3.Trim(QuoteChar),
                    ParseHelper.ParseDouble(param4, ParseHelper.EnUsNumberFormat),
                    ParseHelper.ParseDouble(param5, ParseHelper.EnUsNumberFormat)
                );
                context.CurrentObject.RowInSource = context.CurrentRow;
                context.IsLooping = false;
                context.IsTriggering = false;
                context.IsLastLineBlank = false;
            }
            else if (i == 7)
            {
                context.CurrentObject = context.Layer.CreateAnimation(
                    //@params[0],
                    @param1,
                    @param2,
                    @param3.Trim(QuoteChar),
                    ParseHelper.ParseDouble(@param4, ParseHelper.EnUsNumberFormat),
                    ParseHelper.ParseDouble(@param5, ParseHelper.EnUsNumberFormat),
                    ParseHelper.ParseInt32(@param6),
                    ParseHelper.ParseDouble(@param7, ParseHelper.EnUsNumberFormat),
                    "LoopForever".AsSpan()
                );
                context.CurrentObject.RowInSource = context.CurrentRow;
                context.IsLooping = false;
                context.IsTriggering = false;
                context.IsLastLineBlank = false;
            }
            else if (i == 8)
            {
                context.CurrentObject = context.Layer.CreateAnimation(
                    //@params[0],
                    @param1,
                    @param2,
                    @param3.Trim(QuoteChar),
                    ParseHelper.ParseDouble(@param4, ParseHelper.EnUsNumberFormat),
                    ParseHelper.ParseDouble(@param5, ParseHelper.EnUsNumberFormat),
                    ParseHelper.ParseInt32(@param6),
                    ParseHelper.ParseDouble(@param7, ParseHelper.EnUsNumberFormat),
                    @param8
                );
                context.CurrentObject.RowInSource = context.CurrentRow;
                context.IsLooping = false;
                context.IsTriggering = false;
                context.IsLastLineBlank = false;
            }
            else
                throw new Exception("Sprite declared wrongly");
        }
        else
        {
            if (context.CurrentObject == null)
                throw new Exception("Sprite need to be declared before using");
            if (context.IsLastLineBlank)
                throw new Exception("Events shouldn't be declared after blank line");

            // 验证层次是否合法
            if (identifierSpan.StartsWith(TriplePrefix0.AsSpan()) ||
                identifierSpan.StartsWith(TriplePrefix1.AsSpan()))
            {
                throw new Exception("Unknown relation of the event");
            }
            else if (identifierSpan.StartsWith(DoublePrefix0.AsSpan()) ||
                     identifierSpan.StartsWith(DoublePrefix1.AsSpan()))
            {
                if (!context.IsLooping && !context.IsTriggering)
                    throw new Exception("The event should be looping or triggering");
            }
            else if (identifierSpan[0] == Prefix0 || identifierSpan[0] == Prefix1)
            {
                if (context.IsLooping || context.IsTriggering)
                {
                    context.CurrentObject?.EndLoop();
                    context.IsLooping = false;
                    context.IsTriggering = false;
                }
            }
            else
            {
                throw new Exception("Unknown relation of the event");
            }

            // 开始验证event类别
            byte easing = byte.MinValue;
            int startTime = int.MinValue;
            int endTime = int.MinValue;
            if (EventTypes.IsBasicEvent(identifier))
            {
                ReadOnlySpan<char> param1 = default;
                ReadOnlySpan<char> param2 = default;
                ReadOnlySpan<char> param3 = default;

                i = 0;
                foreach (var span in others.SpanSplit(SplitChar, context.SpanSplitArgs))
                {
                    i++;
                    switch (i)
                    {
                        case 1: param1 = span; break;
                        case 2: param2 = span; break;
                        case 3: param3 = span; context.SpanSplitArgs.Canceled = true; break;
                        case 4: break;
                    }
                }

                context.SpanSplitArgs.Canceled = false;

                easing = ParseHelper.ParseByte(param1);
                startTime = ParseHelper.ParseInt32(param2);
                endTime = param3.Length == 0 ? startTime : ParseHelper.ParseInt32(param3);

                if (easing is > 34 or < 0)
                    throw new FormatException("Unknown easing");
            }

            if (context.CurrentObject != null)
                ParseEvent(context, others, identifier, easing, startTime, endTime);
        }
    }

    private static void ParseEvent(ParsingContext context,
        ReadOnlySpan<char> rawParams,
        string identifier, byte easing, int startTime, int endTime)
    {
        EventType eventType = EventTypes.GetValue(identifier);
        if (eventType == EventType.Empty)
        {
            throw new Exception($"Unknown event: \"{identifier}\"");
        }

        var size = eventType.Size;
        if (size >= 1)
        {
            var t = 0;
            var valueStore = new List<double>(size * 2);
            foreach (var span in rawParams.SpanSplit(SplitChar))
            {
                t++;

                if (t >= 4)
                {
                    valueStore.Add(ParseHelper.ParseDouble(span, ParseHelper.EnUsNumberFormat));
                }
            }

            if (valueStore.Count <= size * 2 || (valueStore.Count) % size != 0)
            {
                var basicEvent = BasicEvent.Create(eventType, (EasingType)easing, startTime, endTime,
                    valueStore);
                context.CurrentObject!.AddEvent(basicEvent);
            }
            else
            {
                var duration = endTime - startTime;
                for (int i = 0, j = 0; i < rawParams.Length - size; i += size, j++)
                {
                    var copy = new List<double>(size * 2);

                    for (int k = 0; k < size * 2; k++)
                    {
                        copy.Add(rawParams[i + k]);
                    }

                    var newStartTime = startTime + duration * j;
                    var newEndTime = endTime + duration * j;
                    var basicEvent = BasicEvent.Create(eventType, (EasingType)easing,
                        newStartTime, newEndTime, copy);
                    context.CurrentObject!.AddEvent(basicEvent);
                }
            }
        }
        else
        {
            if (identifier == EventTypes.Parameter.Flag)
            {
                var t = 0;
                ParameterType p = default;
                foreach (var span in rawParams.SpanSplit(SplitChar))
                {
                    t++;
                    if (t == 4) p = span[0].ToParameterEnum();
                }

                if (t == 4)
                {
                    context.CurrentObject!.Parameter(startTime, endTime, p);
                }
                else
                {
                    throw new ArgumentException($"Incorrect parameter length for {identifier}: {t - 3}");
                }
            }
            else if (identifier == EventTypes.Loop.Flag)
            {
                var t = 0;
                int loopCount = default;
                foreach (var span in rawParams.SpanSplit(SplitChar))
                {
                    t++;
                    switch (t)
                    {
                        case 1: startTime = ParseHelper.ParseInt32(span); break;
                        case 2: loopCount = ParseHelper.ParseInt32(span); break;
                    }
                }

                if (t == 2)
                {
                    context.CurrentObject!.StartLoop(startTime, loopCount);
                    context.IsLooping = true;
                }
                else
                {
                    throw new ArgumentException($"Incorrect declaration length for {identifier}: {t + 1}");
                }
            }
            else if (identifier == EventTypes.Trigger.Flag)
            {
                var t = 0;
                ReadOnlySpan<char> triggerType = default;
                foreach (var span in rawParams.SpanSplit(SplitChar))
                {
                    t++;
                    switch (t)
                    {
                        case 1: triggerType = span; break;
                        case 2: startTime = ParseHelper.ParseInt32(span); break;
                        case 3: endTime = ParseHelper.ParseInt32(span); break;
                    }
                }

                if (t == 3)
                {
                    context.CurrentObject!.StartTrigger(startTime, endTime, triggerType.ToString());
                    context.IsTriggering = true;
                }
                else
                {
                    throw new ArgumentException($"Incorrect declaration length for {identifier}: {t + 1}");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(identifier));
            }
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

    private class ParsingContext
    {
        public ParsingContext(Layer layer)
        {
            Layer = layer;
            SpanSplitArgs = new SpanSplitArgs();
        }

        public Layer Layer { get; }
        public SpanSplitArgs SpanSplitArgs { get; }

        public int CurrentRow { get; set; }
        public Sprite? CurrentObject { get; set; }
        public bool IsLooping { get; set; }
        public bool IsTriggering { get; set; }
        public bool IsLastLineBlank { get; set; }
        public IList<KeyValuePair<string, string>> Variables { get; }
            = new List<KeyValuePair<string, string>>();
    }
}