using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Services.Base;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets;
using Astralis.Server.Services.Data.Configs;

namespace Astralis.Server.Services.Handlers;

public class MotdHandler : BaseHandlerService
{
    [ScriptConfigVariable("motd")] public MotdConfig Config { get; set; }

    private readonly IVariablesService _variablesService;

    public MotdHandler(
        IEventBusService eventBusService, INetworkServer networkServer, IVariablesService variablesService
    ) : base(eventBusService, networkServer)
    {
        _variablesService = variablesService;
        SubscribeNetworkEvent<MotdRequestMessage>(OnMotdRequest);
    }

    private async ValueTask OnMotdRequest(string sessionId, MotdRequestMessage request)
    {
        var response = new MotdResponseMessage(_variablesService.TranslateText(Config.Message));
        await SendNetworkMessageAsync(sessionId, response);
    }
}
