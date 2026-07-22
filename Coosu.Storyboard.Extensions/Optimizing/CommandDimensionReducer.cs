using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Optimizing;

/// <summary>
/// Chooses one legal representation for command families whose members must not be mixed.
/// Candidates are committed only when their final serialized text is shorter.
/// </summary>
internal static class CommandDimensionReducer
{
    public static void Reduce(Sprite sprite, Func<Sprite, int> measureSerializedLength)
    {
        if (sprite == null) throw new ArgumentNullException(nameof(sprite));
        if (measureSerializedLength == null) throw new ArgumentNullException(nameof(measureSerializedLength));

        ApplyIfShorter(sprite, CanConvertAllVectorsToScale, ConvertAllVectorsToScale,
            measureSerializedLength);
        ApplyIfShorter(sprite, CanSplitAllMoves, SplitAllMoves,
            measureSerializedLength);
    }

    private static void ApplyIfShorter(Sprite sprite, Func<Sprite, bool> canTransform,
        Action<Sprite> transform, Func<Sprite, int> measureSerializedLength)
    {
        if (!canTransform(sprite)) return;

        var candidate = (Sprite)sprite.Clone();
        transform(candidate);
        if (measureSerializedLength(candidate) >= measureSerializedLength(sprite)) return;

        transform(sprite);
    }

    private static bool CanConvertAllVectorsToScale(Sprite sprite)
    {
        var vectors = EnumerateHosts(sprite)
            .SelectMany(k => k.Events)
            .OfType<Vector>()
            .ToArray();

        return vectors.Length > 0 && vectors.All(IsUniform);
    }

    private static bool IsUniform(Vector vector) =>
        vector.StartScaleX.Equals(vector.StartScaleY) &&
        vector.EndScaleX.Equals(vector.EndScaleY);

    private static void ConvertAllVectorsToScale(Sprite sprite)
    {
        foreach (var host in EnumerateHosts(sprite))
        {
            foreach (var vector in host.Events.OfType<Vector>().ToArray())
            {
                var scale = new Scale(vector.Easing, vector.StartTime, vector.EndTime,
                    new List<double> { vector.StartScaleX, vector.EndScaleX });
                host.RemoveEvent(vector);
                host.AddEvent(scale);
            }
        }
    }

    private static bool CanSplitAllMoves(Sprite sprite)
    {
        var events = EnumerateHosts(sprite)
            .SelectMany(k => k.Events)
            .ToArray();

        return events.OfType<Move>().Any() &&
               !events.OfType<MoveX>().Any() &&
               !events.OfType<MoveY>().Any();
    }

    private static void SplitAllMoves(Sprite sprite)
    {
        foreach (var host in EnumerateHosts(sprite))
        {
            foreach (var move in host.Events.OfType<Move>().ToArray())
            {
                var moveX = new MoveX(move.Easing, move.StartTime, move.EndTime,
                    new List<double> { move.StartX, move.EndX });
                var moveY = new MoveY(move.Easing, move.StartTime, move.EndTime,
                    new List<double> { move.StartY, move.EndY });

                host.RemoveEvent(move);
                host.AddEvent(moveX);
                host.AddEvent(moveY);
            }

            CollapseStaticAxis(host, EventTypes.MoveX);
            CollapseStaticAxis(host, EventTypes.MoveY);
        }
    }

    private static void CollapseStaticAxis(IDetailedEventHost host, EventType eventType)
    {
        var events = host.Events
            .OfType<BasicEvent>()
            .Where(k => k.EventType == eventType)
            .ToArray();
        if (events.Length == 0) return;

        foreach (var @event in events.Where(k => k.IsStartsEqualsEnds()))
            @event.Easing = LinearEase.Instance;

        var value = events[0].GetStarts().Single();
        if (events.Any(k => !k.IsStartsEqualsEnds() || !k.GetStarts().Single().Equals(value))) return;

        var startTime = events.Min(k => k.StartTime);
        var endTime = events.Max(k => k.EndTime);
        foreach (var @event in events)
            host.RemoveEvent(@event);

        host.AddEvent(BasicEvent.Create(eventType, LinearEase.Instance, startTime, endTime,
            new List<double> { value, value }));
    }

    private static IEnumerable<IDetailedEventHost> EnumerateHosts(Sprite sprite)
    {
        yield return sprite;

        foreach (var loop in sprite.LoopList)
            yield return loop;

        foreach (var trigger in sprite.TriggerList)
            yield return trigger;
    }
}
