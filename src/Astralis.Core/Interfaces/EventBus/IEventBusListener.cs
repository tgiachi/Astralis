using System.Threading.Tasks;

namespace Astralis.Core.Interfaces.EventBus;

public interface IEventBusListener<in TEvent>
{
    Task OnEventAsync(TEvent message);
}
