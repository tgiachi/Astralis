using System.Threading.Tasks;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Packets;

namespace Astralis.Network.Interfaces.Services;

public interface INetworkMessageFactory
{
    Task<INetworkPacket> SerializeAsync<T>(T message) where T : class;

    Task<INetworkMessage> ParseAsync(INetworkPacket packet);
}
