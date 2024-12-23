using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Serilog;

namespace Astralis.Core.Services;

public class EventBusService : IEventBusService
{
    private readonly ILogger _logger = Log.ForContext<EventBusService>();

    private readonly ConcurrentDictionary<Type, object> _subjects = new();

    public ISubject<object> AllEventsObservable { get; } = new Subject<object>();


    public async Task PublishAsync<TEvent>(TEvent eventItem) where TEvent : class
    {
        _logger.Debug("Publishing event: {Event}", eventItem);
        if (_subjects.TryGetValue(typeof(TEvent), out var subject))
        {
            var typedSubject = (ISubject<TEvent>)subject;
            typedSubject.OnNext(eventItem);
        }

        AllEventsObservable.OnNext(eventItem);
    }

    public void Publish<TEvent>(TEvent eventItem) where TEvent : class
    {
        _logger.Debug("Publishing event: {Event}", eventItem);

        if (_subjects.TryGetValue(typeof(TEvent), out var subject))
        {
            var typedSubject = (ISubject<TEvent>)subject;
            typedSubject.OnNext(eventItem);
        }

        AllEventsObservable.OnNext(eventItem);
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var subject = (ISubject<TEvent>)_subjects.GetOrAdd(typeof(TEvent), _ => new Subject<TEvent>());
        return subject.AsObservable().Subscribe(handler);
    }

    public IDisposable SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var subject = (ISubject<TEvent>)_subjects.GetOrAdd(typeof(TEvent), _ => new Subject<TEvent>());
        return subject.AsObservable().Subscribe(async e => await handler(e));
    }


    public void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        var subject = (ISubject<TEvent>)_subjects.GetOrAdd(typeof(TEvent), _ => new Subject<TEvent>());
        subject.AsObservable().Subscribe(async e => await listener.OnEventAsync(e));
    }
}
