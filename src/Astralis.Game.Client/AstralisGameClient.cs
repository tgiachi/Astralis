
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Server.Data.Internal;
using Astralis.Game.Client.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace Astralis.Game.Client;

public class AstralisGameClient : IHostedService
{
    private readonly ILogger _logger = Log.ForContext<AstralisGameClient>();

    private readonly IOpenGlContext _openGlContext;
    private readonly IServiceProvider _serviceProvider;

    private readonly List<SystemServiceData> _systemServiceData;

    public AstralisGameClient(
        IOpenGlContext openGlContext, List<SystemServiceData> systemServiceData, IServiceProvider serviceProvider
    )
    {
        _openGlContext = openGlContext;
        _systemServiceData = systemServiceData;
        _serviceProvider = serviceProvider;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => _openGlContext.Run(), cancellationToken);

        await StartServicesAsync();
    }


    private async Task StartServicesAsync()
    {
        foreach (var systemServiceData in _systemServiceData.OrderBy(s => s.Priority))
        {
            var service = _serviceProvider.GetRequiredService(systemServiceData.InterfaceType);

            _logger.Information("Starting service {ServiceType}", service.GetType().Name);

            if (service is IAstralisSystemService astralisSystemService)
            {
                await astralisSystemService.StartAsync();
            }
        }
    }

    private async Task StopServicesAsync()
    {
        foreach (var systemServiceData in _systemServiceData.OrderByDescending(s => s.Priority))
        {
            var service = _serviceProvider.GetRequiredService(systemServiceData.InterfaceType);

            _logger.Information("Stopping service {ServiceType}", service.GetType().Name);
            if (service is IAstralisSystemService astralisSystemService)
            {
                await astralisSystemService.StopAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _openGlContext.Stop();

        return StopServicesAsync();
    }
}
