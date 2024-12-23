using System.Threading.Tasks;
using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Interfaces.Listeners;

public interface INetworkMessageListener<in TMessage> where TMessage : class, INetworkMessage
{
    ValueTask OnMessageReceivedAsync(string sessionId, TMessage message);
}
