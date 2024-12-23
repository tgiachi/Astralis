using System.Threading.Tasks;

namespace Astralis.Core.Interfaces.Services.Base;

public interface IAstralisSystemService
{
    Task StartAsync();

    Task StopAsync();
}
