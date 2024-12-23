using System.Threading.Tasks;

namespace Astralis.Core.Interfaces.Services.Base;

public interface IAstralisGameService
{
    Task OnReadyAsync();

    Task OnShutdownAsync();

}
