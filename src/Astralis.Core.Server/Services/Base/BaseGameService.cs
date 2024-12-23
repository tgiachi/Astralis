using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Interfaces.Services.Base;

namespace Astralis.Core.Server.Services.Base;

public abstract class BaseGameService : IAstralisGameService
{
    private readonly IEventBusService _eventBusService;

    protected BaseGameService(IEventBusService eventBusService)
    {
        _eventBusService = eventBusService;
    }

    protected Task SendEventAsync<TEvent>(TEvent @event) where TEvent : class
    {
        return _eventBusService.PublishAsync(@event);
    }

    protected void SendEvent<TEvent>(TEvent @event) where TEvent : class
    {
        _eventBusService.Publish(@event);
    }


    protected void SubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        _eventBusService.Subscribe(handler);
    }

    protected void SubscribeEvent<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        _eventBusService.Subscribe<TEvent>(listener);
    }

    protected void SubscribeEventAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        _eventBusService.SubscribeAsync(handler);
    }


    public virtual Task OnReadyAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnShutdownAsync()
    {
        return Task.CompletedTask;
    }
}
