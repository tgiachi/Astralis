using System.Drawing;
using Astralis.Game.Client.Core;
using Microsoft.Extensions.Hosting;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client;

public class AstralisGameClient : IHostedService
{
    private readonly OpenGlContext _openGlContext;

    public AstralisGameClient(OpenGlContext openGlContext)
    {
        _openGlContext = openGlContext;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => _openGlContext.Run(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _openGlContext.Stop();
        return Task.CompletedTask;
    }
}
