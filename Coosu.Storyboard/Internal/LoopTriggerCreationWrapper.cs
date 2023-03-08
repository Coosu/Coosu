using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Internal;

internal class LoopTriggerCreationWrapper : IEventHostDisposable
{
    private readonly Loop? _loop;
    private readonly Trigger? _trigger;

    private readonly ILoopHost? _loopHost;
    private readonly ITriggerHost? _triggerHost;

    public LoopTriggerCreationWrapper(ILoopHost loopHost, Loop loop)
    {
        _loopHost = loopHost;
        _loop = loop;
    }

    public LoopTriggerCreationWrapper(ITriggerHost triggerHost, Trigger trigger)
    {
        _triggerHost = triggerHost;
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
            _loopHost!.AddLoop(_loop);
        }
        else
        {
            _triggerHost!.AddTrigger(_trigger!);
        }
    }

    private IEventHost GetBaseEventHost()
    {
        return _loop is null ? _trigger! : _loop;
    }
}