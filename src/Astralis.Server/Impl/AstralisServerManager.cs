using Astralis.Core.Interfaces.Services;
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Events.Engine;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Impl;

public class AstralisServerManager : IAstralisServerManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusService _eventBusService;
    private readonly IScriptEngineSystemService _scriptEngineSystemService;
    private readonly IConfigService _configService;
    private readonly ILogger _logger = Log.ForContext<AstralisServerManager>();
    private readonly List<SystemServiceData> _systemServices;
    private readonly List<GameServiceData> _gameServices;

    public AstralisServerManager(
        IServiceProvider serviceProvider, List<SystemServiceData> systemServices, IEventBusService eventBusService,
        IScriptEngineSystemService scriptEngineSystemService, IConfigService configService,
        List<GameServiceData> gameServices
    )
    {
        _serviceProvider = serviceProvider;
        _systemServices = systemServices;
        _eventBusService = eventBusService;
        _scriptEngineSystemService = scriptEngineSystemService;
        _configService = configService;
        _gameServices = gameServices;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var data in _systemServices.OrderBy(x => x.Priority))
        {
            await StartSystemService(data);
        }

        await _eventBusService.PublishAsync(new EngineStartedEvent());


        await LuaBootstrapAsync(cancellationToken);

        foreach (var service in _gameServices.OrderBy(x => x.Priority))
        {
            _logger.Information("Starting game service {ServiceType}", service.InterfaceType.Name);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var serviceInstance = scope.ServiceProvider.GetService(service.InterfaceType);

            CheckScriptVariables(serviceInstance);

            if (serviceInstance is IAstralisGameService hostedService)
            {
                _logger.Debug("Calling OnReadyAsync for {ServiceType}", service.InterfaceType.Name);
                await hostedService.OnReadyAsync();
                _logger.Debug("Called OnReadyAsync for {ServiceType}", service.InterfaceType.Name);
            }
        }
    }

    private async Task LuaBootstrapAsync(CancellationToken cancellationToken)
    {
        var isBootstrapOk = await _scriptEngineSystemService.BootstrapAsync();

        if (!isBootstrapOk)
        {
            _logger.Error("Failed to bootstrap script engine, shutting down");
            await StopAsync(cancellationToken);

            await Task.Delay(1000, cancellationToken);
            Environment.Exit(1);
        }
    }

    private void CheckScriptVariables(object gameService)
    {
        _configService.SearchForConfigAttributes(gameService);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _eventBusService.PublishAsync(new EngineShuttingDownEvent());

        foreach (var service in _gameServices.OrderByDescending(x => x.Priority))
        {
            _logger.Information("Shutdown service {ServiceType}", service.InterfaceType.Name);

            await using var scope = _serviceProvider.CreateAsyncScope();
            var serviceInstance = scope.ServiceProvider.GetService(service.InterfaceType);

            if (serviceInstance is IAstralisGameService hostedService)
            {
                await hostedService.OnShutdownAsync();
            }
        }

        foreach (var data in _systemServices.OrderByDescending(x => x.Priority))
        {
            StopSystemService(data);
        }
    }

    private async Task StopSystemService(SystemServiceData data)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService(data.InterfaceType) as IAstralisSystemService;

            if (service != null)
            {
                await service.StopAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to stop system service {SystemServiceName}", data.ImplementationType.Name);
        }
    }

    private async Task StartSystemService(SystemServiceData data)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService(data.InterfaceType) as IAstralisSystemService;

            _logger.Information("Starting system service {SystemServiceName}", data.InterfaceType.Name);
            if (service != null)
            {
                await service.StartAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to start system service {SystemServiceName}", data.ImplementationType.Name);
        }
    }
}
