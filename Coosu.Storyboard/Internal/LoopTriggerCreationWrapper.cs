using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Internal;

internal class LoopTriggerCreationWrapper : IDisposableEventHost
{
    private readonly Loop? _loop;
    private readonly Sprite _sprite;
    private readonly Trigger? _trigger;

    public LoopTriggerCreationWrapper(Sprite sprite, Loop loop)
    {
        _sprite = sprite;
        _loop = loop;
    }

    public LoopTriggerCreationWrapper(Sprite sprite, Trigger trigger)
    {
        _sprite = sprite;
        _trigger = trigger;
    }

    public Task WriteHeaderAsync(TextWriter writer) => GetBaseEventHost().WriteHeaderAsync(writer);
    public Task WriteScriptAsync(TextWriter writer) => GetBaseEventHost().WriteScriptAsync(writer);
    public IReadOnlyCollection<IKeyEvent> Events => GetBaseEventHost().Events;
    public void AddEvent(IKeyEvent @event) => GetBaseEventHost().AddEvent(@event);
    public bool RemoveEvent(IKeyEvent @event) => GetBaseEventHost().RemoveEvent(@event);
    public void ResetEventCollection(IComparer<IKeyEvent>? comparer) => GetBaseEventHost().ResetEventCollection(comparer);

    public void Dispose()
    {
        if (_loop is not null)
        {
            _sprite.AddLoop(_loop);
        }
        else
        {
            _sprite.AddTrigger(_trigger!);
        }
    }

    private IEventHost GetBaseEventHost()
    {
        return _loop is null ? _trigger! : _loop;
    }
}