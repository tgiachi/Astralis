using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Interfaces.Services.System;
using NLua;


namespace Astralis.Server.Scripts;

[ScriptModule]
public class EventsModule
{
    private readonly IEventDispatcherService _eventDispatcherService;

    public EventsModule(IEventDispatcherService eventDispatcherService)
    {
        _eventDispatcherService = eventDispatcherService;
    }


    [ScriptFunction("on_event")]
    public void AddEvent(string eventName, LuaFunction function)
    {
        _eventDispatcherService.SubscribeToEvent(eventName, (obj) => { function.Call(obj); });
    }
}
