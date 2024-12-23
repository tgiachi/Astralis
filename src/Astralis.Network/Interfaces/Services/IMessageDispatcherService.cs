using System;
using System.Threading.Tasks;
using Astralis.Network.Interfaces.Listeners;
using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Interfaces.Services;

public interface IMessageDispatcherService : IDisposable
{
    void RegisterMessageListener<TMessage>(INetworkMessageListener<TMessage> listener)
        where TMessage : class, INetworkMessage;

    void RegisterMessageListener<TMessage>(Func<string, TMessage, ValueTask> listener)
        where TMessage : class, INetworkMessage;

    void DispatchMessage<TMessage>(string sessionId, TMessage message) where TMessage : class, INetworkMessage;
}
