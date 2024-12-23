using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Astralis.Core.Interfaces.EventBus;

namespace Astralis.Core.Interfaces.Services;

public interface IEventBusService
{
    Task PublishAsync<TEvent>(TEvent eventItem) where TEvent : class;
    void Publish<TEvent>(TEvent eventItem) where TEvent : class;
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
    void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class;
    IDisposable SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
    ISubject<object> AllEventsObservable { get; }
}
