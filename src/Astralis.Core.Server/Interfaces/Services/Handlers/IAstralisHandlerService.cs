namespace Astralis.Core.Server.Interfaces.Services.Handlers;

public interface IAstralisHandlerService
{
    public Task OnReadyAsync();

    public Task OnShutdownAsync();
}
